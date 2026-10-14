using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Class : BaseItem, IClass
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
    
    [JsonProperty("traits")]
    public List<Trait> Traits { get; set; }
    
    [JsonProperty("expParams")]
    public List<int> ExpParams { get; set; }

    [JsonProperty("params")]
    public List<object> Params { get; set; }

    [JsonProperty("learnings")]
    public List<Learning> Learnings { get; set; }

    /// <summary>
    /// The values of the curve of ATK, DEF, MAT and MDF at the levels 1 to 99 that the editor of MV gives to a new class
    /// (it is not the formula of VX Ace: the editor draws the curve and rounds it, so the numbers are copied from its file).
    /// </summary>
    private static readonly int[] OtherParameterCurve =
    [
        16, 18, 19, 20, 21, 23, 24, 25, 26, 28, 29, 30, 31, 33, 34, 35, 36, 38, 39, 40, 41, 43, 44, 45, 46, 48, 49, 50, 51,
        53, 54, 55, 56, 58, 59, 60, 61, 63, 64, 65, 66, 68, 69, 70, 71, 73, 74, 75, 76, 77, 79, 80, 81, 82, 84, 85, 86, 87,
        89, 90, 91, 92, 94, 95, 96, 97, 99, 100, 101, 102, 104, 105, 106, 107, 109, 110, 111, 112, 114, 115, 116, 117, 119,
        120, 121, 122, 124, 125, 126, 127, 129, 130, 131, 132, 134, 135, 136, 137, 138,
    ];

    /// <summary>The values that the editor of MV gives to a new class: the curves of the parameters and the basic traits.</summary>
    IReadOnlyList<IFeature> IHasFeatures.Features => Traits;

    public override void InitializeNew()
    {
        base.InitializeNew();
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
                    <= 5 => OtherParameterCurve[level - 1],
                    _ => 30 + level * 5 / 2,
                };
            }

            // the editor of MV puts 1 into the level 0 of the maximum HP and of the attack
            if (parameter is 0 or 2)
                levels[0] = 1;

            Params.Add(levels);
        }

        Learnings = new();
        Traits =
        [
            new Trait { Code = FeatureCodes.SpecialParameter, DataId = 0, Value = 1 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.HitRate, Value = 0.95 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Evasion, Value = 0.05 },
            new Trait { Code = FeatureCodes.ExtraParameter, DataId = ExtraParameterIds.Critical, Value = 0.04 },
            new Trait { Code = FeatureCodes.SkillTypeAdd, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.EquipWeaponType, DataId = 1, Value = 0 },
            new Trait { Code = FeatureCodes.EquipArmorType, DataId = 1, Value = 0 },
        ];
    }
}
