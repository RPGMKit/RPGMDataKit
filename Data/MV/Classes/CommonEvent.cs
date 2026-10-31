using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class CommonEvent : MvCommandContainer, ICommonEvent, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new common event: it does not run by itself, only the command that ends it.</summary>
    public void InitializeNew()
    {
        Name = "";
        Trigger = CommonEventTriggerType.None;
        SwitchId = 1;
        NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.MV)];
    }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "trigger")]
    public int Trigger { get; set; }
        
    [JsonProperty(PropertyName = "switchId")]
    public int SwitchId { get; set; }

    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        var head = new StringWriter();
        await CommonEventHelper.DumpCommonEventInfo(this, head, context);
        await DumpWriters.For(options.Format).Write(writer, head.ToString(), this, context, options);
    }
}