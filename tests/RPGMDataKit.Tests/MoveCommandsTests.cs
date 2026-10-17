using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The commands of a movement route. The expected parameters are what the editors of VX Ace and MV write for a new command (the common event
/// "MoveCommands" of the test projects has every command of the route, made with the values the editors give).
/// </summary>
public sealed class MoveCommandsTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    /// <summary>The code, the command and the parameters of a new one.</summary>
    private static readonly (int Code, Func<NamedCommand> Make, object[] Parameters)[] Commands =
    [
        (1, () => new MoveDownCommand(), []),
        (2, () => new MoveLeftCommand(), []),
        (3, () => new MoveRightCommand(), []),
        (4, () => new MoveUpCommand(), []),
        (5, () => new MoveLowerLeftCommand(), []),
        (6, () => new MoveLowerRightCommand(), []),
        (7, () => new MoveUpperLeftCommand(), []),
        (8, () => new MoveUpperRightCommand(), []),
        (9, () => new MoveRandomCommand(), []),
        (10, () => new MoveTowardPlayerCommand(), []),
        (11, () => new MoveAwayFromPlayerCommand(), []),
        (12, () => new MoveForwardCommand(), []),
        (13, () => new MoveBackwardCommand(), []),
        (14, () => new MoveJumpCommand(), [0, 0]),
        (15, () => new MoveWaitCommand(), [60]),
        (16, () => new MoveTurnDownCommand(), []),
        (17, () => new MoveTurnLeftCommand(), []),
        (18, () => new MoveTurnRightCommand(), []),
        (19, () => new MoveTurnUpCommand(), []),
        (20, () => new MoveTurn90DegreesRightCommand(), []),
        (21, () => new MoveTurn90DegreesLeftCommand(), []),
        (22, () => new MoveTurn180DegreesCommand(), []),
        (23, () => new MoveTurn90DegreesRightOrLeftCommand(), []),
        (24, () => new MoveTurnAtRandomCommand(), []),
        (25, () => new MoveTurnTowardPlayerCommand(), []),
        (26, () => new MoveTurnAwayFromPlayerCommand(), []),
        (27, () => new MoveSwitchOnCommand(), [1]),
        (28, () => new MoveSwitchOffCommand(), [1]),
        (29, () => new MoveChangeSpeedCommand(), [4]),
        (30, () => new MoveChangeFrequencyCommand(), [3]),
        (31, () => new MoveWalkingAnimationOnCommand(), []),
        (32, () => new MoveWalkingAnimationOffCommand(), []),
        (33, () => new MoveSteppingAnimationOnCommand(), []),
        (34, () => new MoveSteppingAnimationOffCommand(), []),
        (35, () => new MoveDirectionFixOnCommand(), []),
        (36, () => new MoveDirectionFixOffCommand(), []),
        (37, () => new MoveThroughOnCommand(), []),
        (38, () => new MoveThroughOffCommand(), []),
        (39, () => new MoveTransparentOnCommand(), []),
        (40, () => new MoveTransparentOffCommand(), []),
        (41, () => new MoveChangeImageCommand(), ["", 0]),
        (42, () => new MoveChangeOpacityCommand(), [255]),
        (43, () => new MoveChangeBlendModeCommand(), [0]),
        (45, () => new MoveScriptCommand(), [""]),
        (0, () => new MoveEmptyCommand(), []),
    ];

    public static IEnumerable<object[]> Cases() =>
        Version.All.SelectMany(v => Commands.Select(c => new object[] { v, c.Code, c.Make().GetType().Name }));

    private static IMoveRoute RouteOf(Version v) => v.Types.New<IMoveRoute>();

    private static T InRoute<T>(Version v, T command) where T : NamedCommand
    {
        var route = RouteOf(v);
        route.NamedCommands.Clear();
        route.NamedCommands.Add(command);

        return command;
    }

    private static object? Plain(object? value) => value switch
    {
        Enum e => Convert.ToInt32(e),
        _ => value,
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ANewCommandHasTheCodeAndTheParametersTheEditorGivesIt(Version v, int code, string name)
    {
        var row = Commands.Single(c => c.Code == code);
        Assert.Equal(name, row.Make().GetType().Name);
        var command = InRoute(v, row.Make());

        Assert.Equal(code, command.Raw.Code);
        Assert.Equal(row.Parameters, (command.Raw.Parameters ?? []).Select(p => Plain(p) is int i ? (object)i : p).ToArray());
    }

    // ---- the values that the editors give ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void AValueThatIsSetIsNotReplacedByTheDefaultOne(Version v)
    {
        // 0 is a value like any other: the defaults only fill what was not set
        Assert.Equal(0, InRoute(v, new MoveWaitCommand { Frames = 0 }).Frames);
        Assert.Equal(0, InRoute(v, new MoveChangeOpacityCommand { Opacity = 0 }).Opacity);
        Assert.Equal(7, InRoute(v, new MoveSwitchOnCommand { SwitchId = 7 }).SwitchId);
        Assert.Equal("Actor1", InRoute(v, new MoveChangeImageCommand { Image = "Actor1", Offset = 3 }).Image);
        Assert.Equal(-2, InRoute(v, new MoveJumpCommand { XDelta = -2, YDelta = 5 }).XDelta);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void SpeedAndFrequencyAreEnumerationsWrittenAsNumbers(Version v)
    {
        var speed = InRoute(v, new MoveChangeSpeedCommand { Speed = MoveChangeSpeedCommand.SpeedType.Fasterx4 });
        var frequency = InRoute(v, new MoveChangeFrequencyCommand { Frequency = MoveChangeFrequencyCommand.FrequencyType.Highest });

        Assert.Equal(MoveChangeSpeedCommand.SpeedType.Fasterx4, speed.Speed);
        Assert.Equal(6, Convert.ToInt32(speed.Raw.Parameters![0]));
        Assert.Equal(MoveChangeFrequencyCommand.FrequencyType.Highest, frequency.Frequency);
        Assert.Equal(5, Convert.ToInt32(frequency.Raw.Parameters![0]));
    }

    private static string Describe(Version v, NamedCommand command)
    {
        var project = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);
        Storage storage = v.Name == "MV" ? new MVStorage(project, "data") : new VXAceStorage(project, "Data");

        return command.GetParametersDescription(new EntityContext(storage), new DumpOptions());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTextDumpShowsTheNumbersOfSpeedAndFrequency(Version v)
    {
        Assert.Equal("5", Describe(v, InRoute(v, new MoveChangeSpeedCommand { Speed = MoveChangeSpeedCommand.SpeedType.Fasterx2 })));
        Assert.Equal("1", Describe(v, InRoute(v, new MoveChangeFrequencyCommand { Frequency = MoveChangeFrequencyCommand.FrequencyType.Lowest })));
        Assert.Equal("4", Describe(v, InRoute(v, new MoveChangeSpeedCommand())));
        Assert.Equal("3", Describe(v, InRoute(v, new MoveChangeFrequencyCommand())));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewSoundIsEmptyWithTheValuesOfTheFactoryOfTheMaker(Version v)
    {
        var sound = InRoute(v, new MovePlaySeCommand()).SoundData;

        Assert.Equal("", sound.Name);
        Assert.Equal(100, sound.Pitch);
        if (v.Name == "MV")
        {
            Assert.Equal(90, sound.Volume);
            Assert.Equal(0, sound.Pan);
        }
    }

    [Fact]
    public void ABlendModeThatMvOnlyHasIsRefusedInVxAce()
    {
        Assert.Equal(3, Convert.ToInt32(InRoute(Version.Mv, new MoveChangeBlendModeCommand { BlendMode = BlendMode.Screen }).Raw.Parameters![0]));
        Assert.ThrowsAny<Exception>(() => InRoute(Version.VXAce, new MoveChangeBlendModeCommand()).BlendMode = BlendMode.Screen);
    }
}
