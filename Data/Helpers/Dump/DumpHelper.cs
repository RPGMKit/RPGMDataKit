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
        
        var reporter = new ProgressReporter(token);
        if (reporterCallback != null)
            reporter.Report += reporterCallback;
        reporter.SetStage(DumpStage.Init);
        reporter.ResetTotal(3);

        var existingFiles = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
        List<string> createdFiles = [];
        reporter.Report += (stage, message, level, index, total) =>
        {
            if (stage == DumpStage.Custom)
                createdFiles.Add(Path.GetFullPath(message));
        };

        reporter.SetStage(DumpStage.Progress);
        await reporter.DoReport().For(async () => await CommonEventHelper.DumpCommonEventCommands(storage, path, options, reporter));
        await reporter.DoReport().For(async () => await MapEventHelper.DumpMapEventCommands(storage, path, options, reporter));
        await reporter.DoReport().For(async () => await TroopHelper.DumpTroopCommands(storage, path, options, reporter));
        
        reporter.SetStage(DumpStage.Finished);
        
        if (!reporter.NeedCancel)
        {
            var extraFiles = existingFiles.Except(createdFiles);
            foreach (var extraFile in extraFiles)
                File.Delete(extraFile);
        }
    }

    public static async Task DumpYaml(string path, Storage storage,
        ProgressReporter.ProgressReporterCallback? reporterCallback = null, CancellationToken? token = null)
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

        yamlStorage.Reporter = new ProgressReporter(token);
        if (reporterCallback != null)
            yamlStorage.Reporter.Report += reporterCallback;

        var existingFiles = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories);
        List<string> createdFiles = [];

        yamlStorage.Reporter.Report += (stage, message, _, _, _) =>
        {
            if (stage == DumpStage.Custom)
                createdFiles.Add(Path.GetFullPath(message));
        };

        await yamlStorage.Dump(path, storage);

        if (!yamlStorage.Reporter.NeedCancel)
        {
            var extraFiles = existingFiles.Except(createdFiles);
            foreach (var extraFile in extraFiles)
                File.Delete(extraFile);
        }
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

        yamlStorage.Reporter = reporter != null ? new ProgressReporter(token) : null;
        yamlStorage.Reporter?.Report += reporter;

        yamlStorage.Load(path, storage);
    }
}
