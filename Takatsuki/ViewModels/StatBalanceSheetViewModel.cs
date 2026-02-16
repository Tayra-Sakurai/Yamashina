using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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
        private ObservableCollection<string> methodNames;

        [ObservableProperty]
        private string methodName;

        public StatBalanceSheetsViewModel()
        {
            context = new();
            BalanceSheets = [];
            Month = DateTimeOffset.Now;
            MethodNames = [];

            foreach (var method in context.PaymentMethods)
                methodNames.Add(method.Name);

            MethodName = MethodNames.First();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            BalanceSheets.Clear();

            await context.SaveChangesAsync();

            context = new();
            await context.BalanceSheet.LoadAsync();
            await context.PaymentMethods.LoadAsync();

            List<BalanceSheet> sheets = [.. context.BalanceSheet.Local ];
            sheets.Sort();

            foreach (var sheet in sheets)
                if (sheet.DateTime.Month == Month.Month && sheet.DateTime.Year == Month.Year && sheet.Method.Name == MethodName)
                    BalanceSheets.Add(sheet);
        }
    }
}
