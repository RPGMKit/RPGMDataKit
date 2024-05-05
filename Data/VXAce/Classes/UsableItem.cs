using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

[RubyObject("RPG::UsableItem")]
public class UsableItem : BaseItem
{
    [RubyObject("RPG::UsableItem::Effect")]
    public class Effect : RubySerializationEntity
    {
        [RubyProperty("@code")]
        public int Code;

        [RubyProperty("@data_id")]
        public int DataId;

        [RubyProperty("@value1")]
        public DynamicProperty<int, double> Value1;

        [RubyProperty("@value2")]
        public DynamicProperty<int, double> Value2;
    }

    [RubyObject("RPG::UsableItem::Damage")]
    public class ItemDamage : RubySerializationEntity
    {
        [RubyProperty("@type")]
        public int Type;
        [RubyProperty("@element_id")]
        public int ElementId;
        [RubyProperty("@formula")]
        public string Formula;
        [RubyProperty("@variance")]
        public int Variance;
        [RubyProperty("@critical")]
        public bool Critical;
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