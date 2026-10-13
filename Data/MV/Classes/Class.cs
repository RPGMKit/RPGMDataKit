using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Class : JsonEntity, IClass, INewEntity
{
    public class Learning : JsonEntity, ILearning, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new learning: level 1, the first skill.</summary>
        public void InitializeNew()
        {
            Level = 1;
            SkillId = 1;
            Note = "";
        }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("skillId")]
        public int SkillId { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("note")]
    public string Note { get; set; }
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
    
    [JsonProperty("expParams")]
    public List<int> ExpParams { get; set; }

    [JsonProperty("params")]
    public List<object> Params { get; set; }

    [JsonProperty("learnings")]
    public List<Learning> Learnings { get; set; }

    /// <summary>The values that the editor of MV gives to a new class: the curves of the parameters and the basic traits.</summary>
    public void InitializeNew()
    {
        Name = "";
        Note = "";
        ExpParams = [30, 20, 30, 30];

        // 8 parameters, 100 levels each (the level 0 is not used)
        Params = new();
        for (var parameter = 0; parameter < 8; parameter++)
        {
            var levels = new List<int>(new int[100]);
            for (var level = 1; level < 100; level++)
            {
                levels[level] = parameter switch
                {
                    0 => 400 + level * 50,
                    1 => 80 + level * 10,
                    <= 5 => 15 + level * 5 / 4,
                    _ => 30 + level * 5 / 2,
                };
            }

            Params.Add(levels);
        }

        Learnings = new();
        Traits =
        [
            new Trait { Code = FeatureCodes.SpecialParameter, DataId = 0, Value = 1 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0.95 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Critical, Value = 0 },
            new Trait { Code = FeatureCodes.SkillTypeAdd, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.EquipWeaponType, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.EquipArmorType, DataId = 1, Value = 0 },
        ];
    }
}
