using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump;

namespace RPGMakerLibCoreV2.Data.Helpers.Dump;

public static class DumpHelper
{
    public static async Task Dump(string path, Storage storage, DumpOptions? options = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        options ??= new();
        
        await DumpCommands(path, storage, options);
    }

    public static void DumpYaml(string path, Storage storage, DumpProgressReporter.DumpReporter? reporter = null, CancellationToken? token = null)
    {
        AbstractYamlStorage yamlStorage;
        switch (storage.Version)
        {
            case StorageVersion.VXAce:
                yamlStorage = new VxAceYamlStorage();
                break;
            case StorageVersion.MV:
                yamlStorage = new MvYamlStorage();
                break;
            default:
                throw new Exception($"Unsupported storage version {storage.Version}");
        }

        yamlStorage.Reporter = new DumpProgressReporter(reporter, token);

        yamlStorage.Dump(path, storage);
    }

    public static void LoadYaml(string path, Storage storage, DumpProgressReporter.DumpReporter? reporter = null, CancellationToken? token = null)
    {
        AbstractYamlStorage yamlStorage;
        switch (storage.Version)
        {
            case StorageVersion.VXAce:
                yamlStorage = new VxAceYamlStorage();
                break;
            case StorageVersion.MV:
                yamlStorage = new MvYamlStorage();
                break;
            default:
                throw new Exception($"Unsupported storage version {storage.Version}");
        }

        yamlStorage.Reporter = new DumpProgressReporter(reporter, token);

        yamlStorage.Load(path, storage);
    }

    public static async Task DumpCommands(string path, Storage storage, DumpOptions options)
    {
        await CommonEventHelper.DumpCommonEventCommands(storage, path, options);
        await MapEventHelper.DumpMapEventCommands(storage, path, options);
        await TroopHelper.DumpTroopCommands(storage, path, options);
    }
}
