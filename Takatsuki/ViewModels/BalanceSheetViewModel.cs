// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Takatsuki.Models;

namespace Takatsuki.ViewModels
{
    public partial class BalanceSheetViewModel : ObservableObject
    {
        private readonly TakatsukiContext context;
        private BalanceSheet model;

        public BalanceSheet Model
        {
            get => model;
        }

        /// <summary>
        /// Gets or sets the value of the date of the item.
        /// </summary>
        public DateTimeOffset Date
        {
            get { return model.DateTime.Date; }
            set
            {
                SetProperty(
                    model.DateTime.Date,
                    value.Date,
                    model,
                    DateTimeSetAction);
            }
        }

        /// <summary>
        /// Gets or sets the time component of the associated date and time value.
        /// </summary>
        public TimeSpan Time
        {
            get => model.DateTime.TimeOfDay;

            set => SetProperty(model.DateTime.TimeOfDay, value, model, DateTimeSetAction);
        }
        

        /// <summary>
        /// Represents the method of the trade.
        /// </summary>
        public PaymentMethod Method
        {
            get => model.Method;
            set => SetProperty(model.Method, context.PaymentMethods.Find(value.Id), model, (m, v) => m.Method = v);
        }

        public string Item
        {
            get => model.ItemName;
            set => SetProperty(model.ItemName, value, model, (m, v) => m.ItemName = v);
        }

        public double Balance
        {
            get => model.Balance;
            set => SetProperty(model.Balance, value, model, (m, v) => m.Balance = v);
        }

        /// <summary>
        /// Gets the full list of the available payment methods.
        /// </summary>
        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get
            {
                return [.. context.PaymentMethods.ToList()];
            }
        }

        public void DateTimeSetAction(BalanceSheet model, DateTime date)
        {
            TimeSpan timeOfDay = model.DateTime.TimeOfDay;
            model.DateTime = date.Date.Add(timeOfDay);
        }

        public void DateTimeSetAction(BalanceSheet model, TimeSpan time)
        {
            DateTime date = model.DateTime.Date;
            model.DateTime = date.Add(time);
        }

        public BalanceSheetViewModel()
        {
            context = new();
            model = new()
            {
                Method = context.PaymentMethods.FirstOrDefault()
            };
        }

        public async Task InitializeForExistingValue(BalanceSheet model)
        {
            this.model = 
                await context
                .BalanceSheet
                .FirstAsync(e => e.Id == model.Id);
            OnPropertyChanged();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
