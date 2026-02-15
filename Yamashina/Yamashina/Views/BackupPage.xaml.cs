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
        private readonly string dbPath;
        private readonly SqliteConnection connection;

        public BackupPage()
        {
            InitializeComponent();

            var folder = ApplicationData.Current.LocalFolder;
            dbPath = Path.Combine(folder.Path, "Takatsuki.db");

            connection = new($"Data Source={dbPath}");
            SuperBackupBtn.Click += SuperBackupBtn_Click;
        }

        /// <summary>
        /// Saves a backup.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        private async void SuperBackupBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button)
            {
                button.IsEnabled = false;

                FileSavePicker picker = new(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

                ResourceLoader resourceLoader = new();

                picker.FileTypeChoices.Add(resourceLoader.GetString("DBFile"), new List<string>() { ".db" });

                picker.DefaultFileExtension = ".db";

                picker.SuggestedFileName = "Database_backup";

                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                picker.SuggestedFolder = "";

                picker.CommitButtonText = resourceLoader.GetString("Sv/Label");

                var result = await picker.PickSaveFileAsync();

                if (result != null)
                {
                    string path = result.Path;
                    SqliteConnection backupConnection = new($"Data Source={path}");

                    connection.Open();

                    connection.BackupDatabase(backupConnection);

                    MessageTextBlock.Text = path;
                }

                button.IsEnabled = true;
            }
        }
    }
}
