// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BackupPage : Page
    {
        public BackupPage()
        {
            InitializeComponent();

            VoiceSettingSwitch.Toggled += VoiceSettingSwitch_Toggled;
            ElementSounds.Toggled += ElementSounds_Toggled;
        }

        private void ElementSounds_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["SoundsOfElements"] = ElementSounds.IsOn;

            if (ElementSounds.IsOn)
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            else
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
        }

        private void VoiceSettingSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["VoiceGuideEnabled"] = VoiceSettingSwitch.IsOn;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

            if (settings.Values["VoiceGuideEnabled"] is null)
                settings.Values["VoiceGuideEnabled"] = true;

            if (settings.Values["SoundsOfElements"] is null)
                settings.Values["SoundsOfElements"] = true;

            VoiceSettingSwitch.IsOn = (settings.Values["VoiceGuideEnabled"] as bool?) ?? true;
            ElementSounds.IsOn = (settings.Values["SoundsOfElements"] as bool?) ?? true;
        }
    }
}
