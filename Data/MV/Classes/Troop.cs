using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Troop : JsonEntity, ITroop
{
    public class Member : JsonEntity, IMember
    {
        [JsonProperty("enemyId")]
        public int EnemyId { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }
    }

    public class Page : JsonEntity, IPage
    {
        [JsonProperty(PropertyName = "list")]
        public LookupList<EventCommand> List { get; set; }

        public IEnumerable<NamedCommand> NamedCommands => List.Select(CommandFactory.Create);
        public IEnumerable<IEventCommand> Commands => List;
        
        public override string Dump(EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
                return CommandContainerHelper.Dump(this, context, options);

            return base.Dump(context, options);
        }
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("members")]
    public LookupList<Member> _Members { get; set; }

    public IByIdGetter<IMember> Members => _Members;

    [JsonProperty("pages")]
    public LookupList<Page> _Pages { get; set; }

    public IByIdGetter<IPage> Pages => _Pages;
}
