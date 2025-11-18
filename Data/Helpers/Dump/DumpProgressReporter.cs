namespace RPGMakerLibCoreV2.Data.Helpers.Dump;

public class DumpProgressReporter
{
    public DumpProgressReporter(DumpReporter? reporter, CancellationToken? token = null)
    {
        _reporter = reporter;
        _token = token;
    }

    public class ReportContext : IDisposable
    {
        private readonly Action _finished;

        public ReportContext(Action finished)
        {
            _finished = finished;
        }

        public void Dispose()
        {
            _finished();
        }
    }

    private DumpReporter? _reporter { get; set; }

    private int _current;
    private int _globalCurrent;
    
    private int _total;
    private int _globalTotal;
    
    public void ResetTotal(int val)
    {
        _current = 0;
        _total = val;
    }

    public void SetGlobalTotal(int val)
    {
        _globalTotal = val;
    }

    private readonly CancellationToken? _token;

    public DumpStage Stage { get; private set; }
    
    public void SetStage(DumpStage stage) => Stage = stage;
    
    public bool NeedCancel => _token != null && _token.Value.IsCancellationRequested; 

    public delegate void DumpReporter(DumpStage stage, string current, int id, int currentIndex, int total, int currentGlobalIndex, int globalTotal);

    public ReportContext Report(string current = "", int id = -1)
    {
        _reporter?.Invoke(Stage, current, id, _current, _total, _globalCurrent, _globalTotal);

        return new(() =>
        {
            if (Stage == DumpStage.Progress)
                ++_current;
        });
    }

    public ReportContext ReportGlobal(string current = "", int id = -1)
    {
        _reporter?.Invoke(Stage, current, id, _current, _total, _globalCurrent, _globalTotal);
        
        return new(() =>
        {
            if (Stage == DumpStage.Progress)
                ++_globalCurrent;
        });
    }
}
