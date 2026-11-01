using RPGMDataKit.Data.Enums;
using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump;

namespace RPGMDataKit.Data.Helpers.Dump;

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

        var existingFiles = Directory.GetFiles(path, "*." + options.FileExtension, SearchOption.AllDirectories);
        List<string> createdFiles = [];
        reporter.Report += (stage, message, level, index, total) =>
        {
            if (stage == DumpStage.Custom)
                createdFiles.Add(Path.GetFullPath(message));
        };

        // an html dump has an index in the folder, and each page has a link to it (the pages are one folder down)
        var html = options.Format == DumpFormat.Html;
        var index = html ? new DumpIndex(path) : null;
        var pageOptions = options;
        if (html)
        {
            pageOptions = options.Clone();
            pageOptions.IndexLink = "../" + HtmlIndexWriter.FileName;
        }

        reporter.SetStage(DumpStage.Progress);
        await reporter.DoReport().For(async () => await CommonEventHelper.DumpCommonEventCommands(storage, Path.Combine(path, "CE"), pageOptions, reporter, index));
        await reporter.DoReport().For(async () => await MapEventHelper.DumpMapEventCommands(storage, Path.Combine(path, "ME"), pageOptions, reporter, index));
        await reporter.DoReport().For(async () => await TroopHelper.DumpTroopCommands(storage, Path.Combine(path, "TE"), pageOptions, reporter, index));

        if (index != null && !reporter.NeedCancel)
            reporter.CustomReport(await HtmlIndexWriter.Write(path, storage, index));
        
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
        var yamlStorage = storage.CreateYamlStorage();

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

        await yamlStorage.Dump(path);

        if (!yamlStorage.Reporter.NeedCancel)
        {
            var extraFiles = existingFiles.Except(createdFiles);
            foreach (var extraFile in extraFiles)
                File.Delete(extraFile);
        }
    }

    public static void LoadYaml(string path, Storage storage, ProgressReporter.ProgressReporterCallback? reporter = null, CancellationToken? token = null)
    {
        var yamlStorage = storage.CreateYamlStorage();

        yamlStorage.Reporter = reporter != null ? new ProgressReporter(token) : null;
        yamlStorage.Reporter?.Report += reporter;

        yamlStorage.Load(path);
    }
}
