using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.SpecialTypes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::UsableItem")]
public class UsableItem : BaseItem
{
    [RubyObject("RPG::UsableItem::Effect")]
    public class Effect : RubySerializationEntity, IEffect, INewEntity
    {
        /// <summary>What the editor of VX Ace puts into an effect that is added and not changed: "Recover HP" with 0 and 0 (the numbers are floats).</summary>
        public void InitializeNew()
        {
            Code = 11;
            DataId = 1;
            Value1 = 0.0;
            Value2 = 0.0;
        }

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
    public class ItemDamage : RubySerializationEntity, IItemDamage, INewEntity
    {
        /// <summary>The values of <c>RPG::UsableItem::Damage.new</c> of VX Ace: no damage, the formula 0, the variance of 20%.</summary>
        public void InitializeNew()
        {
            Type = 0;
            ElementId = 0;
            Formula = "0";
            Variance = 20;
            Critical = false;
        }

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

    /// <summary>The values of <c>RPG::UsableItem.new</c> of VX Ace.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = 0;
        Occasion = 0;
        Speed = 0;
        AnimationId = 0;
        SuccessRate = 100;
        Repeats = 1;
        TpGain = 0;
        HitType = 0;
        Damage = new ItemDamage();
        Damage.InitializeNew();
        Effects = new();
    }
}