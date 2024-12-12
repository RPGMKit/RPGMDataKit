using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

public class SimpleParameterProvider : IParameterProvider
{
    private readonly LookupList<object> _parameters;

    public SimpleParameterProvider(LookupList<object> parameters)
    {
        _parameters = parameters;
    }

    public T? GetParameter<T>(int index)
    {
        var value = _parameters.Get(index);
        if (value == null)
            return default;

        return (T?)SerializationHelper.AssignmentConversion(typeof(T), value, true);
    }

    public void SetParameter<T>(int index, T? value)
    {
        _parameters.Set(index, value);
    }

    public int Count => _parameters.Count;
}
