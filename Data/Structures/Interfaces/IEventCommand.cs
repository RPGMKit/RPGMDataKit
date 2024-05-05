using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IEventCommand
{
    public int Code { get; set; }

    public int? Indent { get; set; }

    public IParameterProvider ParameterProvider { get; }
}