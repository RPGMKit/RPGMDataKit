using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The values that the editors give to a new entity (they were compared with the editors of VX Ace and of MV on real projects,
/// the numbers here are the ones that differ between the versions or that are easy to get wrong).
/// </summary>
public sealed class FactoryDefaultsTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    [Theory]
    [MemberData(nameof(Versions))]
    public void NewTextsAreEmptyAndNotNull(Version v)
    {
        var skill = v.New<ISkill>();
        Assert.Equal("", skill.Name);
        Assert.Equal("", skill.Note);
        Assert.Equal("", skill.Description);
        Assert.Equal("", skill.Message1);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void SkillIsUsedOnOneEnemy(Version v)
    {
        var skill = v.New<ISkill>();
        Assert.Equal(ItemScope.OneEnemy, skill.Scope);
        Assert.Equal(1, skill.STypeId);
        Assert.Equal(100, skill.SuccessRate);
        Assert.Equal(1, skill.Repeats);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ItemIsUsedOnOneAllyAndConsumed(Version v)
    {
        var item = v.New<IItem>();
        Assert.Equal(ItemScope.OneAlly, item.Scope);
        Assert.Equal(1, item.ITypeId);
        Assert.True(item.Consumable);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void DamageOfANewUsableItemIsNone(Version v)
    {
        var damage = v.New<ISkill>().Damage;
        Assert.Equal(DamageType.None, damage.Type);
        Assert.Equal("0", damage.Formula);
        Assert.Equal(20, damage.Variance);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WeaponHasTheTwoDefaultFeatures(Version v)
    {
        var features = ((IHasFeatures)v.New<IWeapon>()).Features;

        Assert.Equal(FeatureCodes.AttackElement, features[0].Code);
        Assert.Equal(1, features[0].DataId);
        Assert.Equal(FeatureCodes.ExtraParameter, features[1].Code);
        Assert.Equal(ExtraParameterIds.HitRate, features[1].DataId);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ArmorHasTheEvasionFeature(Version v)
    {
        var features = ((IHasFeatures)v.New<IArmor>()).Features;

        var feature = Assert.Single(features);
        Assert.Equal(FeatureCodes.ExtraParameter, feature.Code);
        Assert.Equal(ExtraParameterIds.Evasion, feature.DataId);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void EquipmentParamsAreEightZeros(Version v)
    {
        Assert.Equal(new int[8], v.New<IWeapon>().Params);
        Assert.Equal(new int[8], v.New<IArmor>().Params);
    }

    [Fact]
    public void TypesOfEquipmentCountFromDifferentNumbers()
    {
        // VX Ace: the weapon is 0, the first armor type is 1; MV: the weapon is 1, the first armor type is 2
        Assert.Equal(0, Version.VXAce.New<IWeapon>().ETypeId);
        Assert.Equal(1, Version.VXAce.New<IArmor>().ETypeId);
        Assert.Equal(1, Version.Mv.New<IWeapon>().ETypeId);
        Assert.Equal(2, Version.Mv.New<IArmor>().ETypeId);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void StateLastsOneTurnWithPriority50(Version v)
    {
        var state = v.New<IState>();
        Assert.Equal(50, state.Priority);
        Assert.Equal(1, state.MinTurns);
        Assert.Equal(1, state.MaxTurns);
        Assert.Equal(100, state.StepsToRemove);
        Assert.Equal(StateRestriction.None, state.Restriction);
        Assert.Empty(state.Features);
    }

    [Fact]
    public void ColorIsOpaqueWhiteAndToneIsNoChange()
    {
        foreach (var v in Version.All)
        {
            var color = v.New<IColor>();
            Assert.Equal([255, 255, 255, 255], new[] { color.Red, color.Green, color.Blue, color.Alpha });

            var tone = v.New<ITone>();
            Assert.Equal([0, 0, 0, 0], new[] { tone.Red, tone.Green, tone.Blue, tone.Gray });
        }
    }

    [Fact]
    public void EveryFactoryEntityIsCreatedWithoutAnException()
    {
        foreach (var v in Version.All)
            foreach (var entityInterface in v.FactoryInterfaces())
                Assert.NotNull(v.New(entityInterface));
    }
}
