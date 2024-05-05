using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;
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

    public IEnumerable<NamedCommand> NamedCommands => List.Select(VXAceCommandFactory.Create);
    public IEnumerable<IEventCommand> Commands => List;

    public string Dump(Storage storage, DumpOptions options)
    {
        if (options.Format == DumpFormat.Text)
            return string.Join("\r\n", NamedCommands.Select(_ => _.AsString(storage, options)));

        throw new NotImplementedException($"{options.Format} not supported");
    }
}
