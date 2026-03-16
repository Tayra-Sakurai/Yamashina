// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Yamashina
{
    internal class PageSaveData
    {
        private object? parametersProperty;
        public string PageType { get; init; }
        public string? ParamType { get; init; }
        public object? Parameters
        {
            get
            {
                if (ParamType == null)
                    return parametersProperty;
                Type? paramType = Type.GetType(ParamType);
                if (paramType == null) return parametersProperty;
                try
                {
                    if (parametersProperty is JsonElement jsonElement)
                    {
                        return jsonElement.Deserialize(paramType);
                    }
                    return Convert.ChangeType(parametersProperty, paramType);
                }
                catch (InvalidCastException)
                {
                    return parametersProperty;
                }
            }
            init => parametersProperty = value;
        }

        public PageSaveData()
        {
            PageType ??= nameof(BalanceSheet);
        }
    }
}
