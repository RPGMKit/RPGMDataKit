namespace RPGMDataKit.Data;

/// <summary>What a storage does with every holder it keeps: loads it ahead, saves it, drops what was loaded.</summary>
internal interface IHolder
{
    void Preload();

    void Save();

    void Unload();
}
