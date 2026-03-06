// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
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
        private readonly List<Type> pages;
        private Type? currentPage = null;

        public MainWindow()
        {
            InitializeComponent();

            pages = [typeof(Views.BalanceSheet)];

            Activated += MainWindow_Activated;
            SuperFrame.Navigating += SuperFrame_Navigated;
            SuperNavigation.BackRequested += SuperNavigation_BackRequested;
            SuperNavigation.ItemInvoked += SuperNavigation_ItemInvoked;
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
            if (pages.Count > 0)
            {
                Type pageType = pages.Last();
                SuperFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                pages.RemoveAt(pages.Count - 1);
                if (pageType == typeof(Views.PaymentMethods))
                    CurrentBalanceItem.IsSelected = true;
                else if (pageType == typeof(Views.BalanceSheet))
                    BalanceSheetItem.IsSelected = true;
                else if (pageType == typeof(Views.BackupPage))
                    sender.SelectedItem = sender.SettingsItem;
                else if (pageType == typeof(Views.MonthlyStat))
                    MonthlyPageItem.IsSelected = true;
            }
            else
                SuperNavigation.IsBackEnabled = false;
        }

        private async void SuperFrame_Navigated(object sender, NavigatingCancelEventArgs e)
        {
            if (currentPage != null)
            {
                pages.Add(currentPage);
                if (!SuperNavigation.IsBackEnabled)
                    SuperNavigation.IsBackEnabled = true;
            }
            currentPage = e.SourcePageType;

            string? pageType = e.SourcePageType.AssemblyQualifiedName;
            object? param = e.Parameter;
            string? paramType = null;

            if (pageType == null)
                return;

            if (param is BalanceSheet p)
            {
                paramType = typeof(BalanceSheet).AssemblyQualifiedName;
                using (TakatsukiContext context = new())
                {
                    param = context.BalanceSheet.Find(p.Id);
                }
            }
            else if (param is PaymentMethod p1)
            {
                paramType = typeof(PaymentMethod).AssemblyQualifiedName;
                using (TakatsukiContext ctx = new())
                {
                    param = ctx.PaymentMethods.Find(p1.Id);
                }
            }
            else if (param is not null)
            {
                paramType = param.GetType().AssemblyQualifiedName;
            }

            PageSaveData pageSaveData = new PageSaveData
            {
                PageType = pageType,
                Parameters = param,
                ParamType = paramType,
            };

            IStorageItem? item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("cachedpage.json");

            if (item != null)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    StorageFile file = (StorageFile)item;

                    await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(pageSaveData));
                }

                return;
            }

            StorageFile storageFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("cachedpage.json");
            await FileIO.WriteTextAsync(storageFile, JsonSerializer.Serialize(pageSaveData));
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (SuperFrame.Content == null)
            {
                StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;
                IStorageItem? storageItem = await localCache.TryGetItemAsync("cachedpage.json");
                if (storageItem.IsOfType(StorageItemTypes.File))
                {
                    StorageFile storageFile = (StorageFile)storageItem;

                    // JSON encoded object value.
                    string content = await FileIO.ReadTextAsync(storageFile);

                    // The first data value.
                    if (JsonSerializer.Deserialize<PageSaveData>(content) is not PageSaveData saveData)
                    {
                        SuperFrame.Navigate(typeof(Views.BalanceSheet));
                        return;
                    }

                    // The navigating page type.
                    if (Type.GetType(saveData.PageType) is not Type pageType)
                    {
                        SuperFrame.Navigate(typeof(Views.BalanceSheet));
                        return;
                    }

                    // The parameter type's full name.
                    if (saveData.ParamType is not string paramType)
                    {
                        SuperFrame.Navigate(pageType);
                        return;
                    }

                    // The parameter's type.
                    if (Type.GetType(paramType) is not Type pageParamType)
                    {
                        SuperFrame.Navigate(pageType);
                        return;
                    }

                    // Navigate to the cached page.
                    if (JsonSerializer.Deserialize(content, typeof(PageSaveData<>).MakeGenericType(pageParamType)) is not object completeContent)
                    {
                        Debug.Fail("Deserialization failed.");
                        SuperFrame.Navigate(pageType);
                        return;
                    }

                    // The parameter information.
                    if (completeContent.GetType().GetProperty("Parameters") is not PropertyInfo paramInfo)
                    {
                        Debug.Fail("\"Parameters\" not found.");
                        SuperFrame.Navigate(pageType);
                        return;
                    }

                    object? parameters = paramInfo.GetValue(completeContent);

                    if (parameters != null)
                    {
                        Debug.WriteLine("Successfully loaded");
                        SuperFrame.Navigate(pageType, parameters);
                        return;
                    }

                    Debug.Fail(paramType);
                    SuperFrame.Navigate(pageType);
                }
            }
        }
    }
}
