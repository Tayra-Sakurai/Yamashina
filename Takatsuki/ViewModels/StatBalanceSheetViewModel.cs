// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Takatsuki.Models;

namespace Takatsuki.ViewModels
{
    public partial class StatBalanceSheetsViewModel : ObservableObject
    {
        private TakatsukiContext context;

        [ObservableProperty]
        private ObservableCollection<BalanceSheet> balanceSheets;

        [ObservableProperty]
        private DateTimeOffset month;

        [ObservableProperty]
        private ObservableCollection<PaymentMethod> methods;

        [ObservableProperty]
        private PaymentMethod method;

        public StatBalanceSheetsViewModel()
        {
            context = new();
            BalanceSheets = [];
            Month = DateTimeOffset.Now;

            Methods = [];

            foreach (PaymentMethod paymentMethod in context.PaymentMethods)
                Methods.Add(paymentMethod);

            Method = Methods.First();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            BalanceSheets.Clear();

            await context.SaveChangesAsync();

            PaymentMethod payment =
                await context.PaymentMethods.FindAsync(Method.Id);
            List<BalanceSheet> balanceSheets1 =
                await context.Entry(payment)
                .Collection(e => e.Entries)
                .Query()
                .ToListAsync();

            balanceSheets1.Sort();

            foreach (BalanceSheet balanceSheet in balanceSheets1)
                if (balanceSheet.DateTime.Year == Month.Year && balanceSheet.DateTime.Month == Month.Month)
                    BalanceSheets.Add(balanceSheet);
        }
    }
}
