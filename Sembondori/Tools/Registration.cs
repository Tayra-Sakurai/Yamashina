// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Sembondori.Tools
{
    public class Registration
    {
        /// <summary>
        /// Registers the designated background task
        /// if there is no registered task with the same name.
        /// </summary>
        /// <param name="taskEntryPoint">The entry point name.</param>
        /// <param name="name">The task name.</param>
        /// <param name="backgroundTrigger">The trigger of the task.</param>
        /// <param name="backgroundCondition">The background task activation condition.</param>
        /// <returns>The <see cref="BackgroundTaskRegistration"/> instance of the registered task.</returns>
        public static BackgroundTaskRegistration Register(
            string taskEntryPoint,
            string name,
            IBackgroundTrigger backgroundTrigger,
            IBackgroundCondition backgroundCondition)
        {
            // If there is a registered task,
            // then the function returns its registration.

            // A registered task registration.
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == name)
                    return (BackgroundTaskRegistration)cur.Value;

            // If not, then this registers a task.

            // A Windows App SDK BackgroundTaskBuilder instance.
            // This builds the background task.
            Microsoft.Windows.ApplicationModel.Background.BackgroundTaskBuilder builder = new()
            {
                Name = name
            };
            
            builder.SetTaskEntryPointClsid(new(taskEntryPoint));

            if (backgroundCondition != null)
                builder.AddCondition(backgroundCondition);

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }
    }
}
