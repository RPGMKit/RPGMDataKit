using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::CommonEvent")]
public class CommonEvent : VxCommandContainer, ICommonEvent, INewEntity
{
    /// <summary>The values of <c>RPG::CommonEvent.new</c> of VX Ace: it does not run by itself, only the command that ends it.</summary>
    public void InitializeNew()
    {
        Name = "";
        Trigger = CommonEventTriggerType.None;
        SwitchId = 1;
        NamedCommands = [CommandContainerHelper.CreateNamed(new Command { Code = 0, Indent = 0, Parameters = [] }, StorageVersion.VXAce)];
    }

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
        var head = new StringWriter();
        await CommonEventHelper.DumpCommonEventInfo(this, head, context);
        await DumpWriters.For(options.Format).Write(writer, head.ToString(), this, context, options);
    }
}
