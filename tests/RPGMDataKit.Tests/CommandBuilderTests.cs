using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Interfaces;
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
        var header = new ShowChoicesCommand { CancelChoiceIndex = v.Name == "MV" ? 1 : 2 };

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

        var header = new ShowChoicesCommand { CancelChoiceIndex = v.Name == "MV" ? 1 : 2 }; // the second choice
        container.ReplaceCommands(b => b.Choices(c => c.When("A").When("B"), header));
        Assert.Equal(v.Name == "MV" ? 1 : 2, container.NamedCommands.OfType<ShowChoicesCommand>().Single().CancelChoiceIndex);

        var branchHeader = new ShowChoicesCommand();
        branchHeader.SetCancelBranch();
        Assert.Throws<ArgumentException>(() => new CommandBuilder().Choices(c => c.When("A"), branchHeader));
    }

    [Fact]
    public void ChoicesNeedAChoiceAndOneCancel()
    {
        Assert.Throws<ArgumentException>(() => new CommandBuilder().Choices(_ => { }));
        Assert.Throws<InvalidOperationException>(() => new CommandBuilder().Choices(c => c.When("A").Cancel().Cancel()));
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
        Assert.Throws<ArgumentException>(() => new CommandBuilder().Battle(5, win: _ => { }));

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
