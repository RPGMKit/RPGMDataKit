using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

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