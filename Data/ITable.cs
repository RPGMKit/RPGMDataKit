namespace RPGMDataKit.Data;

internal interface ITable
{
    void Preload();

    void Save();

    void Unload();
}
