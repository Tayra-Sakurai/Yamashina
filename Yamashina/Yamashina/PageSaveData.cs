// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yamashina.Views;

namespace Yamashina
{
    internal class PageSaveData
    {
        public string PageType { get; init; }
        public string? ParamType { get; init; }
        public object? Parameters { get; init; }

        public PageSaveData()
        {
            PageType ??= nameof(BalanceSheet);
        }
    }

    internal class PageSaveData<TParameter>
    {
        public string PageType { get; init; }
        public string? ParamType { get; init; }
        public required TParameter Parameters { get; init; }

        public PageSaveData()
        {
            PageType ??= nameof(BalanceSheet);
        }
    }
}
