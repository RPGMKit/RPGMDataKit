using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

public class JsonParameterProvider : IParameterProvider
{
    private readonly LookupList<object> _parameters;

    public JsonParameterProvider(LookupList<object> parameters)
    {
        _parameters = parameters;
    }

    public T? GetParameter<T>(int index)
    {
        var value = _parameters.Get(index);
        if (value == null)
            return default;

        if (value is JToken t)
            return t.ToObject<T>();

        var retType = typeof(T);
        if (retType.IsPrimitive)
            return (T)Convert.ChangeType(value, retType);
        
        if (retType.IsEnum)
            return (T)Enum.ToObject(retType, value);
        
        return (T?)SerializationHelper.AssignmentConversion(typeof(T), value, true);
        // return (T)value;
    }

    public void SetParameter<T>(int index, T? value)
    {
        // _parameters.Set(index, value != null ? JToken.FromObject(value, Jsonser) : null);
        _parameters.Set(index, value != null ? value : null);
    }

    private static JsonSerializer Jsonser = new JsonSerializer() { NullValueHandling = NullValueHandling.Include };

    public int Count => _parameters.Count;
}
