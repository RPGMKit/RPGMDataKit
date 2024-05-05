using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class NamedCommand
{
    public IParameterProvider ParameterProvider { get; set; }

    public abstract int Code { get; }
    public abstract string Name { get; }

    public int? Indent { get; set; }

    public bool IsSecondary;

    public virtual string GetParametersDescription(Storage storage, DumpOptions options) => "";

    public string PrefixSymbol { get; set; } = "◆"; 
    public string SecondaryPrefixSymbol { get; set; } = ":"; 
    
    protected virtual string GetStringPrefix()
    {
        return !IsSecondary ? PrefixSymbol : SecondaryPrefixSymbol;
    }

    public T? GetParameter<T>(int index)
    {
        return ParameterProvider.GetParameter<T>(index);
    }

    public void SetParameter(int index, JToken value)
    {
        ParameterProvider.SetParameter(index, value);
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
            str = $"({Code:D3}) {str}";

        // str += $"Indent: " + (Indent != null ? Indent : "null");

        return str;
    }
}
