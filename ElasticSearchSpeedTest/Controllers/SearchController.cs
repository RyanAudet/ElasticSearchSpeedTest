using ElasticSearchSpeedTest.Models;
using ElasticSearchSpeedTest.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSpeedTest.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        IElasticSearchService _elasticSearchService;

        public SearchController(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        [HttpGet("{query}")]
        public async Task<TimedEntity<IEnumerable<Book>>> Get(string query)
        {
            return await _elasticSearchService.SearchBooks(query);
        }

        [HttpPost]
        public async Task Post()
        {
            for (int i = 0; i < 100; i++)
            {
                await _elasticSearchService.IndexBook(new Book()
                {
                    Author = "Test" + i,
                    Title = "Title" + i,
                    Description = "This is a test"
                });
            }
        }
    }
}
