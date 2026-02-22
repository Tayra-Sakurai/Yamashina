// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-Licence-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Sembondori.Tasks
{
    /// <summary>
    /// Implements the notification program from the background.
    /// This task sends reminder of recording.
    /// </summary>
    public sealed class NotificationTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance instance)
        {
            deferral = instance.GetDeferral();

            // TODO: Create background task content.

            deferral.Complete();
        }
    }
}
