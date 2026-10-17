using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class MoveCommand : JsonEntity, IMoveCommand
{
    [JsonProperty(PropertyName = "code")]
    public int Code { get; set; }

    [JsonProperty("indent")]
    public int? Indent { get; set; } = null; // always null

    [JsonProperty(PropertyName = "parameters", NullValueHandling = NullValueHandling.Ignore)]
    public List<object?>? Parameters { get; set; }

    [JsonIgnore]
    public IParameterProvider ParameterProvider => new JsonParameterProvider(this);

    // the empty command that ends a route has no "indent" in the files of the editor ({"code":0}, or {"code":0,"parameters":[]} in the route of a
    // page of a map); the other steps have "indent": null
    public bool ShouldSerializeIndent() => Code != 0 || Indent != null;
}
