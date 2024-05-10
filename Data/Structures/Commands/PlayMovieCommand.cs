using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayMovieCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayMovie;
    public override string GetCommandName(StorageVersion version) => "Play Movie";

    public string GetMovie()
    {
        return GetParameter<string>(0);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var movie = GetMovie();
        if (string.IsNullOrEmpty(movie))
            return "None";

        return $"'{movie}'";
    }
}