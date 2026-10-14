using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class UsableItem : JsonEntity
{
    public class Effect : JsonEntity, IEffect, INewEntity
    {
        /// <summary>What the editor of MV puts into an effect that is added and not changed: "Recover HP" by 100% and 0.</summary>
        public void InitializeNew()
        {
            Code = EffectCodes.RecoverHp;
            DataId = 0;
            Value1 = 1;
            Value2 = 0;
        }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("dataId")]
        public int DataId { get; set; }

        [JsonProperty("value1")]
        public object Value1 { get; set; }

        [JsonProperty("value2")]
        public object Value2 { get; set; }
    }

    public class ItemDamage : JsonEntity, IItemDamage, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new damage: no damage, the formula 0, the variance of 20%.</summary>
        public void InitializeNew()
        {
            Type = DamageType.None;
            ElementId = 0;
            Formula = "0";
            Variance = 20;
            Critical = false;
        }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("elementId")]
        public int ElementId { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        [JsonProperty("variance")]
        public int Variance { get; set; }

        [JsonProperty("critical")]
        public bool Critical { get; set; }
    }
}