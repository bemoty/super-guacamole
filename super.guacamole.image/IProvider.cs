namespace super.guacamole.image;

public interface IProvider<TK, TV>
{
    /**
     * Provides the value associated with the given key from the underlying data source.
     */
    public Task<TV> Provide(TK key);
}