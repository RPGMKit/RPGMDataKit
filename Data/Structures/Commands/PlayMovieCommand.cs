using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayMovieCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayMovie;
    public override string GetCommandName(StorageVersion version) => "Play Movie";

    public string Movie
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (string.IsNullOrEmpty(Movie))
            return "None";

        return $"'{Movie}'";
    }
}