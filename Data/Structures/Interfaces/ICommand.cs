using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommand
{
    public int Code { get; set; }
    
    public int? Indent { get; set; }
    
    public LookupList<object?>? Parameters { get; set; }

    public IParameterProvider ParameterProvider { get; }
}