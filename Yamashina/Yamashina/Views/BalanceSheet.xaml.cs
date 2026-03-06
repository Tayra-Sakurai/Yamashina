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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using Takatsuki.ViewModels;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Yamashina.Extensions;

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
        private MediaPlayer? mediaPlayer;

        public BalanceSheet()
        {
            InitializeComponent();

            viewModel = App.Current.Service.GetService<EntitiesViewModel>();

            SuperPageCommand.CanExecuteRequested += SuperPageCommand_CanExecuteRequested;
            SuperPageCommand.ExecuteRequested += SuperPageCommand_ExecuteRequested;

            SuperListView.ItemClick += SuperListView_ItemClick;

            AddButton.PointerEntered += AddButton_PointerEntered;
            RemoveButton.PointerEntered += RemoveButton_PointerEntered;
            DetailButton.PointerEntered += DetailButton_PointerEntered;
            FilterButton.PointerEntered += FilterButton_PointerEntered;
            SyncButton.PointerEntered += SyncButton_PointerEntered;
            SuperSearchButton.PointerEntered += SuperSearchButton_PointerEntered;
            SaveButton.PointerEntered += SaveButton_PointerEntered;
            MethodComboBox.PointerEntered += SuperComboBox_PointerEntered;
            SuperSearchBox.PointerEntered += SuperSearchBox_PointerEntered;
            SuperListView.PointerEntered += SuperListView_PointerEntered;

            AddButton.PointerExited += PointerExitedAction;
            RemoveButton.PointerExited += PointerExitedAction;
            DetailButton.PointerExited += PointerExitedAction;
            FilterButton.PointerExited += PointerExitedAction;
            SyncButton.PointerExited += PointerExitedAction;
            SuperSearchButton.PointerExited += PointerExitedAction;
            SaveButton.PointerExited += PointerExitedAction;
            MethodComboBox.PointerExited += PointerExitedAction;
            SuperSearchBox.PointerExited += PointerExitedAction;
            SuperListView.PointerExited += PointerExitedAction;

            MethodComboBox.SelectionChanged += SuperComboBox_SelectionChanged;
        }

        private void SuperListView_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/ItemDesc.wav"));
        }

        private async void SuperComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            SpeechSynthesizer synth = new();
            StorageFile ssmlTemplate = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SSMLTemplates/Template_Method.ssml"));

            Takatsuki.Models.PaymentMethod? method = MethodComboBox.SelectedItem as Takatsuki.Models.PaymentMethod;
            if (method == null) return;

            synth.SelectVoiceByLanguage(CultureInfo.CurrentCulture.Name);

            synth.SpeakSsmlAsync(
                string.Format(
                    await FileIO.ReadTextAsync(ssmlTemplate),
                    method.Name));
        }

        private void SuperSearchBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/searchBoxDesc.wav"));
        }

        private void SuperComboBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/ComboBoxDesc.wav"));
        }

        private void SaveButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/saveDesc.wav"));
        }

        private void PointerExitedAction(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            StopPlay();
        }

        private void SuperSearchButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/searchDesc.wav"));
        }

        private void SyncButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new("ms-appx:///Assets/Voices/syncDesc.wav"));
        }

        private void FilterButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new Uri("ms-appx:///Assets/Voices/filterDesc.wav"));
        }

        private void DetailButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new Uri("ms-appx:///Assets/Voices/detailButtonDesc.wav"));
        }

        private void RemoveButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new Uri("ms-appx:///Assets/Voices/removeDesc.wav"));
        }

        private void AddButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            PlayMedia(new Uri("ms-appx:///Assets/Voices/addDesc.wav"));
        }

        private async void SuperListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] is not true)
                return;

            StopPlay();

            SpeechSynthesizer synth = new SpeechSynthesizer();

            synth.SelectVoiceByLanguage(CultureInfo.CurrentCulture);

            if (e.ClickedItem is not Takatsuki.Models.BalanceSheet sheet)
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

            synth.SpeakSsmlAsync(ssmltext);
        }

        private void PlayMedia(Uri uri)
        {
            mediaPlayer = new()
            {
                Source = MediaSource.CreateFromUri(uri)
            };

            mediaPlayer.Play();
        }

        private void StopPlay()
        {
            if (mediaPlayer != null)
            {

                try
                {
                    MediaPlaybackSession session = mediaPlayer.PlaybackSession;
                    bool canPause = session.CanPause;
                    if (canPause)
                        mediaPlayer.Pause();
                }
                catch (COMException comException)
                {
                    Debug.WriteLine("COMException: " + comException.Message);
                    throw new COMException(comException.Message, comException);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw new Exception(ex.Message, ex);
                }
                mediaPlayer.Dispose();
            }
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
