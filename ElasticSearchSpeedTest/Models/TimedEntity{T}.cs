using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSpeedTest.Models
{
    public class TimedEntity<T>
    {
        public T Entity { get; set; }
        public long MilliSeconds { get; set; }
    }
}
