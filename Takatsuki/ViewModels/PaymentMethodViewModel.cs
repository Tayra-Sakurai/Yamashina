// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Takatsuki.Models;

namespace Takatsuki.ViewModels
{
    public partial class PaymentMethodViewModel : ObservableObject
    {
        private readonly TakatsukiContext context;
        private PaymentMethod method;

        public string Name
        {
            get => method.Name;
            set => SetProperty(method.Name, value, method, (m, v) => m.Name = v);
        }

        public double CurrentBalance
        {
            get
            {
                double result = 0;
                foreach (var item in method.Entries)
                {
                    result += item.Balance;
                }
                return result;
            }
        }

        public PaymentMethodViewModel()
        {
            context = new();
            method = context.PaymentMethods.FirstOrDefault();
        }

        public void InitializeForExistingValue(PaymentMethod method)
        {
            this.method = method;
            OnPropertyChanged();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            context.Update(method);
            await context.SaveChangesAsync();
        }
    }
}
