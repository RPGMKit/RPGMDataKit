using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
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
    public int Trigger;

    [RubyProperty("@switch_id")]
    public int SwitchId;

    [RubyProperty("@list")]
    public List<EventCommand> List;

    public IEnumerable<NamedCommand> NamedCommands => List.Select(CommandFactory.Create);
    public IEnumerable<IEventCommand> Commands => List;

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
