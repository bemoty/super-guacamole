namespace super.guacamole.image.Cache;

public interface ICache<TK, TV> where TK : notnull
{
    /**
     * Get the value associated with the given key. If it does not exist, tries to fetch it from the underlying provider.
     */
    public TV Get(TK key);

    /**
     * Puts the value associated with the given key into the cache.
     */
    public void Put(TK key, TV value);

    /**
     * Removes the value associated with the given key from the cache.
     *
     * @return true if the key was found and removed, false otherwise.
     */
    public bool Remove(TK key);
}