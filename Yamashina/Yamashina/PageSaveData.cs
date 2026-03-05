using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yamashina.Views;

namespace Yamashina
{
    internal readonly struct PageSaveData
    {
        public string PageType { get; init; }
        public string? ParamType { get; init; }
        public object? Parameters { get; init; }

        public PageSaveData()
        {
            if (PageType == null)
                PageType = nameof(BalanceSheet);
        }
    }
}
