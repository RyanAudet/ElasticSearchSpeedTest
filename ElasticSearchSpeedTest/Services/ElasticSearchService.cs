using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using ElasticSearchSpeedTest.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ElasticSearchSpeedTest.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        ElasticClient elasticClient;
        ILogger _logger;
        static readonly string BookIndex = "book";

        public ElasticSearchService(IConfiguration configuration, ILogger<ElasticSearchService> logger)
        {
            _logger = logger;
            string elasticSearchUrl = configuration["ElasticSearch:Url"] ?? "https://" + Environment.GetEnvironmentVariable("ElasticSearchUrl");

            var regionRegex = new Regex(@"\.([^\.]+)\.es\.amazonaws\.com/?$");
            var matches = regionRegex.Matches(elasticSearchUrl);

            var elasticSearchRegion = matches.Count > 0 ? matches[0].Groups[1].Value : null;

            if (!string.IsNullOrWhiteSpace(elasticSearchUrl) && !string.IsNullOrWhiteSpace(elasticSearchRegion))
            {

                var httpConnection = new AwsHttpConnection(elasticSearchRegion);

                var pool = new SingleNodeConnectionPool(new Uri(elasticSearchUrl));
                var config = new ConnectionSettings(pool, httpConnection)
                    .DisableDirectStreaming() //this line enables error messages from amazon
                    .DefaultMappingFor<Book>(m => m.IndexName(BookIndex));

                elasticClient = new ElasticClient(config);
            }
        }

        public async Task IndexBook(Book book)
        {
            var response = await elasticClient.IndexDocumentAsync(book);
        }

        public async Task DeleteAllBooks()
        {
            await elasticClient.DeleteIndexAsync(BookIndex);
        }

        public async Task<TimedEntity<IEnumerable<Book>>> SearchBooks(string query)
        {
            var authorQuery = new MatchQuery()
            {
                Field = Infer.Field<Book>(f => f.Author),
                Query = query,

            };

            var titleQuery = new MatchQuery()
            {
                Field = Infer.Field<Book>(f => f.Title),
                Query = query,

            };


            Stopwatch st = new Stopwatch();
            st.Start();

            var response = await elasticClient.SearchAsync<Book>(
                x => x.Index(BookIndex)
                .From(0)
                .Size(50)
                .Query(q => q.Bool(b => b.Should(authorQuery, titleQuery)))
                );

            st.Stop();
            _logger.LogCritical("Elastic search call: " + st.ElapsedMilliseconds);

            return new TimedEntity<IEnumerable<Book>> { Entity = response.Documents.ToArray(), MilliSeconds = st.ElapsedMilliseconds };
        }
    }
}
