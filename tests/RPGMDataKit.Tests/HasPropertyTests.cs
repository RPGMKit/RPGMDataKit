using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <c>HasProperty</c>: whether an entity has a property that its interface has. VX Ace has no pan of a sound, no battler of the side view for an actor
/// and no types of the equipment; its classes declare the properties (marked with <see cref="DoesNotExistInVersionAttribute"/>) and they throw.
/// </summary>
public sealed class HasPropertyTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASoundHasAPanInMvOnly(Version v)
    {
        var sound = v.New<ISE>();

        Assert.Equal(v.Name == "MV", sound.HasProperty(nameof(sound.Pan)));
        Assert.True(sound.HasProperty(nameof(sound.Volume)));
        Assert.True(sound.HasProperty(nameof(sound.Name)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheCheckAgreesWithWhatThePropertyDoes(Version v)
    {
        var sound = v.New<ISE>();

        if (sound.HasProperty(nameof(sound.Pan)))
        {
            sound.Pan = 20;
            Assert.Equal(20, sound.Pan);
        }
        else
        {
            Assert.Throws<DoesNotExistInVersionException>(() => sound.Pan = 20);
            Assert.Throws<DoesNotExistInVersionException>(() => sound.Pan);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnActorHasTheBattlerOfTheSideViewInMvOnlyAndTheSystemHasTheTypesOfTheEquipmentInMvOnly(Version v)
    {
        var actor = v.New<IActor>();
        var system = v.New<ISystem>();

        Assert.Equal(v.Name == "MV", actor.HasProperty(nameof(actor.BattlerName)));
        Assert.Equal(v.Name == "MV", system.HasProperty(nameof(system.EquipmentTypes)));
        Assert.True(actor.HasProperty(nameof(actor.Name)));
    }

    [Fact]
    public void ANameThatTheClassDoesNotHaveIsAMistake()
    {
        var sound = Version.Mv.New<ISE>();

        Assert.Throws<ArgumentException>(() => sound.HasProperty("Panning"));
        Assert.Throws<ArgumentException>(() => Version.VXAce.New<ISE>().HasProperty("Panning"));
    }
}
