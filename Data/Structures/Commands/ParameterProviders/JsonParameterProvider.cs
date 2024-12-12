using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;

public class JsonParameterProvider : IParameterProvider
{
    private readonly LookupList<JToken> _parameters;

    public JsonParameterProvider(LookupList<JToken> parameters)
    {
        _parameters = parameters;
    }

    public T? GetParameter<T>(int index)
    {
        var value = _parameters.Get(index);
        return value == null ? default : value.ToObject<T>();
    }

    public void SetParameter<T>(int index, T? value)
    {
        _parameters.Set(index, value != null ? JToken.FromObject(value, Jsonser) : null);
    }

    private static JsonSerializer Jsonser = new JsonSerializer() { NullValueHandling = NullValueHandling.Include };

    public int Count => _parameters.Count;
}
