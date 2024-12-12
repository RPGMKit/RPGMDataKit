using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayMovieCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayMovie;
    public override string GetCommandName(StorageVersion version) => "Play Movie";

    public string Movie { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Movie = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Movie);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (string.IsNullOrEmpty(Movie))
            return "None";

        return $"'{Movie}'";
    }
}