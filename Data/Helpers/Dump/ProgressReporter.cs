namespace RPGMakerLibCoreV2.Data.Helpers.Dump;

public class ProgressReporter
{
    public ProgressReporter(CancellationToken? token = null)
    {
        _token = token;
    }

    public class ReportContext : IDisposable
    {
        private readonly Action _finished;

        private bool _needFinish = true;
        
        public int Level { get; private set; }
        
        public ReportContext(Action finished, int level)
        {
            _finished = finished;
            Level = level;
        }

        public async Task For(Func<Task> f)
        {
            await f();
            Finish();
        }
        
        public void For(Action a)
        {
            a();
            Finish();
        }

        public void Finish()
        {
            _finished();
            _needFinish = false;
        }

        public void Dispose()
        {
            if (_needFinish)
                _finished();
        }
    }

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
        
        return CreateContext(() =>
        {
            _current[_level] = 0;
            --_level;
        });
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
                Report?.Invoke(stage, "", 0, 0, 0);
                break;
            case DumpStage.Progress:
                if (Stage == DumpStage.None)
                    Report?.Invoke(DumpStage.Init, "", 0, 0, 0);
                break;
        }

        Stage = stage;
    }

    public bool NeedCancel => _token?.IsCancellationRequested ?? false; 

    public delegate void ProgressReporterCallback(DumpStage stage, string message, int level, int currentIndex, int total);

    public ReportContext DoReport(string message = "", int level = -1)
    {
        if (level == -1)
            level = _level;
        
        var indices = _current.Keys.Concat(_total.Keys).Distinct();

        foreach (var index in indices)
            Report?.Invoke(Stage, message, index, Get(index), GetTotal(index));

        return CreateContext(() =>
        {
            if (Stage == DumpStage.Progress)
                Inc(level);
        });
    }

    public void CustomReport(string message)
    {
        Report?.Invoke(DumpStage.Custom, message, -1, -1, 0);
    }

    public event ProgressReporterCallback? Report;

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
