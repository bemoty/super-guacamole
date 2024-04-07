namespace super.guacamole.image.Cache;

public class LruMemoryAsyncCache<TK, TV>(int capacity, IProvider<TK, TV> provider)
    : IAsyncCache<TK, TV> where TK : notnull
{
    private readonly Dictionary<TK, LinkedListNode<LruCacheItem<TK, TV>>> _cacheMap = new();
    private readonly LinkedList<LruCacheItem<TK, TV>> _lruList = [];

    public async Task<TV> Get(TK key)
    {
        if (_cacheMap.TryGetValue(key, out var node))
        {
            var value = node.Value.Value;
            _lruList.Remove(node);
            _lruList.AddLast(node);
            return value;
        }

        var providedValue = await provider.Provide(key);
        Put(key, providedValue);
        return providedValue;
    }

    public void Put(TK key, TV value)
    {
        if (_cacheMap.TryGetValue(key, out var existingNode))
        {
            _lruList.Remove(existingNode);
        }
        else if (_cacheMap.Count >= capacity)
        {
            RemoveFirst();
        }

        LruCacheItem<TK, TV> cacheItem = new LruCacheItem<TK, TV>(key, value);
        LinkedListNode<LruCacheItem<TK, TV>> node = new LinkedListNode<LruCacheItem<TK, TV>>(cacheItem);
        _lruList.AddLast(node);
        _cacheMap[key] = node;
    }

    public bool Remove(TK key)
    {
        if (!_cacheMap.TryGetValue(key, out var node)) return false;
        _lruList.Remove(node);
        _cacheMap.Remove(key);
        return true;
    }

    private void RemoveFirst()
    {
        var node = _lruList.First;
        if (node == null) return;

        _lruList.RemoveFirst();
        _cacheMap.Remove(node.Value.Key);
    }

    private class LruCacheItem<TK, TV>(TK key, TV value)
    {
        public readonly TK Key = key;
        public readonly TV Value = value;
    }
}