namespace RPGMakerLibCoreV2;

public interface IDumpable
{
    public string Dump(Storage storage, DumpOptions options);
}