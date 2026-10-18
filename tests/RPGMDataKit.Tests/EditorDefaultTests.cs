using System.Reflection;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// What the editors give to a new command (<see cref="EditorDefaultAttribute"/>). The values were read from the files that the editor of VX Ace
/// wrote for a command made with the dialog as it opens; for MV only what was seen there (the ids and counts of items, weapons, armors, and the
/// Show Choices) is given, and what is certainly wrong (a parameter that is null) is not left.
/// </summary>
public sealed class EditorDefaultTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static bool IsMv(Version v) => v.Name == "MV";

    private static T InList<T>(Version v, T command) where T : NamedCommand
    {
        EmptyCommandTests.EventOf(v).Append(b => b.Add(command));

        return command;
    }

    [Fact]
    public void ANewCommandOfVxAceHasTheValuesOfTheEditor()
    {
        var v = Version.VXAce;

        Assert.Equal(60, InList(v, new WaitCommand()).FrameCount);

        var enemy = InList(v, new ChangeEnemyHpCommand());
        Assert.Equal([-1, 0, 0, 1, 0], enemy.Raw.Parameters!.Select(Convert.ToInt32));

        var force = InList(v, new ForceActionCommand());
        Assert.Equal([0, 0, 1, -1], force.Raw.Parameters!.Select(Convert.ToInt32));

        var shake = InList(v, new ShakeScreenCommand());
        Assert.Equal([5, 5, 60, 1], shake.Raw.Parameters!.Select(Convert.ToInt32));

        var picture = InList(v, new ShowPictureCommand());
        Assert.Equal([1, 0, 0, 0, 0, 100, 100, 255, 0], picture.Raw.Parameters!.Where((_, i) => i != 1).Select(Convert.ToInt32));
        Assert.Equal("", picture.Picture);

        Assert.Equal("A", InList(v, new ControlSelfSwitchCommand()).Switch);
        Assert.Equal(9, InList(v, new SetWeatherEffectCommand()).Power);
    }

    [Fact]
    public void ANewWindowColorOfVxAceIsTheColorOfANewProject()
    {
        var tone = InList(Version.VXAce, new ChangeWindowColorCommand()).Tone;
        Assert.Equal((-34, 0, 68, 0), (tone.Red, tone.Green, tone.Blue, tone.Gray));

        var own = Version.VXAce.New<ITone>();
        own.Red = 10;
        var set = InList(Version.VXAce, new ChangeWindowColorCommand { Tone = own }).Tone;
        Assert.Equal((10, 0, 0), (set.Red, set.Green, set.Blue));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATintAndAFlashHaveTheirOwnToneAndColor(Version v)
    {
        var screen = InList(v, new TintScreenCommand()).Tone;
        var picture = InList(v, new TintPictureCommand()).Tone;
        var flash = InList(v, new FlashScreenCommand()).Color;

        Assert.Equal((0, 0, 0, 0), (screen.Red, screen.Green, screen.Blue, screen.Gray));
        Assert.Equal((0, 0, 0, 0), (picture.Red, picture.Green, picture.Blue, picture.Gray));
        Assert.Equal((255, 255, 255, 255), (flash.Red, flash.Green, flash.Blue, flash.Alpha));
        Assert.NotSame(screen, picture);   // each command has its own object
    }

    [Fact]
    public void WhatIsNotSeenInTheEditorOfMvIsNotGivenToMv()
    {
        Assert.Equal(0, InList(Version.Mv, new WaitCommand()).FrameCount);
        Assert.Equal(0, InList(Version.Mv, new ChangeEnemyHpCommand()).Value);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewCommandIsNotLeftWithNothingWhereTheEditorsWriteATextOrANumber(Version v)
    {
        // a null is never what an editor writes
        Assert.Equal("", InList(v, new ShowTextBaseCommand()).Face);
        Assert.Equal("", InList(v, new CommentCommand()).Comment);
        Assert.Equal("", InList(v, new ScriptCommand()).Script);

        var variable = InList(v, new ControlVariableCommand { Operand = ControlVariableCommand.OperandType.GameData });
        Assert.Equal([0, 0, 0], new[] { variable.Value1, variable.Value2, variable.Value3 }.Select(Convert.ToInt32));
        Assert.Equal(7, variable.Raw.Parameters!.Count);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ItemsWeaponsAndArmorsAreForTheFirstOneAndTheCountOneInBothMakers(Version v)
    {
        Assert.Equal((1, 1), (InList(v, new ChangeItemsCommand()).ItemId, InList(v, new ChangeItemsCommand()).Value));
        Assert.Equal((1, 1), (InList(v, new ChangeWeaponsCommand()).WeaponId, InList(v, new ChangeWeaponsCommand()).Value));
        Assert.Equal((1, 1), (InList(v, new ChangeArmorsCommand()).ArmorId, InList(v, new ChangeArmorsCommand()).Value));
    }

    [Fact]
    public void WhatIsSetInCodeStaysEvenWhenItIsZero()
    {
        var wait = InList(Version.VXAce, new WaitCommand { FrameCount = 0 });
        Assert.Equal(0, wait.FrameCount);

        var enemy = InList(Version.VXAce, new ChangeEnemyHpCommand { TroopId = 0, Value = 0 });
        Assert.Equal([0, 0, 0, 0, 0], enemy.Raw.Parameters!.Select(Convert.ToInt32));
    }

    [Fact]
    public void ACopyHasWhatTheOriginalHasAndNotWhatTheEditorGives()
    {
        // the original has no name (null), as a command of a file can have; the copy is the same
        var original = new ChangeNameCommand();
        var copy = (ChangeNameCommand)original.Clone();

        InList(Version.VXAce, copy);

        Assert.Equal(0, copy.ActorId);
        Assert.Null(copy.Name);
    }

    // every attribute names a property that holds one parameter, and a new command has the value: so the value goes to the right parameter
    [Theory]
    [MemberData(nameof(Versions))]
    public void EveryDefaultIsWhatANewCommandHasInTheMakerItIsFor(Version v)
    {
        var checkedProperties = 0;
        var types = typeof(NamedCommand).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(NamedCommand)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null);

        foreach (var type in types)
        {
            foreach (var property in type.GetProperties())
            {
                var defaults = property.GetCustomAttributes<EditorDefaultBaseAttribute>().ToArray();
                var attribute = defaults.FirstOrDefault(d => d.Versions.Any(x => x.ToString() == v.Name))
                                ?? defaults.FirstOrDefault(d => d.Versions.Count == 0);
                if (attribute == null)
                    continue;

                var command = InList(v, (NamedCommand)Activator.CreateInstance(type)!);
                if (!command.HasProperty(property.Name))
                    continue;

                AssertSame(attribute.CreateValue(v.Types, property.PropertyType), property.GetValue(command), $"{type.Name}.{property.Name}");
                checkedProperties++;
            }
        }

        Assert.True(checkedProperties >= (IsMv(v) ? 15 : 70), $"{checkedProperties} properties were checked");
    }

    private static void AssertSame(object? expected, object? actual, string what)
    {
        switch (expected)
        {
            case ITone tone:
                var gotTone = Assert.IsAssignableFrom<ITone>(actual);
                Assert.Equal((tone.Red, tone.Green, tone.Blue, tone.Gray), (gotTone.Red, gotTone.Green, gotTone.Blue, gotTone.Gray));
                break;
            case IColor color:
                var gotColor = Assert.IsAssignableFrom<IColor>(actual);
                Assert.Equal((color.Red, color.Green, color.Blue, color.Alpha), (gotColor.Red, gotColor.Green, gotColor.Blue, gotColor.Alpha));
                break;
            case IEnumerable<string> list:
                Assert.Equal(list, Assert.IsAssignableFrom<IEnumerable<string>>(actual));
                break;
            case string text:
                Assert.Equal(text, actual);
                break;
            case Enum:
            case bool:
                Assert.Equal(expected, actual);
                break;
            default:   // a number, or a number in a property of object
                Assert.Equal(Convert.ToInt32(expected), Convert.ToInt32(actual));
                break;
        }
    }
}
