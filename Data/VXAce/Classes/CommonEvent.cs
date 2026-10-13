using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::CommonEvent")]
public class CommonEvent : VxCommandContainer, ICommonEvent
{
    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@trigger")]
    public int Trigger { get; set; }

    [RubyProperty("@switch_id")]
    public int SwitchId { get; set; }

    public async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        if (options.Format == DumpFormat.Text)
        {
            await writer.WriteLineAsync($"Name: [{Id:D3}] {Name}");
            await writer.WriteLineAsync($"Trigger: {Trigger}");
            await writer.WriteLineAsync("SwitchId: " + (Trigger > 0 ? context.NameFormatter.FormatSwitch(SwitchId) : "None"));
            await writer.WriteLineAsync("Commands:");
            foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                await writer.WriteLineAsync(line);
            return;
        }

        throw new NotImplementedException($"{options.Format} not supported");
    }
}
