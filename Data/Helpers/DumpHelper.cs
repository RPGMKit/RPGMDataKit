using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class DumpHelper
{
    public static void Dump(string path, Storage storage, DumpOptions? options = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        // DumpYaml(path, storage);

        var textsPath = Path.Combine(path, "Texts");
        if (!Directory.Exists(textsPath))
            Directory.CreateDirectory(textsPath);

        options ??= new();
        
        DumpCommands(textsPath, storage, options);
    }

    public static void DumpYaml(string path, Storage storage)
    {
        AbstractYamlDumper yamlDumper;
        switch (storage.Version)
        {
            case StorageVersion.VXAce:
                yamlDumper = new VxAceYamlDumper();
                break;
            case StorageVersion.MV:
            default:
                throw new Exception($"Unsupported storage version {storage.Version}");
        }

        yamlDumper.Dump(path, storage);
    }

    public static void DumpCommands(string path, Storage storage, DumpOptions options)
    {
        CommonEventHelper.DumpCommonEventCommands(storage, path, options);
        MapEventHelper.DumpMapEventCommands(storage, path, options);
        TroopHelper.DumpTroopCommands(storage, path, options);
    }
}
