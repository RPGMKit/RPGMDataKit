using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>What the entities of the database share, in both makers.</summary>
public sealed class HierarchyTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    [Theory]
    [MemberData(nameof(Versions))]
    public void DatabaseEntitiesAreBaseItems(Version v)
    {
        Assert.IsAssignableFrom<IBaseItem>(v.New<IActor>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IClass>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<ISkill>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IItem>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IWeapon>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IArmor>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IEnemy>());
        Assert.IsAssignableFrom<IBaseItem>(v.New<IState>());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void SkillsAndItemsAreUsableItems(Version v)
    {
        Assert.IsAssignableFrom<IUsableItem>(v.New<ISkill>());
        Assert.IsAssignableFrom<IUsableItem>(v.New<IItem>());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WeaponsAndArmorsAreEquipItems(Version v)
    {
        Assert.IsAssignableFrom<IEquipItem>(v.New<IWeapon>());
        Assert.IsAssignableFrom<IEquipItem>(v.New<IArmor>());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void EntitiesWithFeaturesExposeThem(Version v)
    {
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IActor>());
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IClass>());
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IWeapon>());
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IArmor>());
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IEnemy>());
        Assert.IsAssignableFrom<IHasFeatures>(v.New<IState>());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void IconsAndDescriptionsAreWhereTheEditorHasThem(Version v)
    {
        Assert.IsAssignableFrom<IHasIcon>(v.New<ISkill>());
        Assert.IsAssignableFrom<IHasIcon>(v.New<IItem>());
        Assert.IsAssignableFrom<IHasIcon>(v.New<IWeapon>());
        Assert.IsAssignableFrom<IHasIcon>(v.New<IArmor>());
        Assert.IsAssignableFrom<IHasIcon>(v.New<IState>());

        Assert.IsAssignableFrom<IHasDescription>(v.New<ISkill>());
        Assert.IsAssignableFrom<IHasDescription>(v.New<IItem>());
        Assert.IsAssignableFrom<IHasDescription>(v.New<IWeapon>());
        Assert.IsAssignableFrom<IHasDescription>(v.New<IArmor>());
    }

    [Fact]
    public void StateOfMvHasNoDescription()
    {
        // the editor of MV has no such field, a new state does not get one (VX Ace keeps it in every BaseItem)
        Assert.False(Version.Mv.New<IState>() is IHasDescription);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void FeaturesOfTheInterfaceAreTheListOfTheVersion(Version v)
    {
        var weapon = v.New<IWeapon>();

        var features = ((IHasFeatures)weapon).Features;

        Assert.Equal(2, features.Count);
        Assert.Equal(v.Name == "MV" ? ((Data.MV.Classes.Weapon)weapon).Traits.Count : ((Data.VXAce.Classes.Weapon)weapon).Features.Count,
            features.Count);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void UsableItemListsAreTheListsOfTheVersion(Version v)
    {
        var skill = v.New<ISkill>();

        Assert.NotNull(skill.Damage);
        Assert.Empty(skill.Effects);
    }
}
