using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.Structures.Commands;

public class SelectItemCommand : NamedCommand
{
    public override int Code => CommandCodes.SelectItem;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Select Item";
        else
            return "Select Key Item";
    }
    
    public enum KeyItemType
    {
        Regular = 1,
        Key = 2,
        HiddenA = 3,
        HiddenB = 4,
    }

    private static readonly Dictionary<KeyItemType, string> KeyItemTypeNames = new()
    {
        [KeyItemType.Regular] = "Regular Item",
        [KeyItemType.Key] = "Key Item",
        [KeyItemType.HiddenA] = "Hidden Item A",
        [KeyItemType.HiddenB] = "Hidden Item B",
    };

    [EditorDefault(1, StorageVersion.VXAce)]
    public int VariableId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    /// <summary>
    /// The kind of the items to select. MV only: VX Ace selects key items and has no such parameter, so it throws
    /// <c>DoesNotExistInVersionException</c>. A new command of MV selects regular items, as the editor makes it.
    /// </summary>
    [DoesNotExistInVersion(StorageVersion.VXAce)]
    public KeyItemType Type
    {
        get => Get<KeyItemType>(1);
        set => Set(1, value);
    }

    protected override void OnBinding(StorageVersion version)
    {
        // a new command of MV selects regular items
        if (version == StorageVersion.MV && !IsParameterSet(1))
            Set(1, KeyItemType.Regular);
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        // VX Ace has the variable only
        if (version == StorageVersion.VXAce && Raw.Parameters is { Count: > 1 } parameters)
            parameters.RemoveRange(1, parameters.Count - 1);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatVariable(VariableId, new()
        {
            BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
        });

        if (context.Version == StorageVersion.MV)
        {
            if (!KeyItemTypeNames.TryGetValue(Type, out var itemType))
                throw new Exception($"Unknown item type for select item command: {Type}");

            str += $", {itemType}";
        }

        return str;
    }
}