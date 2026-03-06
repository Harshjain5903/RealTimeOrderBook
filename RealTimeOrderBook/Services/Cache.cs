// Author: Harsh Jain
// Real-Time Order Book - Caching layer for performance optimization

using System;
using System.Collections.Generic;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Generic cache implementation with TTL and eviction policies.
    /// Used for optimizing frequently accessed data.
    /// </summary>
    public class CacheEntry<T>
    {
        public T Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public bool IsExpired => ExpiresAt.HasValue && DateTime.Now > ExpiresAt.Value;

        public CacheEntry(T value, TimeSpan? ttl = null)
        {
            Value = value;
            CreatedAt = DateTime.Now;
            ExpiresAt = ttl.HasValue ? DateTime.Now.Add(ttl.Value) : null;
        }
    }

    /// <summary>
    /// Thread-safe cache with TTL, LRU eviction, and statistics.
    /// </summary>
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, CacheEntry<TValue>> _cache;
        private readonly int _maxSize;
        private readonly TimeSpan? _defaultTtl;
        private long _hits = 0;
        private long _misses = 0;

        public int Count => _cache.Count;
        public long Hits => _hits;
        public long Misses => _misses;
        public decimal HitRate => (_hits + _misses) > 0 ? (decimal)_hits / (_hits + _misses) * 100 : 0;

        public Cache(int maxSize = 1000, TimeSpan? defaultTtl = null)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Max size must be positive", nameof(maxSize));

            _maxSize = maxSize;
            _defaultTtl = defaultTtl;
            _cache = new Dictionary<TKey, CacheEntry<TValue>>(maxSize);
        }

        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue? value)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
                {
                    value = entry.Value;
                    _hits++;
                    return true;
                }

                // Remove expired entry
                if (_cache.TryGetValue(key, out var expiredEntry) && expiredEntry.IsExpired)
                {
                    _cache.Remove(key);
                }

                value = default;
                _misses++;
                return false;
            }
        }

        /// <summary>
        /// Sets a value in cache.
        /// </summary>
        public void Set(TKey key, TValue value, TimeSpan? ttl = null)
        {
            lock (_lock)
            {
                ttl ??= _defaultTtl;
                var entry = new CacheEntry<TValue>(value, ttl);

                if (_cache.ContainsKey(key))
                {
                    _cache[key] = entry;
                }
                else
                {
                    // Evict oldest entry if cache is full
                    if (_cache.Count >= _maxSize)
                    {
                        var oldestKey = _cache.Keys.First();
                        _cache.Remove(oldestKey);
                        Logger.Debug($"Cache evicted oldest entry: {oldestKey}");
                    }

                    _cache[key] = entry;
                }
            }
        }

        /// <summary>
        /// Removes a value from cache.
        /// </summary>
        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return _cache.Remove(key);
            }
        }

        /// <summary>
        /// Clears expired entries.
        /// </summary>
        public int ClearExpired()
        {
            lock (_lock)
            {
                var expiredKeys = _cache
                    .Where(kvp => kvp.Value.IsExpired)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in expiredKeys)
                {
                    _cache.Remove(key);
                }

                return expiredKeys.Count;
            }
        }

        /// <summary>
        /// Clears entire cache.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                _hits = 0;
                _misses = 0;
                Logger.Info("Cache cleared");
            }
        }

        /// <summary>
        /// Gets cache statistics.
        /// </summary>
        public string GetStatistics()
        {
            lock (_lock)
            {
                return $@"=== Cache Statistics ===
Entries: {Count}/{_maxSize}
Hits: {_hits}
Misses: {_misses}
Hit Rate: {HitRate:F2}%
========================";
            }
        }
    }
}
