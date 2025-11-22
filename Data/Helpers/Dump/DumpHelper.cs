using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump;

namespace RPGMakerLibCoreV2.Data.Helpers.Dump;

public static class DumpHelper
{
    public static async Task Dump(string path, Storage storage, DumpOptions? options = null, ProgressReporter.ProgressReporterCallback? reporterCallback = null, CancellationToken? token = null)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        options ??= new();
        
        var reporter = new ProgressReporter(reporterCallback, token);
        reporter.SetStage(DumpStage.Init);
        reporter.ResetTotal(3);

        reporter.SetStage(DumpStage.Progress);
        await reporter.Report().For(async () => await CommonEventHelper.DumpCommonEventCommands(storage, path, options, reporter));
        await reporter.Report().For(async () => await MapEventHelper.DumpMapEventCommands(storage, path, options, reporter));
        await reporter.Report().For(async () => await TroopHelper.DumpTroopCommands(storage, path, options, reporter));
        
        reporter.SetStage(DumpStage.Finished);
    }

    public static void DumpYaml(string path, Storage storage, ProgressReporter.ProgressReporterCallback? reporterCallback = null, CancellationToken? token = null)
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

        yamlStorage.Reporter = reporterCallback != null ? new ProgressReporter(reporterCallback, token) : null;

        yamlStorage.Dump(path, storage);
    }

    public static void LoadYaml(string path, Storage storage, ProgressReporter.ProgressReporterCallback? reporter = null, CancellationToken? token = null)
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

        yamlStorage.Reporter = reporter != null ? new ProgressReporter(reporter, token) : null;

        yamlStorage.Load(path, storage);
    }
}
