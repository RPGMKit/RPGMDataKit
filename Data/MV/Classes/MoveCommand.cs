using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class MoveCommand : JsonEntity, IMoveCommand
{
    [JsonProperty(PropertyName = "code")]
    public int Code { get; set; }

    [JsonProperty(PropertyName = "parameters")]
    public LookupList<JToken> Parameters;

    [JsonIgnore]
    public IParameterProvider ParameterProvider => new JsonParameterProvider(Parameters);
}
