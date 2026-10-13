using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

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
