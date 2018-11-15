using ElasticSearchSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSpeedTest.Services
{
    public interface IElasticSearchService
    {
        Task<TimedEntity<IEnumerable<Book>>> SearchBooks(string query);
        Task IndexBook(Book book);
    }
}
