// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takatsuki.Contexts;
using Takatsuki.Models;

namespace Takatsuki.Converters
{
    public class MethodConverter : IValueConverter
    {
        private readonly TakatsukiContext takatsukiContext = new();

        /// <summary>
        /// Converts the value to string.
        /// </summary>
        /// <param name="value">The converted value. Must be a <see cref="PaymentMethod"/> instance.</param>
        /// <param name="targetType">The target property's type.</param>
        /// <param name="parameter">The conversion parameters.</param>
        /// <param name="language">The language information.</param>
        /// <returns>The converted string.</returns>
        /// <exception cref="ArgumentException">When the value is not a <see cref="PaymentMethod"/> instance.</exception>
        /// <exception cref="ArgumentNullException">When the value is null.</exception>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is PaymentMethod method)
                return method.Name;
            else
                throw new ArgumentException("\"MethodConverter\" supports only \"PaymentMethod\" type.", nameof(value));
        }

        /// <summary>
        /// Converts the UI property back to the source.
        /// </summary>
        /// <param name="value">The UI property value. Must be a string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameters.</param>
        /// <param name="language">The language information.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">When the value is null.</exception>
        /// <exception cref="ArgumentException">When the value is not valid.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string strValue = (string)value;
                return takatsukiContext.PaymentMethods.First(method => method.Name == strValue);
            }
            catch (InvalidCastException castException)
            {
                throw new ArgumentException("Invalid value.", nameof(value), castException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw new ArgumentNullException("Invalid value.", argumentNullException);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new ArgumentException("Invalid value", nameof(value), invalidOperationException);
            }
        }
    }
}
