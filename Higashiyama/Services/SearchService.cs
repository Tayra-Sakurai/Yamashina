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
    public class SearchService : ISearchService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        public SearchService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.embeddingGenerator = embeddingGenerator;
        }

        public async IAsyncEnumerable<float> SearchAsync(string query, string[] docs)
        {
            NDVector<float> queryVector = [.. (await embeddingGenerator.GenerateAsync(query)).Vector.ToArray()];
            foreach (string doc in docs)
            {
                NDVector<float> docVector = [.. (await embeddingGenerator.GenerateAsync(doc)).Vector.ToArray()];
                yield return docVector * queryVector;
            }
        }
    }
}
