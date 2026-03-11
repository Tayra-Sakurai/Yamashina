// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Higashiyama.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.UI.Xaml.Automation;
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
    public partial class EntitiesViewModel : ObservableObject
    {
        private readonly TakatsukiContext _context;
        private readonly ISearchService search;

        private PaymentMethod paymentMethod;

        [ObservableProperty]
        private ObservableCollection<BalanceSheet> balanceSheets;
        [ObservableProperty]
        private ObservableCollection<PaymentMethod> paymentMethods;

        public PaymentMethod Filtered
        {
            get => paymentMethod;
            set => SetProperty(ref paymentMethod, value);
        }

        public EntitiesViewModel(ISearchService search)
        {
            this.search = search;
            _context = new();
            if (_context.Database.EnsureCreated())
            {
                PaymentMethod paymentMethod = new()
                {
                    Name = "現金"
                };
                _context.Add(paymentMethod);
                _context.SaveChanges();
            }
            BalanceSheets = [];
            _context.BalanceSheet.Load();
            _context.PaymentMethods.Load();
            PaymentMethods = _context.PaymentMethods.Local.ToObservableCollection();
            Filtered = _context.PaymentMethods.First();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            await _context.SaveChangesAsync();
            BalanceSheets.Clear();
            List<BalanceSheet> balanceSheets = [.. _context.BalanceSheet.Local ];
            balanceSheets.Sort();
            foreach (var sheet in balanceSheets)
                BalanceSheets.Add(sheet);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        [RelayCommand(CanExecute = nameof(CanRemove))]
        public void Remove(BalanceSheet balanceSheet)
        {
            _context.Remove(balanceSheet);
            BalanceSheets.Remove(balanceSheet);
        }

        public bool CanRemove(BalanceSheet balanceSheet)
        {
            if (balanceSheet == null) return false;
            EntityEntry<BalanceSheet> entry = _context.Entry(balanceSheet);
            return entry.State != EntityState.Deleted && entry.State != EntityState.Detached;
        }

        [RelayCommand]
        public void Add()
        {
            BalanceSheet entity = new()
            {
                Method = _context.PaymentMethods.FirstOrDefault(),
            };
            entity = _context.Add(entity).Entity;
            BalanceSheets.Add(entity);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task Filter()
        {
            BalanceSheets.Clear();

            PaymentMethod paymentMethod =
                await _context
                .PaymentMethods
                .Include(e => e.Entries)
                .FirstAsync(e => e.Id == Filtered.Id);

            List<BalanceSheet> balanceSheets = paymentMethod.Entries.ToList();
            balanceSheets.Sort();

            foreach (var entry in balanceSheets)
                BalanceSheets.Add(entry);
        }

        /// <summary>
        /// Searches the balance sheet queries about the item name.
        /// </summary>
        /// <param name="query">Search Query.</param>
        /// <returns>The task.</returns>
        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearchExecute))]
        public async Task SearchAsync(string query)
        {
            // The item names.
            IEnumerable<string> strings = from BalanceSheet b in BalanceSheets select b.ItemName;

            // The item names as array.
            string[] docs = strings.ToArray();

            // Reorder the items.

            // Copy of the entries.
            BalanceSheet[] copiedEntries = new BalanceSheet[BalanceSheets.Count];
            // The index
            int index = 0;

            BalanceSheets.CopyTo(copiedEntries, 0);

            BalanceSheets.Clear();

            // The match rates.
            List<float> matchRates = [];

            // The matching rate.
            await foreach (
                float matchRate
                in search.SearchAsync(query, docs))
            {
                matchRates.Add(matchRate);
                matchRates.Sort();
                matchRates.Reverse();

                BalanceSheets.Insert(matchRates.IndexOf(matchRate), copiedEntries[index++]);
            }
        }

        /// <summary>
        /// Judges if the search is executable.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>Returns <see cref="true"/> if the query is valid; otherwise returns <see cref="false"/>.</returns>
        public bool CanSearchExecute(string query)
        {
            return !string.IsNullOrEmpty(query);
        }
    }
}
