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
        public IEnumerable<ICommand>? Commands => List;

        public override void OnDeserialized()
        {
            base.OnDeserialized();
            NamedCommands = CommandContainerHelper.Unpack(List, StorageVersion.MV);
        }

        public override void OnSerializing()
        {
            base.OnSerializing();
            List = CommandContainerHelper.Pack<Command>(NamedCommands, StorageVersion.MV);
        }

        public override async Task Dump(TextWriter writer, EntityContext context, DumpOptions options)
        {
            if (options.Format == DumpFormat.Text)
            {
                await writer.WriteLineAsync();
                foreach (var line in CommandContainerHelper.CommandsAsText(this, context, options))
                    await writer.WriteLineAsync(line);
                return;
            }

            await base.Dump(writer, context, options);
        }
    }
}
