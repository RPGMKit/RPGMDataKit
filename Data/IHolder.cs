namespace RPGMDataKit.Data;

/// <summary>What a storage does with every holder it keeps: loads it ahead, saves it, drops what was loaded.</summary>
internal interface IHolder
{
    void Preload();

    void Save();

    void Unload();

    /// <summary>
    /// Lets the visitor work with the holder under its real type: a holder is known to a storage as <see cref="IHolder"/> only,
    /// and the visitor needs the type of the entity (<c>DataLookupHolder&lt;IActor&gt;</c>, <c>DataLookupHolder&lt;Quest&gt;</c>).
    /// </summary>
    /// <param name="name">The name of the holder in the storage.</param>
    Task Accept(IHolderVisitor visitor, string name);
}

/// <summary>Work that depends on the type of the entity of a holder (a dump, loading a dump).</summary>
internal interface IHolderVisitor
{
    /// <summary>A list or a dictionary of entities.</summary>
    Task Visit<T>(DataLookupHolder<T> holder, string name);

    /// <summary>A single entity.</summary>
    Task Visit<T>(DataHolder<T> holder, string name);

    Task Visit(MapsHolder holder, string name);
}
