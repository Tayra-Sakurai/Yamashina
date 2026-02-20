// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takatsuki.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BalanceSheet> Entries { get; } = new List<BalanceSheet>();

        public override string ToString()
        {
            return $"{Name} ({Entries.Count})";
        }
    }
}
