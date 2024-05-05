using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class NamedCommand
{
    public IParameterProvider ParameterProvider { get; set; }

    public abstract int Code { get; }
    
    public abstract string GetCommandName(StorageVersion version);

    public int? Indent { get; set; }

    public bool IsSecondary;

    public virtual string GetParametersDescription(Storage storage, DumpOptions options) => "";

    public string GetPrefixSymbol(StorageVersion version)
    {
        switch (version)
        {
            case StorageVersion.MV:
                return "◆";
            case StorageVersion.VXAce:
                return "@>";
        }

        return "";
    }

    public string GetSecondaryPrefixSymbol(StorageVersion version)
    {
        switch (version)
        {
            case StorageVersion.MV:
                return ":";
            case StorageVersion.VXAce:
                return ": ";
        }

        return "";
    }

    protected virtual string GetStringPrefix(StorageVersion version)
    {
        return !IsSecondary ? GetPrefixSymbol(version) : GetSecondaryPrefixSymbol(version);
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
        string str = "";
        if (options.WithIndent && Indent > 0)
        {
            var whiteSpace = storage.Version == StorageVersion.VXAce ? "  " : " ";

            str = string.Concat(Enumerable.Repeat(whiteSpace, (int)Indent));
        }

        str += GetStringPrefix(storage.Version);

        var name = GetCommandName(storage.Version);
        str += name;

        var paramDesc = GetParametersDescription(storage, options);

        if (!string.IsNullOrEmpty(paramDesc))
        {
            if (string.IsNullOrWhiteSpace(name) || !IsSecondary)
                str += storage.Version == StorageVersion.MV ? " : " : ": ";

            str += paramDesc;
        }

        if (options.AppendCommandCode)
            str = $"({Code:D3}) {str}";

        // str += $"Indent: " + (Indent != null ? Indent : "null");

        return str;
    }
}
