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
        if (value == null)
            return default;

        if (value is JValue jv)
            return jv.ToObject<T>();

        if (value is JObject jo)
            return jo.ToObject<T>();
        
        if (value is JArray ja)
            return ja.ToObject<T>();

        throw new Exception("not implemented");
    }

    public void SetParameter<T>(int index, T? value)
    {
        _parameters.Set(index, value != null ? JToken.FromObject(value) : null);
    }
}
