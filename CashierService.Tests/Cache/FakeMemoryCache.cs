using Bogus;
using CashierService.Application.Commands;
using CashierService.Application;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace CashierService.Tests.Cache
{
    public class FakeMemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();

        public void Dispose(){}

        public ICacheEntry CreateEntry(object key)
        {
            var cacheEntry = new FakeCacheEntry { Key = key };
            _cache[key] = cacheEntry;
            return cacheEntry;
        }

        public bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }
    }

}
