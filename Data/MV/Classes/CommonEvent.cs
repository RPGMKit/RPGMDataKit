using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class CommonEvent : JsonEntity, ICommonEvent
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public IEnumerable<NamedCommand> NamedCommands => List.Select(CommandFactory.Create);
        public IEnumerable<IEventCommand> Commands => List;

        [JsonProperty(PropertyName = "list")]
        public LookupList<EventCommand> List { get; set; }

        public override void Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
            {
                foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                    writer.WriteLine(line);
                return;
            }

            base.Dump(writer, context, options);
        }
    }
}
