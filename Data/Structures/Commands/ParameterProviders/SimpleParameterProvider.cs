using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization;

namespace RPGMDataKit.Data.Structures.Commands.ParameterProviders;

public class SimpleParameterProvider : IParameterProvider
{
    private readonly ICommand _command;

    public SimpleParameterProvider(ICommand command)
    {
        _command = command;
    }

    public T? GetParameter<T>(int index)
    {
        var value = _command.Parameters?.Get(index);
        if (value == null)
            return default;

        return (T?)SerializationHelper.AssignmentConversion(typeof(T), value, true);
    }

    public void SetParameter<T>(int index, T? value)
    {
        _command.Parameters ??= new();
        _command.Parameters.Set(index, value);
    }

    public int Count => _command.Parameters?.Count ?? 0;
    
    public void Clear() => _command.Parameters?.Clear();
}
