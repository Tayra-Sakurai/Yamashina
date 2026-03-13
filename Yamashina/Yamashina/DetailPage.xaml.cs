// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Takatsuki.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Yamashina
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        private BalanceSheetViewModel? viewModel;

        public DetailPage()
        {
            InitializeComponent();

            viewModel = App.Current.Service.GetService<BalanceSheetViewModel>();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel != null)
            {
                if (e.Parameter is Takatsuki.Models.BalanceSheet bs)
                    await viewModel.InitializeForExistingValue(bs);
                else if (e.Parameter is not null)
                {
                    PropertyInfo? propertyInfo = e.GetType().GetProperty("Id");
                    if (propertyInfo is not null)
                    {
                        Type propertyType = propertyInfo.PropertyType;
                        if (propertyType == typeof(int))
                            await viewModel.InitializeForExistingValue((int)propertyInfo.GetValue(e.Parameter)!);
                    }
                }
            }
            Debug.WriteLine("This page is DetailPage.");
            Debug.WriteLine(e.Parameter?.GetType().FullName);
        }
    }
}
