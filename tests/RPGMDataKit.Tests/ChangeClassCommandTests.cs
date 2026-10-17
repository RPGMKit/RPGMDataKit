using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// Change Class has "Save Level" in the editor of MV only; the editor of VX Ace writes the actor and the class, nothing more.
/// </summary>
public sealed class ChangeClassCommandTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static ChangeClassCommand Made(Version v, ChangeClassCommand command)
    {
        var container = EmptyCommandTests.EventOf(v);
        container.Append(b => b.Add(command));

        return command;
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewCommandHasTheParametersOfItsMaker(Version v)
    {
        var command = Made(v, new ChangeClassCommand { ActorId = 15, ClassId = 30 });

        // MV: the actor, the class and false for Save Level, as the editor writes it
        Assert.Equal(v.Name == "MV" ? 3 : 2, command.Raw.Parameters!.Count);
        Assert.Equal(15, Convert.ToInt32(command.Raw.Parameters[0]));
        Assert.Equal(30, Convert.ToInt32(command.Raw.Parameters[1]));
        if (v.Name == "MV")
        {
            Assert.IsType<bool>(command.Raw.Parameters[2]);
            Assert.False(command.SaveLevel);
        }
    }

    [Fact]
    public void SaveLevelIsKeptInMv()
    {
        var command = Made(Version.Mv, new ChangeClassCommand { ActorId = 1, ClassId = 2, SaveLevel = true });

        Assert.True(command.SaveLevel);
        Assert.Equal(true, command.Raw.Parameters![2]);

        command.SaveLevel = false;
        Assert.Equal(false, command.Raw.Parameters[2]);
    }

    [Fact]
    public void VxAceHasNoSaveLevel()
    {
        var command = Made(Version.VXAce, new ChangeClassCommand { ActorId = 1, ClassId = 2 });

        Assert.Throws<DoesNotExistInVersionException>(() => command.SaveLevel);
        Assert.Throws<DoesNotExistInVersionException>(() => command.SaveLevel = true);
    }

    [Fact]
    public void ACommandOfMvWithSaveLevelPutIntoVxAceLosesIt()
    {
        var command = new ChangeClassCommand { ActorId = 1, ClassId = 2, SaveLevel = true };

        var inVx = Made(Version.VXAce, command.Clone().Cast<ChangeClassCommand>());

        Assert.Equal(2, inVx.Raw.Parameters!.Count);
        Assert.Equal((1, 2), (inVx.ActorId, inVx.ClassId));
    }

    [Fact]
    public void ACommandOfMvCopiedToMvKeepsSaveLevel()
    {
        var original = Made(Version.Mv, new ChangeClassCommand { ActorId = 1, ClassId = 2, SaveLevel = true });

        var copy = Made(Version.Mv, original.Clone().Cast<ChangeClassCommand>());

        Assert.True(copy.SaveLevel);
    }
}
