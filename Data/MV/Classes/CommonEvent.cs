using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class CommonEvent : MvCommandContainer, ICommonEvent
{
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
        if (options.Format == DumpFormat.Text)
        {
            await writer.WriteLineAsync($"Name: [{Id:D3}] {Name}");
            await writer.WriteLineAsync($"Trigger: {Trigger}");
            await writer.WriteLineAsync("SwitchId: " + (Trigger > 0 ? context.NameFormatter.FormatSwitch(SwitchId) : "-"));
            await writer.WriteLineAsync("Commands:");
            foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                await writer.WriteLineAsync(line);
        }
    }
}