using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <see cref="CommandBuilder"/> and what puts its commands into a container (<see cref="CommandContainerExtensions"/>). The lists are written as
/// "code@indent" of each command, the indents and the empty commands (0) being the ones that the editors make.
/// </summary>
public sealed class CommandBuilderTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static ICommonEvent Empty(Version v) => EmptyCommandTests.EventOf(v);

    private static string Shape(ICommandContainer container) =>
        string.Join(" ", container.NamedCommands.Select(c => $"{c.Code}@{c.Indent}"));

    /// <summary>The list is as expected and is a list that the editor could have made.</summary>
    private static void Expect(string expected, ICommandContainer container)
    {
        Assert.Equal(expected, Shape(container));
        IndentHelper.ValidateIndents(container);
        IndentHelper.ValidateStructure(container);
        EmptyCommandHelper.ValidateEmptyCommands(container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TextAndIfWithElse(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b
            .Text("a", "b")
            .IfSwitch(5, then: t => t.Text("on"), @else: e => e.Add(new WaitCommand { FrameCount = 30 })));

        Expect("101@0 401@0 401@0 111@0 101@1 401@1 0@1 411@0 230@1 0@1 412@0 0@0", container);
        var text = container.NamedCommands.OfType<ShowTextLineCommand>().Select(c => c.Text);
        Assert.Equal(["a", "b", "on"], text);
        Assert.Equal(5, container.NamedCommands.OfType<IfSwitchCommand>().Single().SwitchId);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TextIsCutIntoLinesAtTheLineBreaksAndTheCarriageReturnsAreDropped(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Text("a\r\nb", "c", "d\ne\r", "", "f\n"));

        // a, b | c | d, e | an empty line for "" | f and the empty one after its line break
        Expect("101@0 401@0 401@0 401@0 401@0 401@0 401@0 401@0 401@0 0@0", container);
        Assert.Equal(["a", "b", "c", "d", "e", "", "f", ""], container.NamedCommands.OfType<ShowTextLineCommand>().Select(c => c.Text));
        Assert.DoesNotContain(container.NamedCommands.OfType<ShowTextLineCommand>(), c => c.Text.Contains('\r') || c.Text.Contains('\n'));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATextWithAHeaderIsCutToo(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Text(new ShowTextBaseCommand { Face = "Actor1" }, "x\r\ny"));

        Assert.Equal(["x", "y"], container.NamedCommands.OfType<ShowTextLineCommand>().Select(c => c.Text));
        Assert.Equal("Actor1", container.NamedCommands.OfType<ShowTextBaseCommand>().Single().Face);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void BranchesThatAreEmptyAreJustTheEmptyCommand(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.IfSwitch(1));
        Expect("111@0 0@1 412@0 0@0", container);

        container.ReplaceCommands(b => b.IfSwitch(1, @else: _ => { }));
        Expect("111@0 0@1 411@0 0@1 412@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheShortcutsMakeTheConditionsOfTheirNames(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b
            .IfSwitch(3, state: OnOffState.Off)
            .IfVariable(4, IfVariableCommand.OperatorType.Gte, 10)
            .IfVariableWithVariable(5, IfVariableCommand.OperatorType.Lt, 6)
            .IfSelfSwitch("B")
            .IfScript("a == 1")
            .IfGold(IfGoldCommand.OperatorType.Gte, 100)
            .IfItem(7));

        Assert.Equal(7, container.NamedCommands.OfType<IfCommand>().Count());
        var off = container.NamedCommands.OfType<IfSwitchCommand>().Single();
        Assert.Equal((3, OnOffState.Off), (off.SwitchId, off.State));
        var variables = container.NamedCommands.OfType<IfVariableCommand>().ToList();
        Assert.Equal((4, IfVariableCommand.OperandType.Constant, 10), (variables[0].VariableId, variables[0].Operand, variables[0].Value));
        Assert.Equal((5, IfVariableCommand.OperandType.Variable, 6), (variables[1].VariableId, variables[1].Operand, variables[1].Value));
        Assert.Equal(IfVariableCommand.OperatorType.Lt, variables[1].Operator);
        Assert.Equal("B", container.NamedCommands.OfType<IfSelfSwitchCommand>().Single().Switch);
        Assert.Equal("a == 1", container.NamedCommands.OfType<IfScriptCommand>().Single().Script);
        Assert.Equal(100, container.NamedCommands.OfType<IfGoldCommand>().Single().Amount);
        Assert.Equal(7, container.NamedCommands.OfType<IfItemCommand>().Single().ItemId);
        Assert.Equal(6, container.NamedCommands.OfType<IfCommand>().Select(c => c.Type).Distinct().Count()); // Variable twice
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ChoicesWithTheirBranchesAndCancel(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Choices(c => c
            .When("Yes", y => y.Text("ok"))
            .When("No")
            .Cancel()));

        Expect("102@0 402@0 101@1 401@1 0@1 402@0 0@1 403@0 0@1 404@0 0@0", container);
        var choices = container.NamedCommands.OfType<ShowChoicesCommand>().Single();
        Assert.Equal(["Yes", "No"], choices.Choices);
        Assert.True(choices.IsCancelChoiceBranch);
        Assert.Equal(v.Name == "MV" ? -2 : 5, choices.CancelChoiceIndex);
        var cases = container.NamedCommands.OfType<ChoiceCaseCommand>().ToList();
        Assert.Equal([(0, "Yes"), (1, "No")], cases.Select(c => (c.ChoiceIndex, c.ChoiceName)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WithoutCancelTheChoicesDoNotAllowIt(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Choices(c => c.When("A").When("B")));

        Expect("102@0 402@0 0@1 402@0 0@1 404@0 0@0", container);
        var choices = container.NamedCommands.OfType<ShowChoicesCommand>().Single();
        Assert.False(choices.IsCancelChoiceBranch);
        Assert.Equal(v.Name == "MV" ? -1 : 0, choices.CancelChoiceIndex);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AShowChoicesOfTheEditorWithItsDefaultsIsMadeToTheLastParameter(Version v)
    {
        // what the editors make for a new Show Choices with Yes and No (both were looked at in the editors themselves): Cancel is the choice #2,
        // and in MV the first choice is the default one, the window is on the right and has a background
        var container = Empty(v);
        var header = new ShowChoicesCommand { Cancel = ShowChoicesCommand.CancelType.Choice2 };

        container.ReplaceCommands(b => b.Choices(c => c.When("Yes").When("No"), header));

        Expect("102@0 402@0 0@1 402@0 0@1 404@0 0@0", container);
        var parameters = container.NamedCommands[0].Raw.Parameters!;
        Assert.Equal(["Yes", "No"], (IEnumerable<string>)parameters[0]!);
        Assert.Equal(v.Name == "MV" ? [1, 0, 2, 0] : [2], parameters.Skip(1).Select(Convert.ToInt32).ToArray());
        Assert.Equal([0, 1], container.NamedCommands.OfType<ChoiceCaseCommand>().Select(c => c.ChoiceIndex));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ThePositionThatIsSetIsKept(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Choices(c => c.When("A"), new ShowChoicesCommand { Position = ShowChoicesCommand.PositionType.Left }));

        if (v.Name == "MV")
            Assert.Equal(ShowChoicesCommand.PositionType.Left, container.NamedCommands.OfType<ShowChoicesCommand>().Single().Position);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheRawParametersAreWhatTheEditorWrites(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Choices(c => c.When("A").Cancel()));

        var choices = container.NamedCommands.OfType<ShowChoicesCommand>().Single();
        Assert.Equal(v.Name == "MV" ? 5 : 2, choices.Raw.Parameters!.Count); // VX Ace has the choices and the cancel only
        var cancel = container.NamedCommands.OfType<ChoiceCaseCancelCommand>().Single();
        Assert.Equal(v.Name == "MV" ? 2 : 0, cancel.Raw.Parameters!.Count);
        if (v.Name == "MV")
            Assert.Equal(6, Convert.ToInt32(cancel.Raw.Parameters[0]));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void CancelOfTheHeaderIsKeptForAChoiceAndIsAnErrorForABranchWithoutOne(Version v)
    {
        var container = Empty(v);

        var header = new ShowChoicesCommand { Cancel = ShowChoicesCommand.CancelType.Choice2 }; // the second choice
        container.ReplaceCommands(b => b.Choices(c => c.When("A").When("B"), header));
        Assert.Equal(v.Name == "MV" ? 1 : 2, container.NamedCommands.OfType<ShowChoicesCommand>().Single().CancelChoiceIndex);

        var branchHeader = new ShowChoicesCommand();
        branchHeader.Cancel = ShowChoicesCommand.CancelType.Branch;
        Assert.Throws<ArgumentException>(() => new CommandBuilder(TypeMap.Mv).Choices(c => c.When("A"), branchHeader));
    }

    [Fact]
    public void ChoicesNeedAChoiceAndOneCancel()
    {
        Assert.Throws<ArgumentException>(() => new CommandBuilder(TypeMap.Mv).Choices(_ => { }));
        Assert.Throws<InvalidOperationException>(() => new CommandBuilder(TypeMap.Mv).Choices(c => c.When("A").Cancel().Cancel()));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void BattleHasTheBranchesThatAreGiven(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Battle(5, win: w => w.Add(new WaitCommand { FrameCount = 1 }), escape: _ => { }));
        Expect("301@0 601@0 230@1 0@1 602@0 0@1 604@0 0@0", container);
        var battle = container.NamedCommands.OfType<BattleProcessingCommand>().Single();
        Assert.Equal((5, true, false), (battle.SourceId, battle.CanEscape, battle.CanLoose));

        container.ReplaceCommands(b => b.Battle(5, lose: _ => { }));
        Expect("301@0 601@0 0@1 603@0 0@1 604@0 0@0", container);
        battle = container.NamedCommands.OfType<BattleProcessingCommand>().Single();
        Assert.Equal((false, true), (battle.CanEscape, battle.CanLoose));

        container.ReplaceCommands(b => b.Battle(5, escape: _ => { }, lose: _ => { }));
        Expect("301@0 601@0 0@1 602@0 0@1 603@0 0@1 604@0 0@0", container);

        container.ReplaceCommands(b => b.Battle(5));
        Expect("301@0 0@0", container);
    }

    [Fact]
    public void TheWinBranchAloneIsAnError() =>
        Assert.Throws<ArgumentException>(() => new CommandBuilder(TypeMap.Mv).Battle(5, win: _ => { }));

    [Theory]
    [MemberData(nameof(Versions))]
    public void ScrollingTextIsCutIntoLinesLikeText(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.ScrollingText("a\r\nb", "c"));

        Expect("105@0 405@0 405@0 405@0 0@0", container);
        var header = container.NamedCommands.OfType<ShowScrollingTextBaseCommand>().Single();
        Assert.Equal((2, false), (header.Speed, header.NoFastForward)); // what the editor gives to a new one
        Assert.Equal(["a", "b", "c"], container.NamedCommands.OfType<ShowScrollingTextLine>().Select(c => c.Text));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ACommandAfterATextIsRightAfterItAndTheEmptyCommandStaysAtTheEndOfTheList(Version v)
    {
        // a text is not a block: no empty command after its lines, only the one that ends the whole list
        var container = Empty(v);
        container.ReplaceCommands(b => b.ScrollingText("a", "b").Add(new WaitCommand()).Text("c"));
        Expect("105@0 405@0 405@0 230@0 101@0 401@0 0@0", container);

        // and the same when the commands are added in another call: they go before the empty command at the end
        container.AppendCommands(b => b.Add(new WaitCommand()));
        Expect("105@0 405@0 405@0 230@0 101@0 401@0 230@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATextInsertedIntoTheMiddleOfAListHasNoEmptyCommandAfterIt(Version v)
    {
        // Change Gold, Change Gold, Empty Command: a text at the index 1 goes between the two, and the one empty command stays at the end
        var container = Empty(v);
        container.NamedCommands.Add(new ChangeGoldCommand());
        container.NamedCommands.Add(new ChangeGoldCommand());
        container.NamedCommands.Add(new EmptyCommand());
        IndentHelper.SetupIndents(container); // the commands are made by hand, so they have no indents yet; the ones of a file have
        Expect("125@0 125@0 0@0", container);

        container.InsertCommands(1, b => b.ScrollingText("a"));

        Expect("125@0 105@0 405@0 125@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ScrollingTextKeepsTheSpeedOfItsHeader(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.ScrollingText(new ShowScrollingTextBaseCommand { Speed = 5, NoFastForward = true }, "x"));

        var header = container.NamedCommands.OfType<ShowScrollingTextBaseCommand>().Single();
        Assert.Equal((5, true), (header.Speed, header.NoFastForward));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheOtherConditionsHaveShortcutsToo(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b
            .IfWeapon(3, includeEquipment: true)
            .IfArmor(4)
            .IfTimer(60, IfTimerCommand.OperatorType.Lte)
            .IfEventFacing(2, Direction.Left)
            .IfVehicle(VehicleType.Airship)
            .IfActorInParty(1)
            .IfActorName(1, "Eric")
            .IfActorClass(1, 5)
            .IfActorSkill(1, 6)
            .IfActorWeapon(1, 7)
            .IfActorArmor(1, 8)
            .IfActorState(1, 9)
            .IfEnemyAppeared(0)
            .IfEnemyState(1, 10));

        var weapon = container.NamedCommands.OfType<IfWeaponCommand>().Single();
        Assert.Equal((3, true), (weapon.WeaponId, weapon.IncludeEquipment));
        var armor = container.NamedCommands.OfType<IfArmorCommand>().Single();
        Assert.Equal((4, false), (armor.ArmorId, armor.IncludeEquipment));
        var timer = container.NamedCommands.OfType<IfTimerCommand>().Single();
        Assert.Equal((60, IfTimerCommand.OperatorType.Lte), (timer.Seconds, timer.Operator));
        var facing = container.NamedCommands.OfType<IfEventCommand>().Single();
        Assert.Equal((2, Direction.Left), (facing.EventId, facing.Direction));
        Assert.Equal(VehicleType.Airship, container.NamedCommands.OfType<IfVehicleCommand>().Single().VehicleType);

        var actors = container.NamedCommands.OfType<IfActorCommand>().ToList();
        Assert.Equal(
            [
                (IfActorCommand.CheckType.InParty, 0), (IfActorCommand.CheckType.Name, "Eric"), (IfActorCommand.CheckType.Class, 5),
                (IfActorCommand.CheckType.Skill, 6), (IfActorCommand.CheckType.WeaponEquipped, 7), (IfActorCommand.CheckType.ArmorEquipped, 8),
                (IfActorCommand.CheckType.State, 9),
            ],
            actors.Select(a => (a.Check, (object)(a.Param is string text ? text : Convert.ToInt32(a.Param)))));
        Assert.All(actors, a => Assert.Equal(1, a.ActorId));

        var enemies = container.NamedCommands.OfType<IfEnemyCommand>().ToList();
        Assert.Equal([(IfEnemyCommand.CheckType.Appeared, 0, 0), (IfEnemyCommand.CheckType.State, 1, 10)], enemies.Select(e => (e.Check, e.TroopId, e.Param)));

        // every one of them is a condition with its branches
        Expect(string.Join(" ", Enumerable.Repeat("111@0 0@1 412@0", 14)) + " 0@0", container);
    }

    // ---- Set Movement Route ----

    private static List<int> RouteCodes(ICommandContainer container) =>
        container.NamedCommands.OfType<SetMovementRouteCommand>().Single().Route.NamedCommands.Select(c => c.Code).ToList();

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARouteIsOneCommandWithTheStepsAndTheEmptyCommandThatEndsIt(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(r => r.Down(2).Wait(30).TurnLeft()));

        Expect("205@0 0@0", container); // the steps are in the route, not in the list of the event
        var command = container.NamedCommands.OfType<SetMovementRouteCommand>().Single();
        Assert.Equal(SetMovementRouteCommand.PlayerEventId, command.EventId);
        Assert.Equal([1, 1, 15, 17, 0], RouteCodes(container));
        Assert.Equal(30, command.Route.NamedCommands.OfType<MoveWaitCommand>().Single().Frames);
        Assert.Equal((false, false, true), (command.Route.Repeat, command.Route.Skippable, command.Route.Wait)); // what the editors give
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheOptionsOfARouteAreKept(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(r => r.Forward(), eventId: 3, repeat: true, skippable: true, wait: false));

        var command = container.NamedCommands.OfType<SetMovementRouteCommand>().Single();
        Assert.Equal(3, command.EventId);
        Assert.Equal((true, true, false), (command.Route.Repeat, command.Route.Skippable, command.Route.Wait));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARouteWithNoStepsHasTheEmptyCommandOnly(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(_ => { }));

        Assert.Equal([0], RouteCodes(container));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewCommandOfARouteIsForThePlayerAndHasAnEmptyRoute(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Add(new SetMovementRouteCommand()));

        var command = container.NamedCommands.OfType<SetMovementRouteCommand>().Single();
        Assert.Equal(-1, command.EventId);
        Assert.Equal([0], RouteCodes(container));
        Assert.Equal((false, false, true), (command.Route.Repeat, command.Route.Skippable, command.Route.Wait));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheStepsGetTheValuesTheEditorGivesThem(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(r => r.Wait().Speed(MoveChangeSpeedCommand.SpeedType.Fasterx2)
            .Frequency(MoveChangeFrequencyCommand.FrequencyType.Higher).Opacity(128).Image("Actor1", 2).SwitchOn(5).Script("x = 1")));

        var steps = container.NamedCommands.OfType<SetMovementRouteCommand>().Single().Route.NamedCommands;
        Assert.Equal(60, steps.OfType<MoveWaitCommand>().Single().Frames);
        Assert.Equal(MoveChangeSpeedCommand.SpeedType.Fasterx2, steps.OfType<MoveChangeSpeedCommand>().Single().Speed);
        Assert.Equal(MoveChangeFrequencyCommand.FrequencyType.Higher, steps.OfType<MoveChangeFrequencyCommand>().Single().Frequency);
        Assert.Equal(128, steps.OfType<MoveChangeOpacityCommand>().Single().Opacity);
        var image = steps.OfType<MoveChangeImageCommand>().Single();
        Assert.Equal(("Actor1", 2), (image.Image, image.Offset));
        Assert.Equal(5, steps.OfType<MoveSwitchOnCommand>().Single().SwitchId);
        Assert.Equal("x = 1", steps.OfType<MoveScriptCommand>().Single().Script);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTurnsAndTheSwitchesOfACharacterHaveTheirCommands(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(r => r
            .Random().TowardPlayer().AwayFromPlayer().Jump(2, -1)
            .TurnDown().TurnRight().TurnUp().Turn90Right().Turn90Left().Turn180().Turn90RightOrLeft().TurnAtRandom().TurnTowardPlayer().TurnAwayFromPlayer()
            .SwitchOff(2).WalkingAnimation(true).WalkingAnimation(false).SteppingAnimation(true).SteppingAnimation(false)
            .DirectionFix(true).DirectionFix(false).Through(true).Through(false).Transparent(true).Transparent(false)
            .Blend(BlendMode.Add).LowerLeft().LowerRight().UpperLeft().UpperRight().Backward(2).Left().Right().Up()));

        Assert.Equal(
            [9, 10, 11, 14, 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 43, 5, 6, 7, 8, 13, 13, 2, 3, 4, 0],
            RouteCodes(container));
        var jump = container.NamedCommands.OfType<SetMovementRouteCommand>().Single().Route.NamedCommands.OfType<MoveJumpCommand>().Single();
        Assert.Equal((2, -1), (jump.XDelta, jump.YDelta));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASoundOfARouteKeepsItsValuesAndVxAceHasNoPan(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.MoveRoute(r => r.PlaySe("Absorb1", volume: 80, pitch: 120, pan: 30)));

        var sound = container.NamedCommands.OfType<SetMovementRouteCommand>().Single().Route.NamedCommands.OfType<MovePlaySeCommand>().Single().SoundData;
        Assert.Equal(("Absorb1", 80, 120), (sound.Name, sound.Volume, sound.Pitch));
        if (v.Name == "MV")
            Assert.Equal(30, sound.Pan);
        else
            Assert.Throws<DoesNotExistInVersionException>(() => sound.Pan);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARouteInABranchIsOneCommandOfTheBranch(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.IfSwitch(1, then: t => t.MoveRoute(r => r.Up())));

        Expect("111@0 205@1 0@1 412@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheRouteAndTheSoundAreMadeByTheTypeMapOfTheBuilder(Version v)
    {
        // the builder is made for a maker, so the objects in its commands are of the classes of that maker from the start
        var builder = new CommandBuilder(v.Types).MoveRoute(r => r.PlaySe("Absorb1", volume: 80));

        var command = builder.Commands.OfType<SetMovementRouteCommand>().Single();
        Assert.IsType(v.Types.Resolve(typeof(IMoveRoute)), command.Route);
        var sound = command.Route.NamedCommands.OfType<MovePlaySeCommand>().Single().SoundData;
        Assert.IsType(v.Types.Resolve(typeof(ISE)), sound);
        Assert.Equal(80, sound.Volume);
        Assert.Same(v.Types, builder.Types);
    }

    [Fact]
    public void ABuilderNeedsATypeMap() => Assert.Throws<ArgumentNullException>(() => new CommandBuilder(null!));

    [Fact]
    public void ABlendModeOfMvOnlyIsRefusedInAListOfVxAce()
    {
        var mv = Empty(Version.Mv);
        mv.ReplaceCommands(b => b.MoveRoute(r => r.Blend(BlendMode.Screen)));
        Assert.Contains(43, RouteCodes(mv));

        Assert.Throws<DoesNotExistInVersionException>(() => Empty(Version.VXAce).ReplaceCommands(b => b.MoveRoute(r => r.Blend(BlendMode.Screen))));
    }

    [Fact]
    public void TheValuesOfARouteMustBeInTheRangeOfTheEditor()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.Opacity(256)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.Opacity(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.PlaySe("x", volume: 101)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.PlaySe("x", pitch: 49)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.PlaySe("x", pitch: 151)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.PlaySe("x", pan: -101)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.PlaySe("x", pan: 101)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CommandBuilder(TypeMap.Mv).MoveRoute(r => r.Down(0)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AShopIsTheFirstThingAndTheLinesAfterIt(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Shop(s => s.Item(1).Weapon(4, price: 70).Armor(6), purchaseOnly: true));

        Expect("302@0 605@0 605@0 0@0", container); // a shop is not a block: no empty command between the lines
        var first = container.NamedCommands.OfType<ShopProcessingCommand>().Single();
        Assert.Equal((ShopProcessingCommand.ItemType.Item, 1, false, true), (first.Type, first.ItemId, first.HasCustomPrice, first.PurchaseOnly));
        Assert.Equal(5, first.Raw.Parameters!.Count);

        var lines = container.NamedCommands.OfType<ShopProcessingLineCommand>().ToList();
        Assert.Equal((ShopProcessingCommand.ItemType.Weapon, 4, true, 70), (lines[0].Type, lines[0].ItemId, lines[0].HasCustomPrice, lines[0].CustomPrice));
        Assert.Equal((ShopProcessingCommand.ItemType.Armor, 6, false, 0), (lines[1].Type, lines[1].ItemId, lines[1].HasCustomPrice, lines[1].CustomPrice));
        Assert.All(lines, line => Assert.Equal(4, line.Raw.Parameters!.Count));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AShopWithOneThingIsJustTheFirstCommand(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Shop(s => s.Item(9)));

        Expect("302@0 0@0", container);
        Assert.False(container.NamedCommands.OfType<ShopProcessingCommand>().Single().PurchaseOnly);
    }

    [Fact]
    public void AShopNeedsSomethingToSell() => Assert.Throws<ArgumentException>(() => new CommandBuilder(TypeMap.Mv).Shop(_ => { }));

    [Theory]
    [MemberData(nameof(Versions))]
    public void LoopWithBreak(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Loop(l => l.IfSwitch(2, then: t => t.BreakLoop())));

        Expect("112@0 111@1 113@2 0@2 412@1 0@1 413@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void CommentAndScriptHaveALineForEachString(Version v)
    {
        var container = Empty(v);

        container.ReplaceCommands(b => b.Comment("a", "b", "c").Script("x", "y"));

        Expect("108@0 408@0 408@0 355@0 655@0 0@0", container);
        Assert.Equal("a", container.NamedCommands.OfType<CommentCommand>().Single().Comment);
        Assert.Equal(["b", "c"], container.NamedCommands.OfType<CommentContinueCommand>().Select(c => c.Comment));
        Assert.Equal("x", container.NamedCommands.OfType<ScriptCommand>().Single().Script);
        Assert.Equal("y", container.NamedCommands.OfType<ScriptLineCommand>().Single().Text);
    }

    // ---- putting the commands into a list that has some ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void AppendGoesBeforeTheEmptyCommandThatEndsTheList(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230, 0);

        container.AppendCommands(b => b.IfSwitch(1));

        Expect("230@0 111@0 0@1 412@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AppendToAListWithTwoEmptyCommandsAtTheEndLeavesOne(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230, 0, 0);

        container.AppendCommands(b => b.Add(new WaitCommand()));

        Expect("230@0 230@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AppendToAListWithoutTheEndGivesItOne(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230);

        container.AppendCommands(b => b.Add(new WaitCommand()));

        Expect("230@0 230@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AppendToAnEmptyListMakesTheEndToo(Version v)
    {
        var container = Empty(v);

        container.AppendCommands(b => b.Add(new WaitCommand()));

        Expect("230@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void InsertAtTheEndOfABranchGoesIntoTheBranch(Version v)
    {
        var container = Empty(v);
        container.ReplaceCommands(b => b.IfSwitch(1, then: t => t.Add(new WaitCommand())));
        Expect("111@0 230@1 0@1 412@0 0@0", container);

        container.InsertCommands(3, b => b.Add(new WaitCommand())); // before Branch End

        Expect("111@0 230@1 230@1 0@1 412@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void InsertInTheMiddleOfAListKeepsTheRest(Version v)
    {
        var container = Empty(v);
        container.ReplaceCommands(b => b.Add(new WaitCommand()).IfSwitch(1, then: t => t.Add(new WaitCommand())));

        container.InsertCommands(0, b => b.Text("first"));
        Expect("101@0 401@0 230@0 111@0 230@1 0@1 412@0 0@0", container);

        container.InsertCommands(4, b => b.IfSwitch(2)); // before the Wait in the branch
        Expect("101@0 401@0 230@0 111@0 111@1 0@2 412@1 230@1 0@1 412@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABattleWithoutBranchesBeforeABranchOfAnotherOneIsNotTakenForItsBranch(Version v)
    {
        var container = Empty(v);
        container.ReplaceCommands(b => b.Battle(1, win: w => w.Battle(2), escape: _ => { }));
        Expect("301@0 601@0 301@1 0@1 602@0 0@1 604@0 0@0", container);

        container.InsertCommands(4, b => b.Battle(3)); // before the empty command that ends Win
        Expect("301@0 601@0 301@1 301@1 0@1 602@0 0@1 604@0 0@0", container);

        container.InsertCommands(5, b => b.Battle(4)); // at the end of Win, after the empty command of it
        Expect("301@0 601@0 301@1 301@1 301@1 0@1 602@0 0@1 604@0 0@0", container);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ReplaceMakesTheListNewAndInsertChecksTheIndex(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230, 230, 0);

        container.ReplaceCommands(b => b.Text("only"));
        Expect("101@0 401@0 0@0", container);

        Assert.Throws<ArgumentOutOfRangeException>(() => container.InsertCommands(-1, _ => { }));
        Assert.Throws<ArgumentOutOfRangeException>(() => container.InsertCommands(4, _ => { }));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ReplaceWithNothingLeavesTheEmptyCommand(Version v)
    {
        var container = EmptyCommandTests.EventOf(v, 230, 0);

        container.ReplaceCommands(_ => { });

        Expect("0@0", container);
    }

    // ---- random lists against what the editor makes ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void RandomTreesAreMadeAsTheEditorMakesThem(Version v)
    {
        var random = new Random(20261018);
        for (var round = 0; round < 300; ++round)
        {
            var expected = new List<(int Code, int Indent)>();
            var container = Empty(v);

            container.ReplaceCommands(b => Fill(random, b, expected, 0, 0));
            expected.Add((0, 0)); // the end of the list

            Assert.Equal(string.Join(" ", expected.Select(c => $"{c.Code}@{c.Indent}")), Shape(container));
            IndentHelper.ValidateIndents(container);
            IndentHelper.ValidateStructure(container);
            EmptyCommandHelper.ValidateEmptyCommands(container);
        }
    }

    // the commands of a branch go to the builder and, as they are made, to the list of what must come out
    private static void Branch(Random random, CommandBuilder b, List<(int, int)> expected, int indent, int depth)
    {
        Fill(random, b, expected, indent, depth + 1);
        expected.Add((0, indent));
    }

    private static void Fill(Random random, CommandBuilder b, List<(int, int)> expected, int indent, int depth)
    {
        for (var n = random.Next(0, 4); n > 0; --n)
        {
            switch (depth < 4 ? random.Next(0, 7) : 0)
            {
                case 1:
                {
                    expected.Add((111, indent));
                    var withElse = random.Next(2) == 0;
                    b.IfSwitch(1,
                        then: t => Branch(random, t, expected, indent + 1, depth),
                        @else: withElse
                            ? e =>
                            {
                                expected.Add((411, indent));
                                Branch(random, e, expected, indent + 1, depth);
                            }
                            : null);
                    expected.Add((412, indent));
                    break;
                }
                case 2:
                    expected.Add((112, indent));
                    b.Loop(l => Branch(random, l, expected, indent + 1, depth));
                    expected.Add((413, indent));
                    break;
                case 3:
                {
                    expected.Add((102, indent));
                    var whens = random.Next(1, 4);
                    var cancel = random.Next(2) == 0;
                    b.Choices(c =>
                    {
                        for (var i = 0; i < whens; i++)
                        {
                            c.When("w" + i, w =>
                            {
                                expected.Add((402, indent));
                                Branch(random, w, expected, indent + 1, depth);
                            });
                        }

                        if (cancel)
                        {
                            c.Cancel(w =>
                            {
                                expected.Add((403, indent));
                                Branch(random, w, expected, indent + 1, depth);
                            });
                        }
                    });
                    expected.Add((404, indent));
                    break;
                }
                case 4:
                {
                    expected.Add((301, indent));
                    var escape = random.Next(2) == 0;
                    var lose = random.Next(2) == 0;
                    if (!escape && !lose)
                    {
                        b.Battle(1);
                        break;
                    }

                    b.Battle(1,
                        win: w =>
                        {
                            expected.Add((601, indent));
                            Branch(random, w, expected, indent + 1, depth);
                        },
                        escape: escape
                            ? x =>
                            {
                                expected.Add((602, indent));
                                Branch(random, x, expected, indent + 1, depth);
                            }
                            : null,
                        lose: lose
                            ? x =>
                            {
                                expected.Add((603, indent));
                                Branch(random, x, expected, indent + 1, depth);
                            }
                            : null);
                    expected.Add((604, indent));
                    break;
                }
                case 5:
                    expected.Add((108, indent));
                    expected.Add((408, indent));
                    b.Comment("a", "b");
                    break;
                case 6:
                    expected.Add((101, indent));
                    expected.Add((401, indent));
                    expected.Add((401, indent));
                    b.Text("a", "b");
                    break;
                default:
                    expected.Add((230, indent));
                    b.Add(new WaitCommand());
                    break;
            }
        }
    }
}
