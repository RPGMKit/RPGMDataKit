namespace RPGMakerLibCoreV2.Data;

internal interface ITable
{
    void Preload();

    void Save();

    void Unload();
}
