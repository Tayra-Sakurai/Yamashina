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
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Takatsuki.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
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

            // SuperListView.ItemClick += SuperListView_ItemClick;
        }

        private async void SuperListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpeechSynthesizer speechSynthesizer = new();

            VoiceInformation voice =
                (
                    from VoiceInformation v in SpeechSynthesizer.AllVoices
                    where v.Language == CultureInfo.CurrentCulture.Name
                    where v.Gender == VoiceGender.Female
                    select v
                ).FirstOrDefault() ?? SpeechSynthesizer.DefaultVoice;

            speechSynthesizer.Voice = voice;

            Uri uri = new("ms-appx:///Assets/SSMLTemplates/Template_Balance.ssml");
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);

            ResourceLoader resourceLoader = new();

            using (Stream stream = await storageFile.OpenStreamForReadAsync())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string ssmlstr = reader.ReadToEnd();

                if (SuperListView.SelectedItem is Takatsuki.Models.BalanceSheet item)
                {
                    string[] strings =
                        [
                            resourceLoader.GetString("DTCol/Content"),
                                item.DateTime.ToString("yyyy-MM-dd"),
                                item.DateTime.ToString("HH:mm"),
                                resourceLoader.GetString("ItCol/Content"),
                                item.ItemName
                        ];

                    ssmlstr = string.Format(ssmlstr, strings);
                    using (SpeechSynthesisStream synthesisStream = await speechSynthesizer.SynthesizeSsmlToStreamAsync(ssmlstr))
                    {
                        MediaPlayer mediaPlayer = new()
                        {
                            Source = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType)
                        };
                        mediaPlayer.Play();
                    }
                }
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
