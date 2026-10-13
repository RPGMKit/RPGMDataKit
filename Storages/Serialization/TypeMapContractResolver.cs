using Newtonsoft.Json.Serialization;

namespace RPGMDataKit.Storages.Serialization;

/// <summary>Deserializes members declared as an entity interface into the concrete type from <see cref="TypeMap"/>.</summary>
public class TypeMapContractResolver : DefaultContractResolver
{
    private readonly TypeMap _types;

    public TypeMapContractResolver(TypeMap types)
    {
        _types = types;
    }

    protected override JsonContract CreateContract(Type objectType)
    {
        return base.CreateContract(objectType.IsInterface ? _types.Resolve(objectType) : objectType);
    }
}
