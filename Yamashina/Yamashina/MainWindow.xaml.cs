// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private List<PageSaveData> pages;
        private bool isBacked = false;
        private JsonSerializerOptions options;

        public MainWindow()
        {
            InitializeComponent();

            pages = [];
            options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            Activated += MainWindow_Activated;
            SuperFrame.Navigating += SuperFrame_Navigated;
            SuperNavigation.BackRequested += SuperNavigation_BackRequested;
            SuperNavigation.ItemInvoked += SuperNavigation_ItemInvoked;
            Closed += MainWindow_Closed;
        }

        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            string jsonStr = JsonSerializer.Serialize(pages, options);
            IStorageItem? storageItem = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("cachedpages.json");
            if (storageItem != null)
            {
                if (storageItem.IsOfType(StorageItemTypes.File))
                {
                    StorageFile file = (StorageFile)storageItem;
                    await FileIO.WriteTextAsync(file, jsonStr);
                }
                return;
            }
            StorageFile storage = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("cachedpages.json");
            await FileIO.WriteTextAsync(storage, jsonStr);
        }

        private void SuperNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string item = (string)args.InvokedItem;
            ResourceLoader resourceLoader = new();
            if (item == resourceLoader.GetString("BS/Content"))
                SuperFrame.Navigate(typeof(BalanceSheet));
            else if (item == resourceLoader.GetString("CB/Content"))
                SuperFrame.Navigate(typeof(PaymentMethods));
            else if (args.IsSettingsInvoked)
                SuperFrame.Navigate(typeof(BackupPage));
            else if (item == resourceLoader.GetString("Monthly/Content"))
                SuperFrame.Navigate(typeof(MonthlyStat));
        }

        private void SuperNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            pages.RemoveAt(pages.Count - 1);
            PageSaveData? pageSaveData = pages.LastOrDefault();
            if (pageSaveData != null)
            {
                Type? pType = Type.GetType(pageSaveData.PageType);
                if (pType != null)
                {
                    isBacked = true;
                    SuperFrame.Navigate(
                        pType,
                        pageSaveData.Parameters,
                        new SlideNavigationTransitionInfo
                        {
                            Effect = SlideNavigationTransitionEffect.FromLeft,
                        });
                    return;
                }
            }
            SuperNavigation.IsBackEnabled = false;
            return;
        }

        private void SuperFrame_Navigated(object sender, NavigatingCancelEventArgs e)
        {
            if (isBacked)
            {
                isBacked = false;
                return;
            }
            PageSaveData pageSaveData = new()
            {
                PageType = e.SourcePageType.AssemblyQualifiedName ?? throw new InvalidOperationException("No page has been found."),
                ParamType = e.Parameter?.GetType()?.AssemblyQualifiedName,
                Parameters = e.Parameter,
            };
            pages.Add(pageSaveData);
            if (pages.Count > 2)
                SuperNavigation.IsBackEnabled = true;
            else
                SuperNavigation.IsBackEnabled = false;
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (SuperFrame.SourcePageType == null)
            {
                List<PageSaveData>? data = await LoadJsonDataAsync();
                if (data == null || data.Count == 0)
                {
                    SuperFrame.Navigate(typeof(BalanceSheet));
                    return;
                }
                pages = data;
                PageSaveData data1 = data.Last();
                Type? pageType = Type.GetType(data1.PageType);
                if (pageType == null)
                {
                    SuperFrame.Navigate(typeof(BalanceSheet));
                    return;
                }
                SuperFrame.Navigate(pageType, data1.Parameters);
                return;
            }
        }

        private static async Task<List<PageSaveData>?> LoadJsonDataAsync()
        {
            IStorageItem? item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("cachedpages.json");
            if (item is null)
                return null;
            else if (item.IsOfType(StorageItemTypes.File))
            {
                StorageFile file = (StorageFile)item;
                string jsonContent = await FileIO.ReadTextAsync(file);
                if (string.IsNullOrEmpty(jsonContent)) return null;
                List<PageSaveData>? data = JsonSerializer.Deserialize<List<PageSaveData>>(jsonContent);
                return data;
            }
            return null;
        }
    }
}
