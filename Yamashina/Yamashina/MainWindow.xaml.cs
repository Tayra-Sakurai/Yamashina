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
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Takatsuki.Models;
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
        private ObservableCollection<PageSaveData> pages;
        private bool isBacked = false;

        public MainWindow()
        {
            InitializeComponent();

            pages = [];

            Activated += MainWindow_Activated;
            SuperFrame.Navigating += SuperFrame_Navigated;
            SuperNavigation.BackRequested += SuperNavigation_BackRequested;
            SuperNavigation.ItemInvoked += SuperNavigation_ItemInvoked;
            pages.CollectionChanged += Pages_CollectionChanged;
        }

        private async void Pages_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            List<PageSaveData> data = pages.ToList();
            IStorageItem item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("chachedpages.json");
            if (item == null)
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("cachedpages.json");
            else if (item.IsOfType(StorageItemTypes.Folder))
                return;
            StorageFile file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("cachedpages.json");
            string writingContent = JsonSerializer.Serialize(data);
            await FileIO.WriteTextAsync(file, writingContent);
        }

        private void SuperNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string item = (string)args.InvokedItem;
            ResourceLoader resourceLoader = new();
            if (item == resourceLoader.GetString("BS/Content"))
                SuperFrame.Navigate(typeof(Views.BalanceSheet));
            else if (item == resourceLoader.GetString("CB/Content"))
                SuperFrame.Navigate(typeof(Views.PaymentMethods));
            else if (args.IsSettingsInvoked)
                SuperFrame.Navigate(typeof(Views.BackupPage));
            else if (item == resourceLoader.GetString("Monthly/Content"))
                SuperFrame.Navigate(typeof(Views.MonthlyStat));
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
                ParamType = e.Parameter.GetType().AssemblyQualifiedName,
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
                    SuperFrame.Navigate(typeof(Views.BalanceSheet));
                    return;
                }
                PageSaveData data1 = data.Last();
                Type? pageType = Type.GetType(data1.PageType);
                if (pageType == null)
                {
                    SuperFrame.Navigate(typeof(Views.BalanceSheet));
                    return;
                }
                SuperFrame.Navigate(pageType, data1.Parameters);
                return;
            }
        }

        private async Task<List<PageSaveData>?> LoadJsonDataAsync()
        {
            IStorageItem? item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("cahchedpages.json");
            if (item is null)
                return null;
            else if (item.IsOfType(StorageItemTypes.File))
            {
                StorageFile file = (StorageFile)item;
                string jsonContent = await FileIO.ReadTextAsync(file);
                List<PageSaveData>? data = JsonSerializer.Deserialize<List<PageSaveData>>(jsonContent);
                return data;
            }
            return null;
        }
    }
}
