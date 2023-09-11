using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures;

public class Command : JsonEntity
{
    [JsonProperty(PropertyName = "code")]
    public int Code;

    [JsonProperty(PropertyName = "indent")]
    public int? Indent;

    [JsonProperty(PropertyName = "parameters")]
    public LookupList<JToken> Parameters;
}
