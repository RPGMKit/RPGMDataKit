using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Command : JsonEntity, ICommand
{
    [JsonProperty(PropertyName = "code")]
    public int Code { get; set; }

    [JsonProperty(PropertyName = "indent")]
    public int? Indent { get; set; }

    [JsonProperty(PropertyName = "parameters", NullValueHandling = NullValueHandling.Ignore)]
    public List<object?>? Parameters { get; set; }

    [JsonIgnore]
    public IParameterProvider ParameterProvider => new JsonParameterProvider(this);
}
