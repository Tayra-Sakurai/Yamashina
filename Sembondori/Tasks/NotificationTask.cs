// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-Licence-Identifier: GPL-3.0-or-later

using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Sembondori.Tasks
{
    /// <summary>
    /// Implements the notification program from the background.
    /// This task sends reminder of recording.
    /// </summary>
    [Guid("D0CC0FBE-5D77-41BA-865A-5211AA379190")]
    public sealed class NotificationTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public void Run(IBackgroundTaskInstance instance)
        {
            AppNotification appNotification = new AppNotificationBuilder()
                .AddArgument("action", "AppNotificationClick")
                .AddText("Let's make today's record.")
                .BuildNotification();

            AppNotificationManager.Default.Show(appNotification);
        }
    }
}
