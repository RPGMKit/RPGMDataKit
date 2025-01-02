using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::UsableItem")]
public class UsableItem : BaseItem
{
    [RubyObject("RPG::UsableItem::Effect")]
    public class Effect : RubySerializationEntity, IEffect
    {
        [RubyProperty("@code")]
        public int Code { get; set; }

        [RubyProperty("@data_id")]
        public int DataId { get; set; }

        [RubyProperty("@value1")]
        public object Value1 { get; set; }

        [RubyProperty("@value2")]
        public object Value2 { get; set; }
    }

    [RubyObject("RPG::UsableItem::Damage")]
    public class ItemDamage : RubySerializationEntity, IItemDamage
    {
        [RubyProperty("@type")]
        public int Type { get; set; }
        [RubyProperty("@element_id")]
        public int ElementId { get; set; }
        [RubyProperty("@formula")]
        public string Formula { get; set; }
        [RubyProperty("@variance")]
        public int Variance { get; set; }
        [RubyProperty("@critical")]
        public bool Critical { get; set; }
    }

    [RubyProperty("@scope")]
    public int Scope;
    [RubyProperty("@occasion")]
    public int Occasion;
    [RubyProperty("@speed")]
    public int Speed;
    [RubyProperty("@animation_id")]
    public int AnimationId;
    [RubyProperty("@success_rate")]
    public int SuccessRate;
    [RubyProperty("@repeats")]
    public int Repeats;
    [RubyProperty("@tp_gain")]
    public int TpGain;
    [RubyProperty("@hit_type")]
    public int HitType;
    [RubyProperty("@damage")]
    public ItemDamage Damage;
    [RubyProperty("@effects")]
    public List<Effect> Effects;
}