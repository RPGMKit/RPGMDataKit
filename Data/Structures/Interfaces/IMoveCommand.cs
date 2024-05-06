using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMoveCommand
{
    public int Code { get; set; }

    public IParameterProvider ParameterProvider { get; }
}