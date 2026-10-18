using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeActorImagesCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorImages;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Actor Images";
        else
            return "Change Actor Graphic";
    }

    [EditorDefault(1, StorageVersion.VXAce)]
    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault("")]
    public string CharacterImage
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public int CharacterImageOffset
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    [EditorDefault("")]
    public string FaceImage
    {
        get => Get<string>(3);
        set => Set(3, value);
    }

    public int FaceImageOffset
    {
        get => Get<int>(4);
        set => Set(4, value);
    }

    // exists only in MV; a command made in code keeps it until it is put into a VXAce list
    public string SwBattler
    {
        get => Get<string>(5);
        set
        {
            if (Version == StorageVersion.MV || IsDetached)
                Set(5, value);
        }
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version != StorageVersion.MV)
            Raw.Parameters!.RemoveRange(5, Raw.Parameters.Count - 5);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatActor(ActorId);

        var format = context.Version == StorageVersion.MV ? ", {0}({1})" : ", '{0}', {1}";
        var none = context.Version == StorageVersion.MV ? ", None" : ", (None)";

        if (!string.IsNullOrEmpty(FaceImage))
            str += string.Format(format, FaceImage, FaceImageOffset);
        else
            str += none;

        if (!string.IsNullOrEmpty(CharacterImage))
            str += string.Format(format, CharacterImage, CharacterImageOffset);
        else
            str += none;

        if (context.Version == StorageVersion.MV)
        {
            if (!string.IsNullOrEmpty(SwBattler))
                str += $", {SwBattler}";
            else
                str += none;
        }

        return str;
    }
}