using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ICommand
{
    public int Code { get; set; }
    
    public int? Indent { get; set; }
    
    public List<object?>? Parameters { get; set; }

    public IParameterProvider ParameterProvider { get; }
}