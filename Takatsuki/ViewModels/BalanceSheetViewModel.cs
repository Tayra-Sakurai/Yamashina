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
        private TakatsukiContext context;
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
        public string Method
        {
            get { return model.Method.Name; }
            set
            {
                PaymentMethod method = model.Method;
                PaymentMethod entity = context.PaymentMethods.FirstOrDefault(
                    e => e.Name == value);
                if (entity  != null)
                    method = entity;
                SetProperty(
                    model.Method,
                    method,
                    model,
                    (m, v) => m.Method = v);
            }
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
        public ObservableCollection<string> PaymentMethods
        {
            get
            {
                ObservableCollection<string> strings = [];
                foreach (var method in context.PaymentMethods)
                    strings.Add(method.Name);
                return strings;
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

        public BalanceSheetViewModel(BalanceSheet model)
            : this()
        {
            InitializeForExistingValue(model);
        }

        public void InitializeForExistingValue(BalanceSheet model)
        {
            this.model = model;
            OnPropertyChanged();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
            context = new();
            if (context.Entry(model).State == EntityState.Detached)
                context.Update(model);
            await context.SaveChangesAsync();
        }
    }
}
