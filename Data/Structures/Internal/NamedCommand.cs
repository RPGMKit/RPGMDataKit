using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public abstract class NamedCommand
{
    public abstract int Code { get; }
    
    public abstract string GetCommandName(StorageVersion version);

    public int? Indent { get; set; }

    public bool IsSecondary;

    public T Cast<T>() where T : NamedCommand => (T)this;

    public virtual void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
    }

    public void _Pack(ICommand command, StorageVersion version)
    {
        IParameterProvider provider;
        if (version == StorageVersion.MV)
            provider = new JsonParameterProvider(command);
        else
            provider = new SimpleParameterProvider(command);

        provider.Clear();
        Pack(provider, version);
    }

    public virtual void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
    }

    public virtual string GetParametersDescription(EntityContext context, DumpOptions options) => "";

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

    public string AsString(EntityContext context, DumpOptions options)
    {
        string str = "";
        if (options.WithIndent && Indent > 0)
        {
            var whiteSpace = context.Version == StorageVersion.MV ? " " : "  ";

            str = string.Concat(Enumerable.Repeat(whiteSpace, (int)Indent));
        }

        str += GetStringPrefix(context.Version);

        var name = GetCommandName(context.Version);
        str += name;

        var paramDesc = GetParametersDescription(context, options);

        if (!string.IsNullOrEmpty(paramDesc))
        {
            if (string.IsNullOrWhiteSpace(name) || !IsSecondary)
                str += context.Version == StorageVersion.MV ? " : " : ": ";
            else
                str += " ";

            str += paramDesc;
        }

        if (options.AppendCommandCode)
            str = $"({Code:D3}) {str}";

        // str += $"Indent: " + (Indent != null ? Indent : "null");

        return str;
    }
}
