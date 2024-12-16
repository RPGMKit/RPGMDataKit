using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes
{
    public class MoveRoute : JsonEntity, IMoveRoute
    {
        [JsonProperty(PropertyName = "repeat")]
        public bool Repeat { get; set; }

        [JsonProperty(PropertyName = "skippable")]
        public bool Skippable { get; set; }

        [JsonProperty(PropertyName = "wait")]
        public bool Wait { get; set; }

        [JsonProperty(PropertyName = "list")]
        public LookupList<MoveCommand> List { get; set; }

        public List<NamedCommand> NamedCommands { get; set; }
        
        public IEnumerable<IMoveCommand>? Commands => List;
        
        protected override void OnDeserialize() => NamedCommands = CommandContainerHelper.Unpack(Commands, StorageVersion.MV);

        protected override void OnPreSerialize() => List = CommandContainerHelper.Pack<MoveCommand>(NamedCommands, StorageVersion.MV);
    }
}
