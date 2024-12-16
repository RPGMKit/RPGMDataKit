using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
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

        [JsonProperty(PropertyName = "trigger")]
        public int Trigger { get; set; }
        
        [JsonProperty(PropertyName = "switchId")]
        public int SwitchId { get; set; }

        [JsonProperty(PropertyName = "list")]
        public LookupList<Command>? List { get; set; }
        
        public List<NamedCommand> NamedCommands { get; set; }
        public IEnumerable<ICommandWithIndent>? Commands => List;

        protected override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.MV);

        protected override void OnPreSerialize() => List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.MV);
        
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
