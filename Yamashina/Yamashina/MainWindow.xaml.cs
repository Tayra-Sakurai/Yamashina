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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yamashina.Views;

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

            pages = [typeof(BalanceSheet)];

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
                SuperFrame.Navigate(typeof(BalanceSheet));
            else if (item == resourceLoader.GetString("CB/Content"))
                SuperFrame.Navigate(typeof(PaymentMethods));
            else if (item == resourceLoader.GetString("BU/Content"))
                SuperFrame.Navigate(typeof(BackupPage));
            else if (item == resourceLoader.GetString("Monthly/Content"))
                SuperFrame.Navigate(typeof(MonthlyStat));
        }

        private void SuperNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (pages.Count > 0)
            {
                Type pageType = pages.Last();
                SuperFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                pages.RemoveAt(pages.Count - 1);
                if (pageType == typeof(PaymentMethods))
                    CurrentBalanceItem.IsSelected = true;
                else if (pageType == typeof(BalanceSheet))
                    BalanceSheetItem.IsSelected = true;
                else if (pageType == typeof(BackupPage))
                    BackupPageItem.IsSelected = true;
                else if (pageType == typeof(MonthlyStat))
                    MonthlyPageItem.IsSelected = true;
            }
            else
                SuperNavigation.IsBackEnabled = false;
        }

        private void SuperFrame_Navigated(object sender, NavigatingCancelEventArgs e)
        {
            if (currentPage != null)
            {
                pages.Add(currentPage);
                if (!SuperNavigation.IsBackEnabled)
                    SuperNavigation.IsBackEnabled = true;
            }
            currentPage = e.SourcePageType;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            SuperFrame.Navigate(typeof(BalanceSheet));
        }
    }
}
