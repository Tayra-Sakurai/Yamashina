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
    public sealed partial class PaymentMethods : Page
    {
        private readonly PaymentMethodsViewModel? viewModel;

        public PaymentMethods()
        {
            InitializeComponent();

            viewModel = App.Current.Service.GetService<PaymentMethodsViewModel>();

            SuperPageCmd.ExecuteRequested += SuperPageCmd_ExecuteRequested;
            SuperPageCmd.CanExecuteRequested += SuperPageCmd_CanExecuteRequested;
        }

        private void SuperPageCmd_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
        {
            args.CanExecute = args.Parameter is PaymentMethodItemViewModel;
        }

        private void SuperPageCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PaymentMethodItemViewModel model)
                Frame.Navigate(typeof(PaymentDetail), model.PaymentMethod, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel != null)
                await viewModel.LoadAsync();

            SuperPageCmd.NotifyCanExecuteChanged();
        }
    }
}
