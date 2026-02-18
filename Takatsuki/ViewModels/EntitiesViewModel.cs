using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Higashiyama.Services;
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
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            await _context.SaveChangesAsync();
            BalanceSheets.Clear();
            List<BalanceSheet> balanceSheets = [.. _context.BalanceSheet.Local ];
            balanceSheets.Sort();
            foreach (var sheet in balanceSheets)
                BalanceSheets.Add(sheet);
        }

        [RelayCommand]
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

        [RelayCommand]
        public void Filter()
        {
            BalanceSheets.Clear();
            foreach(var item in _context.BalanceSheet)
            {
                if (item.Method == Filtered)
                    BalanceSheets.Add(item);
            }
        }

        /// <summary>
        /// Searches the balance sheet queries about the item name.
        /// </summary>
        /// <param name="query">Search Query.</param>
        /// <returns>The task.</returns>
        [RelayCommand(CanExecute = nameof(CanSearchExecute))]
        public async Task SearchAsync(string query)
        {
            // The item names.
            IEnumerable<string> strings = from BalanceSheet b in BalanceSheets select b.ItemName;

            // The item names as array.
            string[] docs = strings.ToArray();

            // The index.
            int i = 0;

            // The matched query data list.
            List<BalanceSheet> list = [];

            await foreach (var item in search.SearchAsync(query, docs))
                if (item >= 0.625)
                    list.Add(BalanceSheets[i++]);

            BalanceSheets.Clear();

            foreach (var i2 in list)
                BalanceSheets.Add(i2);
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
