// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Takatsuki.ViewModels
{
    public partial class BackupViewModel : ObservableObject
    {
        private readonly TakatsukiContext context;

        public int DataEntries => context.BalanceSheet.ToList().Count;

        [ObservableProperty]
        private int fileSize = 0;

        public BackupViewModel()
        {
            context = new();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task MeasureAsync()
        {
            try
            {
                SqliteConnection connection = new(context.Database.GetConnectionString());
                StorageFile file = await StorageFile.GetFileFromPathAsync(connection.DataSource);
                IBuffer buffer = await FileIO.ReadBufferAsync(file);
                FileSize = (int)(buffer.Length / 1024);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creates a backup of the database.
        /// </summary>
        /// <param name="file">The target database file.</param>
        /// <exception cref="ArgumentException">When <paramref name="file"/> is not writable.</exception>
        /// <exception cref="ArgumentNullException">When the <paramref name="file"/> is null.</exception>
        /// <exception cref="InvalidOperationException">When <paramref name="file"/> is not a database file or when this function fails to get this database.</exception>
        public void Backup(IStorageFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Attributes.HasFlag(FileAttributes.ReadOnly))
                throw new ArgumentException("The file must be writable.", nameof(file));

            if (file.FileType != ".db")
                throw new InvalidOperationException("The file is not a database file.");

            // The backup database.
            using SqliteConnection connectionToTarget = new($"Data Source={file.Path}");

            DbConnection connection = context.Database.GetDbConnection();
            connection.Open();

            if (connection is SqliteConnection sqlite)
                sqlite.BackupDatabase(connectionToTarget);
            else
                throw new InvalidOperationException("Database is not SQLite.");
        }
    }
}
