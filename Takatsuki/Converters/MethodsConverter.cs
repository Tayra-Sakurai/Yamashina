// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Models;

namespace Takatsuki.Converters
{
    public class MethodsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is ICollection<PaymentMethod> paymentMethods)
            {
                List<string> strings =
                    (from method in paymentMethods
                     select method.Name).ToList();

                return new ObservableCollection<string>(strings);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
