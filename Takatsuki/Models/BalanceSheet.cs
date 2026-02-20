// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takatsuki.Models
{
    public class BalanceSheet : IComparable<BalanceSheet>
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string ItemName { get; set; } = string.Empty;
        public int MethodId { get; set; }
        public double Balance { get; set; } = 0;
        public PaymentMethod Method { get; set; } = null!;

        public int CompareTo (BalanceSheet other)
        {
            int dateTimeComparison = DateTime.CompareTo(other.DateTime);
            if (dateTimeComparison != 0) return dateTimeComparison;
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return $"{Id}: {ItemName}, {MethodId}";
        }
    }
}
