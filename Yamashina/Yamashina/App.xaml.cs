using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Takatsuki.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public IServiceProvider Service { get; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Service = ConfigureServices();
            InitializeComponent();
        }

        /// <summary>
        /// Configures and builds the application's service provider with required dependencies.
        /// </summary>
        /// <remarks>Registers view model types with transient lifetimes. Use the returned service
        /// provider to resolve application services as needed.</remarks>
        /// <returns>An <see cref="IServiceProvider"/> instance containing the registered services.</returns>
        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();

            services.AddTransient<BalanceSheetViewModel>();
            services.AddTransient<EntitiesViewModel>();
            services.AddTransient<PaymentMethodsViewModel>();
            services.AddTransient<PaymentMethodViewModel>();
            services.AddTransient<StatBalanceSheetsViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        public static new App Current => (App)Application.Current;
    }
}
