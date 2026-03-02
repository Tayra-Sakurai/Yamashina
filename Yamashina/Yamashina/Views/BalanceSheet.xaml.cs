// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using Takatsuki.ViewModels;
using Windows.Storage;

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

            SuperListView.ItemClick += SuperListView_ItemClick;
        }

        private async void SuperListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            SpeechSynthesizer synth = new SpeechSynthesizer();
            Takatsuki.Models.BalanceSheet? sheet = e.ClickedItem as Takatsuki.Models.BalanceSheet;

            if (sheet == null)
                return;

            StorageFile ssmlTemplate = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SSMLTemplates/Template_Balance.ssml"));
            string template = await FileIO.ReadTextAsync(ssmlTemplate);
            ResourceLoader resourceLoader = new();

            string ssmltext =
                string.Format(
                    template,
                    resourceLoader.GetString("DTCol/Content"),
                    sheet.DateTime.ToString("yyyy-MM-dd"),
                    sheet.DateTime.ToString("HH:mm:ss"),
                    resourceLoader.GetString("ItCol/Content"),
                    sheet.ItemName,
                    resourceLoader.GetString("MtdCol/Content"),
                    sheet.Method.Name,
                    resourceLoader.GetString("BlncCol/Content"),
                    sheet.Balance.ToString());

            synth.SpeakSsml(ssmltext);
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
