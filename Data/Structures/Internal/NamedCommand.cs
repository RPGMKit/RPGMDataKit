using Newtonsoft.Json.Linq;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class NamedCommand
{
    public LookupList<JToken> Parameters = new();

    public abstract int Code { get; }
    public abstract string Name { get; }

    public int? Indent { get; set; }

    public bool IsSecondary;

    public virtual string GetParametersDescription(Storage storage, DumpOptions options) => "";

    protected virtual string GetStringPrefix()
    {
        return !IsSecondary ? "◆" : ":";
    }

    public JToken? GetParameter(int index)
    {
        return Parameters.Get(index);
    }

    public void SetParameter(int index, JToken value)
    {
        Parameters.Set(index, value);
    }

    public string AsString(Storage storage, DumpOptions options)
    {
        var str = options.WithIndent && Indent > 0 ? string.Concat(Enumerable.Repeat(' ', (int)Indent)) : "";

        str += GetStringPrefix();

        var name = Name;
        str += name;

        var paramDesc = GetParametersDescription(storage, options);

        if (!string.IsNullOrEmpty(paramDesc))
            str += ((string.IsNullOrWhiteSpace(name) || !IsSecondary) ? " : " : " ") + paramDesc;

        if (options.AppendCommandCode)
            str += $" ({Code})";

        // str += $"Indent: " + (Indent != null ? Indent : "null");

        return str;
    }
}
