// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Takatsuki.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Yamashina.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonthlyStat : Page
    {
        private readonly StatBalanceSheetsViewModel viewModel;

        public MonthlyStat()
        {
            InitializeComponent();

            viewModel = App.Current.Service.GetRequiredService<StatBalanceSheetsViewModel>();

            SuperListView.PointerEntered += SuperListView_PointerEntered;
            SuperListView.PointerExited += SuperListView_PointerExited;
            MyComboBox.PointerEntered += MyComboBox_PointerEntered;
            MyComboBox.PointerExited += SuperListView_PointerExited;
            SuperMonthPicker.PointerEntered += SuperMonthPicker_PointerEntered;
            SuperMonthPicker.PointerExited += SuperListView_PointerExited;
            SuperCommandButton.PointerEntered += SuperCommandButton_PointerEntered;
            SuperCommandButton.PointerExited += SuperListView_PointerExited;
            SuperSync.PointerEntered += SuperSync_PointerEntered;
            SuperSync.PointerExited += SuperListView_PointerExited;
            SuperListView.ItemClick += SuperListView_ItemClick;
        }

        private async void SuperListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // The invoked item.
            if (e.ClickedItem is not Takatsuki.Models.BalanceSheet balanceSheet)
                return;

            // The system default speech synthesyzer.
            System.Speech.Synthesis.SpeechSynthesizer synth = new();
            synth.SelectVoiceByLanguage(CultureInfo.CurrentCulture);

            // The template file
            StorageFile templateFile = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///Assets/SSMLTemplates/Template_Balance.ssml"));

            // The resource loader.
            // This is necessary to read the titles of the item sections.
            ResourceLoader resourceLoader = new();

            synth.SpeakSsmlAsync(
                string.Format(
                    await FileIO.ReadTextAsync(templateFile),
                    resourceLoader.GetString("DTCol/Content"),
                    balanceSheet.DateTime.ToString("yyyy-MM-dd"),
                    balanceSheet.DateTime.ToString("HH:mm:ss"),
                    resourceLoader.GetString("ItCol/Content"),
                    balanceSheet.ItemName,
                    resourceLoader.GetString("MtdCol/Content"),
                    balanceSheet.Method.Name,
                    resourceLoader.GetString("BlncCol/Content"),
                    balanceSheet.Balance.ToString()));
        }

        private void SuperSync_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayMedia(new("ms-appx:///Assets/Voices/ApplyFilterDesc.wav"));
        }

        private void SuperCommandButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayMedia(new("ms-appx:///Assets/Voices/detailButtondesc.wav"));
        }

        private void SuperMonthPicker_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayMedia(new("ms-appx:///Assets/Voices/MonthDesc.wav"));
        }

        private void MyComboBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayMedia(new("ms-appx:///Assets/Voices/ComboBoxDesc.wav"));
        }

        private void SuperListView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StopPlayingMedia();
        }

        private void SuperListView_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PlayMedia(new("ms-appx:///Assets/Voices/ItemDesc.wav"));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await viewModel.LoadAsync();
        }

        private void PlayMedia(Uri uri)
        {
            SuperMediaPlayerElement.Source = MediaSource.CreateFromUri(uri);
            SuperMediaPlayerElement.MediaPlayer.Play();
        }

        private void StopPlayingMedia()
        {
            SuperMediaPlayerElement.MediaPlayer.Pause();
        }

        private void SuperCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Takatsuki.Models.BalanceSheet)
                args.CanExecute = true;
            else
                args.CanExecute = false;
        }

        private void SuperCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Frame.Navigate(typeof(DetailPage), args.Parameter);
        }
    }
}
