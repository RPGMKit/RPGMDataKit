using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.MV.Classes;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeClassCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeClass;
    public override string GetCommandName(StorageVersion version) => "Change Class";

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int ClassId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    /// <summary>"Save Level" of the editor of MV (the engine calls it <c>keepExp</c>). VX Ace has no such parameter and throws <c>DoesNotExistInVersionException</c>.</summary>
    public bool SaveLevel
    {
        get
        {
            RequireVersion(StorageVersion.MV, nameof(SaveLevel));
            return Get<bool>(2);
        }
        set
        {
            RequireVersion(StorageVersion.MV, nameof(SaveLevel));
            Set(2, value);
        }
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        // VX Ace has the actor and the class only
        if (version == StorageVersion.VXAce && Raw.Parameters is { Count: > 2 } parameters)
            parameters.RemoveRange(2, parameters.Count - 2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(ActorId)}, {context.NameFormatter.FormatClass(ClassId)}";
    }
}