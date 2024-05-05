using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

public class SimpleParameterProvider : IParameterProvider
{
    private readonly LookupList<object> _parameter;

    public SimpleParameterProvider(LookupList<object> parameters)
    {
        _parameter = parameters;
    }

    public T? GetParameter<T>(int index)
    {
        var value = _parameter.Get(index);
        if (value == null)
            return default;

        return (T?)SerializationHelper.ManualCast(typeof(T), value);
    }

    public void SetParameter<T>(int index, T? value)
    {
        _parameter.Set(index, value);
    }
}
