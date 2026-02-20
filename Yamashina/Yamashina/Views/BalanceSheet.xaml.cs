// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.Extensions.DependencyInjection;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Takatsuki.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BalanceSheet : Page
    {
        private EntitiesViewModel? viewModel;

        public BalanceSheet()
        {
            InitializeComponent();

            viewModel = App.Current.Service.GetService<EntitiesViewModel>();

            SuperPageCommand.CanExecuteRequested += SuperPageCommand_CanExecuteRequested;
            SuperPageCommand.ExecuteRequested += SuperPageCommand_ExecuteRequested;
        }

        private void SuperPageCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Takatsuki.Models.BalanceSheet balanceSheet)
                Frame.Navigate(typeof(DetailPage), balanceSheet, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void SuperPageCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
        {
            args.CanExecute = args.Parameter is Takatsuki.Models.BalanceSheet;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel != null)
                await viewModel.LoadAsync();

            SuperPageCommand.NotifyCanExecuteChanged();
        }
    }
}
