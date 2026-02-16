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
    public partial class PaymentMethodsViewModel : ObservableObject
    {
        private readonly TakatsukiContext context;

        [ObservableProperty]
        private ObservableCollection<PaymentMethodItemViewModel> viewModels;

        public PaymentMethodsViewModel()
        {
            context = new();
            if (context.Database.EnsureCreated())
            {
                Debug.Fail("Invalid action.");
                PaymentMethod method = new()
                {
                    Name = "現金"
                };
                context.Add(method);
                context.SaveChanges();
            }
            ViewModels = [];
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            await context.SaveChangesAsync();
            ViewModels.Clear();
            await foreach (
                var method in
                context.PaymentMethods
                .Include(e => e.Entries)
                .AsAsyncEnumerable())
                ViewModels.Add(new() { PaymentMethod = method });
        }

        [RelayCommand]
        public void Add()
        {
            PaymentMethod method = new()
            {
                Name = string.Empty
            };
            method = context.Add(method).Entity;
            ViewModels.Add(
                new()
                {
                    PaymentMethod = method
                }
            );
        }

        [RelayCommand(CanExecute = nameof(CanRemove))]
        public void Remove(PaymentMethodItemViewModel item)
        {
            ViewModels.Remove(item);
            var entity = context.Attach(item.PaymentMethod).Entity;
            context.Remove(entity);
        }

        public static bool CanRemove(PaymentMethodItemViewModel item)
        {
            if (item == null) return false;
            return true;
        }
    }
}
