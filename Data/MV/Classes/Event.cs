using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class Event : JsonEntity, IEvent
    {
        public class Page : JsonEntity, IPage
        {
            [JsonProperty(PropertyName = "list")]
            public LookupList<Command> List { get; set; }

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

            public List<NamedCommand> NamedCommands { get; set; }
            public IEnumerable<ICommandWithIndent> Commands => List;
            
            protected override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.MV);

            protected override void OnPreSerialize() => List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.MV);

        }
        
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "pages")]
        public LookupList<Page> _Pages { get; set; }

        public IByIdGetter<IPage> Pages => _Pages;

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }
    }
}