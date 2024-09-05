using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CashierService.Tests.Cache
{
    public class FakeCacheEntry : ICacheEntry
    {
        public object Key { get; set; }
        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal; 

        public long? Size { get; set; }

        public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = new List<PostEvictionCallbackRegistration>();

        public ICacheEntry RegisterPostEvictionCallback(Action<object, object, EvictionReason> callback, object state)
        {
            return this;
        }

        public ICacheEntry RegisterPostEvictionCallback(Action<object, object, EvictionReason> callback)
        {
            return this;
        }

        public void Dispose(){}
    }

}
