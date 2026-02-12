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
    public partial class EntitiesViewModel : ObservableObject
    {
        private readonly TakatsukiContext _context;

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

        public EntitiesViewModel()
        {
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
    }
}
