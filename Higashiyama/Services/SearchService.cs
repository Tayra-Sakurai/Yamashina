// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Higashiyama.Specials;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace Higashiyama.Services
{
    /// <summary>
    /// The class implementing the <see cref="ISearchService"/>.
    /// </summary>
    public class SearchService : ISearchService
    {
        /// <summary>
        /// The <see cref="IEmbeddingGenerator"/> instance.
        /// </summary>
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="embeddingGenerator">Service handled <see cref="IEmbeddingGenerator{string, Embedding{float}}"/> instance.</param>
        public SearchService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.embeddingGenerator = embeddingGenerator;
        }

        public async IAsyncEnumerable<float> SearchAsync(string query, string[] docs)
        {
            // The query specifier.
            AdditionalPropertiesDictionary queryOptionsAdditional = new();
            queryOptionsAdditional["task_type"] = "retrieval_query";

            // The option object.
            EmbeddingGenerationOptions optionsQuery = new()
            {
                AdditionalProperties = queryOptionsAdditional
            };

            // The oprions for searched words.
            EmbeddingGenerationOptions optionsDoc = new()
            {
                AdditionalProperties = new()
                {
                    { "task_type", "classification" }
                }
            };

            // The query vector.
            NDVector<float> queryVector = [.. (await embeddingGenerator.GenerateAsync(query, optionsQuery)).Vector.ToArray()];

            // The document.
            foreach (string doc in docs)
            {
                // The document vector.
                NDVector<float> docVector = [.. (await embeddingGenerator.GenerateAsync(doc, optionsDoc)).Vector.ToArray()];
                yield return docVector * queryVector;
            }
        }
    }
}
