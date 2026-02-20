// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Higashiyama.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Returns how close the query and the documents.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="docs">The documents.</param>
        /// <returns>The match rates.</returns>
        IAsyncEnumerable<float> SearchAsync(string query, string[] docs);
    }
}
