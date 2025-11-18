using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::CommonEvent")]
public class CommonEvent : RubySerializationEntity, ICommonEvent
{
    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@trigger")]
    public int Trigger { get; set; }

    [RubyProperty("@switch_id")]
    public int SwitchId { get; set; }

    [RubyProperty("@list")]
    public LookupList<Command>? List;

    public List<NamedCommand> NamedCommands { get; set; }

    public IEnumerable<ICommandWithIndent>? Commands => List;

    public override void OnDeserialized()
    {
        base.OnDeserialized();
        NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.VXAce);
    }

    public override void OnSerializing()
    {
        base.OnSerializing();
        List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.VXAce);
    }

    public void Dump(TextWriter writer, EntityContext context, DumpOptions options)
    {
        if (options.Format == DumpFormat.Text)
        {
            foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                writer.WriteLine(line);
            return;
        }

        throw new NotImplementedException($"{options.Format} not supported");
    }
}
