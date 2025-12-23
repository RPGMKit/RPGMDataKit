namespace RPGMakerLibCoreV2.Data.Helpers.Dump;

public class ProgressReporter
{
    public ProgressReporter(ProgressReporterCallback? reporterCallback, CancellationToken? token = null)
    {
        _reporterCallback = reporterCallback;
        _token = token;
    }

    public class ReportContext : IDisposable
    {
        private readonly Action _finished;

        private bool _needCall = true;
        
        public int Level { get; private set; }
        
        public ReportContext(Action finished, int level)
        {
            _finished = finished;
            Level = level;
        }

        public async Task For(Func<Task> f)
        {
            await f();
            _finished();
            _needCall = false;
        }
        
        public void For(Action a)
        {
            a();
            _finished();
            _needCall = false;
        }

        public void Dispose()
        {
            if (_needCall)
                _finished();
        }
    }

    private ProgressReporterCallback? _reporterCallback;

    private int _level = 0;
    private Dictionary<int, int> _current = new();
    private Dictionary<int, int> _total = new();
    
    public void ResetTotal(int val)
    {
        Set(0);
        SetTotal(val);
    }

    public void SetLevel(int level)
    {
        _level = level;
    }
    
    public ReportContext IncLevel(int total)
    {
        ++_level;
        ResetTotal(total);
        
        return CreateContext(() => --_level);
    }

    private readonly CancellationToken? _token;

    public DumpStage Stage { get; private set; }

    public void SetStage(DumpStage stage)
    {
        switch (stage)
        {
            case DumpStage.Init:
            case DumpStage.Cancelled:
            case DumpStage.Finished:
                _reporterCallback?.Invoke(stage, "", 0, 0, 0);
                break;
            case DumpStage.Progress:
                if (Stage == DumpStage.None)
                    _reporterCallback?.Invoke(DumpStage.Init, "", 0, 0, 0);
                break;
        }

        Stage = stage;
    }

    public bool NeedCancel => _token?.IsCancellationRequested ?? false; 

    public delegate void ProgressReporterCallback(DumpStage stage, string message, int level, int currentIndex, int total);

    public ReportContext Report(string message = "", int level = -1)
    {
        if (level == -1)
            level = _level;
        
        var indices = _current.Keys.Concat(_total.Keys).Distinct();

        foreach (var index in indices)
            _reporterCallback?.Invoke(Stage, message, index, Get(index), GetTotal(index));

        return CreateContext(() =>
        {
            if (Stage == DumpStage.Progress)
                Inc(level);
        });
    }

    private ReportContext CreateContext(Action a)
    {
        return new(a, _level);
    }

    private int Get(int level)
    {
        return _current.GetValueOrDefault(level, 0);
    }
    
    private int GetTotal(int level)
    {
        return _total.GetValueOrDefault(level, 0);
    }

    private void Inc(int level)
    {
        _current[level] = Get(level) + 1;
    }
    
    private void IncTotal()
    {
        _total[_level] = GetTotal(_level) + 1;
    }

    private void Set(int value)
    {
        _current[_level] = value;
    }

    public void SetTotal(int value)
    {
        _total[_level] = value;
    }
}
