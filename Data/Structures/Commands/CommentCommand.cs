using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class CommentCommand : NamedCommand
{
    public override int Code => CommandCodes.Comment;
    public override string GetCommandName(StorageVersion version) => "Comment";

    public string Comment { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Comment = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Comment);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Comment;
}