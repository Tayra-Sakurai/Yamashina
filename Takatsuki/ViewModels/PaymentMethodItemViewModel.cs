using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Models;

namespace Takatsuki.ViewModels
{
    public class PaymentMethodItemViewModel
    {
        public PaymentMethod PaymentMethod { get; init; }
        public string Name
        {
            get => PaymentMethod.Name;
        }
        public double CurrentBalance
        {
            get
            {
                double balance = 0;
                if (PaymentMethod.Entries != null)
                {
                    foreach (var item in PaymentMethod.Entries)
                    {
                        balance += item.Balance;
                        Debug.WriteLine(item.ItemName);
                    }
                }
                return balance;
            }
        }
    }
}
