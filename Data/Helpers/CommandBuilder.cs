using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Commands.IfCommands;
using RPGMDataKit.Data.Structures.Commands.MovementCommands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Makes the commands of blocks (If, Loop, Show Choices, Battle Processing) and of commands of several lines (Text, Comment, Script) from what
/// the blocks hold, so that the commands that open, divide and close a block do not have to be put one by one. The commands go in the order
/// of the calls; a block takes the function that makes its commands:
/// <code>
/// container.AppendCommands(b => b
///     .Text("Hello", "again")
///     .IfSwitch(5, then: t => t.Text("On"), @else: e => e.Add(new WaitCommand { FrameCount = 30 }))
///     .Choices(c => c.When("Yes", y => y.Text("Fine")).When("No").Cancel()));
/// </code>
/// The builder makes the commands and the empty command that ends each branch; the indents (and the empty command that ends the list) are set when
/// the commands are put into a container (<see cref="CommandContainerExtensions"/>). The commands of one builder can be put into one container once.
/// </summary>
public sealed class CommandBuilder
{
    private readonly List<NamedCommand> _commands = [];

    /// <param name="types">
    /// The type map of the maker whose list the commands are for (the one of the storage, <c>TypeMap.VXAce</c> or <c>TypeMap.Mv</c>), by which the
    /// objects in the commands, such as a movement route or a sound, are made. The methods that put the commands into a container
    /// (<see cref="CommandContainerExtensions"/>) take it from the container.
    /// </param>
    public CommandBuilder(TypeMap types)
    {
        Types = types ?? throw new ArgumentNullException(nameof(types));
    }

    /// <summary>The type map of the maker that the commands are for.</summary>
    public TypeMap Types { get; }

    /// <summary>The commands that were made.</summary>
    public IReadOnlyList<NamedCommand> Commands => _commands;

    /// <summary>Any command, which is put as it is.</summary>
    public CommandBuilder Add(params NamedCommand[] commands)
    {
        foreach (var command in commands)
        {
            ArgumentNullException.ThrowIfNull(command);
            _commands.Add(command);
        }

        return this;
    }

    /// <summary>The commands of a branch and the empty command that ends it, as the editors make it.</summary>
    private void Branch(Action<CommandBuilder>? commands)
    {
        commands?.Invoke(this);
        Add(new EmptyCommand());
    }

    // ---- commands of several lines ----

    /// <summary>A text as the editor makes it (at the bottom, in a window, without a face); one line for each line of the strings, see <see cref="Text(ShowTextBaseCommand, string[])"/>.</summary>
    public CommandBuilder Text(params string[] lines) => Text(
        new ShowTextBaseCommand
        {
            Face = "",
            Position = ShowTextBaseCommand.PositionType.Bottom,
            Background = BackgroundType.Window,
        }, lines);

    /// <summary>
    /// A text with the face, the position and the background of <paramref name="header"/>. Each string is cut into lines at the line breaks
    /// (<c>\n</c>; the <c>\r</c> are dropped), and each line is a command of its own: <c>Text("a\r\nb", "c")</c> has three lines. A line break at the end
    /// of a string makes an empty line.
    /// </summary>
    public CommandBuilder Text(ShowTextBaseCommand header, params string[] lines)
    {
        Add(header);
        foreach (var line in LinesOf(lines))
            Add(new ShowTextLineCommand { Text = line });

        return this;
    }

    /// <summary>
    /// A text that scrolls up the screen, as the editor makes it (speed 2, it can be fast forwarded); one line for each line of the strings,
    /// see <see cref="Text(ShowTextBaseCommand, string[])"/>.
    /// </summary>
    public CommandBuilder ScrollingText(params string[] lines) =>
        ScrollingText(new ShowScrollingTextBaseCommand { Speed = 2, NoFastForward = false }, lines);

    /// <summary>A text that scrolls with the speed and the fast forward of <paramref name="header"/>.</summary>
    public CommandBuilder ScrollingText(ShowScrollingTextBaseCommand header, params string[] lines)
    {
        Add(header);
        foreach (var line in LinesOf(lines))
            Add(new ShowScrollingTextLine { Text = line });

        return this;
    }

    /// <summary>The lines of the strings: each string is cut at the line breaks (<c>\n</c>) and the <c>\r</c> are dropped.</summary>
    private static IEnumerable<string> LinesOf(string[] strings)
    {
        foreach (var text in strings)
        {
            ArgumentNullException.ThrowIfNull(text);
            foreach (var line in text.Replace("\r", "").Split('\n'))
                yield return line;
        }
    }

    /// <summary>A comment, one line for each string.</summary>
    public CommandBuilder Comment(params string[] lines)
    {
        Add(new CommentCommand { Comment = lines.FirstOrDefault() ?? "" });
        foreach (var line in lines.Skip(1))
            Add(new CommentContinueCommand { Comment = line });

        return this;
    }

    /// <summary>A script, one line for each string.</summary>
    public CommandBuilder Script(params string[] lines)
    {
        Add(new ScriptCommand { Script = lines.FirstOrDefault() ?? "" });
        foreach (var line in lines.Skip(1))
            Add(new ScriptLineCommand { Text = line });

        return this;
    }

    // ---- If ----

    /// <summary>
    /// If with any condition. <paramref name="else"/> makes the Else branch: the branch is there when the function is given, even if the function
    /// makes nothing. Both branches may be left out and are then empty.
    /// </summary>
    public CommandBuilder If(IfCommand condition, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null)
    {
        Add(condition);
        Branch(then);
        if (@else != null)
        {
            Add(new ElseCommand());
            Branch(@else);
        }

        return Add(new EndIfCommand());
    }

    public CommandBuilder IfSwitch(int switchId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null,
        OnOffState state = OnOffState.On) =>
        If(new IfSwitchCommand { SwitchId = switchId, State = state }, then, @else);

    /// <summary>The variable compared with a number.</summary>
    public CommandBuilder IfVariable(int variableId, IfVariableCommand.OperatorType @operator, int value,
        Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfVariableCommand
        {
            VariableId = variableId,
            Operator = @operator,
            Operand = IfVariableCommand.OperandType.Constant,
            Value = value,
        }, then, @else);

    /// <summary>The variable compared with another variable.</summary>
    public CommandBuilder IfVariableWithVariable(int variableId, IfVariableCommand.OperatorType @operator, int otherVariableId,
        Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfVariableCommand
        {
            VariableId = variableId,
            Operator = @operator,
            Operand = IfVariableCommand.OperandType.Variable,
            Value = otherVariableId,
        }, then, @else);

    /// <param name="selfSwitch">"A", "B", "C" or "D".</param>
    public CommandBuilder IfSelfSwitch(string selfSwitch, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null,
        OnOffState state = OnOffState.On) =>
        If(new IfSelfSwitchCommand { Switch = selfSwitch, State = state }, then, @else);

    public CommandBuilder IfScript(string script, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfScriptCommand { Script = script }, then, @else);

    public CommandBuilder IfGold(IfGoldCommand.OperatorType @operator, int amount, Action<CommandBuilder>? then = null,
        Action<CommandBuilder>? @else = null) =>
        If(new IfGoldCommand { Operator = @operator, Amount = amount }, then, @else);

    /// <summary>The party has the item.</summary>
    public CommandBuilder IfItem(int itemId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfItemCommand { ItemId = itemId }, then, @else);

    /// <summary>The party has the weapon; with <paramref name="includeEquipment"/> also when it is equipped.</summary>
    public CommandBuilder IfWeapon(int weaponId, bool includeEquipment = false, Action<CommandBuilder>? then = null,
        Action<CommandBuilder>? @else = null) =>
        If(new IfWeaponCommand { WeaponId = weaponId, IncludeEquipment = includeEquipment }, then, @else);

    /// <summary>The party has the armor; with <paramref name="includeEquipment"/> also when it is equipped.</summary>
    public CommandBuilder IfArmor(int armorId, bool includeEquipment = false, Action<CommandBuilder>? then = null,
        Action<CommandBuilder>? @else = null) =>
        If(new IfArmorCommand { ArmorId = armorId, IncludeEquipment = includeEquipment }, then, @else);

    /// <summary>The timer has run for at least (or at most) <paramref name="seconds"/>.</summary>
    public CommandBuilder IfTimer(int seconds, IfTimerCommand.OperatorType @operator = IfTimerCommand.OperatorType.Gte,
        Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfTimerCommand { Seconds = seconds, Operator = @operator }, then, @else);

    /// <summary>The event faces the direction (the engines: a negative number is the player, 0 is the event that runs the commands).</summary>
    public CommandBuilder IfEventFacing(int eventId, Direction direction, Action<CommandBuilder>? then = null,
        Action<CommandBuilder>? @else = null) =>
        If(new IfEventCommand { EventId = eventId, Direction = direction }, then, @else);

    public CommandBuilder IfVehicle(VehicleType vehicle, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfVehicleCommand { VehicleType = vehicle }, then, @else);

    // the conditions about an actor and an enemy have several kinds: one method for each

    public CommandBuilder IfActorInParty(int actorId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.InParty, 0, then, @else);

    public CommandBuilder IfActorName(int actorId, string name, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.Name, name, then, @else);

    public CommandBuilder IfActorClass(int actorId, int classId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.Class, classId, then, @else);

    public CommandBuilder IfActorSkill(int actorId, int skillId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.Skill, skillId, then, @else);

    /// <summary>The actor has the weapon equipped.</summary>
    public CommandBuilder IfActorWeapon(int actorId, int weaponId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.WeaponEquipped, weaponId, then, @else);

    /// <summary>The actor has the armor equipped.</summary>
    public CommandBuilder IfActorArmor(int actorId, int armorId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.ArmorEquipped, armorId, then, @else);

    public CommandBuilder IfActorState(int actorId, int stateId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        IfActor(actorId, IfActorCommand.CheckType.State, stateId, then, @else);

    private CommandBuilder IfActor(int actorId, IfActorCommand.CheckType check, object param, Action<CommandBuilder>? then,
        Action<CommandBuilder>? @else) =>
        If(new IfActorCommand { ActorId = actorId, Check = check, Param = param }, then, @else);

    /// <summary>The enemy of the troop has appeared; <paramref name="enemyIndex"/> is its place in the troop counted from 0 (the editors show it from 1).</summary>
    public CommandBuilder IfEnemyAppeared(int enemyIndex, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfEnemyCommand { TroopId = enemyIndex, Check = IfEnemyCommand.CheckType.Appeared, Param = 0 }, then, @else);

    public CommandBuilder IfEnemyState(int enemyIndex, int stateId, Action<CommandBuilder>? then = null, Action<CommandBuilder>? @else = null) =>
        If(new IfEnemyCommand { TroopId = enemyIndex, Check = IfEnemyCommand.CheckType.State, Param = stateId }, then, @else);

    // ---- Loop ----

    public CommandBuilder Loop(Action<CommandBuilder>? body = null)
    {
        Add(new LoopCommand());
        Branch(body);

        return Add(new EndLoopCommand());
    }

    public CommandBuilder BreakLoop() => Add(new BreakLoopCommand());

    // ---- Show Choices ----

    /// <summary>
    /// Show Choices: the choices are given with their branches (<see cref="ChoicesBuilder.When"/>), so there can be no branch without a choice and no
    /// choice without a branch. Cancel is a branch if <see cref="ChoicesBuilder.Cancel"/> is called and is not allowed otherwise.
    /// </summary>
    /// <param name="define">Makes the choices.</param>
    /// <param name="header">The command with the position, the background and the default choice; its choices and cancel are set here.</param>
    public CommandBuilder Choices(Action<ChoicesBuilder> define, ShowChoicesCommand? header = null)
    {
        var choices = new ChoicesBuilder();
        define(choices);
        if (choices.Options.Count == 0)
            throw new ArgumentException("Show Choices needs at least one choice.", nameof(define));

        header ??= new ShowChoicesCommand();
        header.Choices = choices.Options.Select(o => o.Text).ToList();
        if (choices.HasCancel)
            header.Cancel = ShowChoicesCommand.CancelType.Branch;
        else if (header.IsCancelChoiceBranch)
            throw new ArgumentException("Cancel of the command is a branch, but the choices have no Cancel.", nameof(header));

        Add(header);
        for (var i = 0; i < choices.Options.Count; i++)
        {
            Add(new ChoiceCaseCommand { ChoiceIndex = i, ChoiceName = choices.Options[i].Text });
            Branch(choices.Options[i].Branch);
        }

        if (choices.HasCancel)
        {
            Add(new ChoiceCaseCancelCommand());
            Branch(choices.CancelBranch);
        }

        return Add(new EndChoicesCommand());
    }

    // ---- Battle Processing ----

    /// <summary>
    /// Battle Processing of the troop. The branches that are given decide what the command can do: Escape sets "Can Escape", Lose sets
    /// "Continue Even When Loser". The Win branch is there whenever there is another one (the editors make it so) and is empty if
    /// <paramref name="win"/> is not given; <paramref name="win"/> alone is an error, since a battle with no other branch has none.
    /// </summary>
    public CommandBuilder Battle(int troopId, Action<CommandBuilder>? win = null, Action<CommandBuilder>? escape = null,
        Action<CommandBuilder>? lose = null) =>
        Battle(new BattleProcessingCommand { DsgType = BattleProcessingCommand.DesignationType.Direct, SourceId = troopId },
            win, escape, lose);

    /// <summary>The same for a command that says what the battle is (the troop by a variable, a random encounter).</summary>
    public CommandBuilder Battle(BattleProcessingCommand command, Action<CommandBuilder>? win = null,
        Action<CommandBuilder>? escape = null, Action<CommandBuilder>? lose = null)
    {
        if (win != null && escape == null && lose == null)
            throw new ArgumentException("Battle Processing has the Win branch only together with the Escape or the Lose branch.", nameof(win));

        command.CanEscape = escape != null;
        command.CanLoose = lose != null;
        Add(command);
        if (escape == null && lose == null)
            return this;

        Add(new BattleProcessingIfWinCommand());
        Branch(win);
        if (escape != null)
        {
            Add(new BattleProcessingIfEscapeCommand());
            Branch(escape);
        }

        if (lose != null)
        {
            Add(new BattleProcessingIfLoseCommand());
            Branch(lose);
        }

        return Add(new BattleProcessingEndCommand());
    }

    // ---- Set Movement Route ----

    /// <summary>
    /// Set Movement Route: the steps are given in the order they are walked (<see cref="RouteBuilder"/>). The empty command that ends every route
    /// is added. The commands of the route have the values the editor of the maker of the list gives them.
    /// </summary>
    /// <param name="define">Makes the steps of the route.</param>
    /// <param name="eventId">Whose route: -1 is the player (as the editors have it), 0 is the event that runs the commands, a positive number is the event.</param>
    /// <param name="repeat">Repeat Movement.</param>
    /// <param name="skippable">Skip If Cannot Move.</param>
    /// <param name="wait">Wait for Completion.</param>
    public CommandBuilder MoveRoute(Action<RouteBuilder> define, int eventId = SetMovementRouteCommand.PlayerEventId, bool repeat = false,
        bool skippable = false, bool wait = true)
    {
        var steps = new RouteBuilder(Types);
        define(steps);

        // the route is of the maker of the list, so its steps get the values that the editor of that maker gives them
        var route = Types.New<IMoveRoute>();
        route.Repeat = repeat;
        route.Skippable = skippable;
        route.Wait = wait;
        route.NamedCommands = [.. steps.Steps, new MoveEmptyCommand()];

        return Add(new SetMovementRouteCommand { EventId = eventId, Route = route });
    }

    /// <summary>The steps of <see cref="MoveRoute"/>.</summary>
    public sealed class RouteBuilder
    {
        private readonly TypeMap _types;

        internal RouteBuilder(TypeMap types)
        {
            _types = types;
        }

        internal List<NamedCommand> Steps { get; } = [];

        /// <summary>Any step, which is put as it is. The empty command that ends the route is not to be added: it is added by <see cref="MoveRoute"/>.</summary>
        public RouteBuilder Add(params NamedCommand[] steps)
        {
            foreach (var step in steps)
            {
                ArgumentNullException.ThrowIfNull(step);
                Steps.Add(step);
            }

            return this;
        }

        private RouteBuilder Add(NamedCommand step, int times)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(times, 1);
            Steps.Add(step);
            for (var i = 1; i < times; i++)
                Steps.Add(step.Clone());

            return this;
        }

        // ---- steps (a number of times) ----

        public RouteBuilder Down(int times = 1) => Add(new MoveDownCommand(), times);

        public RouteBuilder Left(int times = 1) => Add(new MoveLeftCommand(), times);

        public RouteBuilder Right(int times = 1) => Add(new MoveRightCommand(), times);

        public RouteBuilder Up(int times = 1) => Add(new MoveUpCommand(), times);

        public RouteBuilder LowerLeft(int times = 1) => Add(new MoveLowerLeftCommand(), times);

        public RouteBuilder LowerRight(int times = 1) => Add(new MoveLowerRightCommand(), times);

        public RouteBuilder UpperLeft(int times = 1) => Add(new MoveUpperLeftCommand(), times);

        public RouteBuilder UpperRight(int times = 1) => Add(new MoveUpperRightCommand(), times);

        public RouteBuilder Forward(int times = 1) => Add(new MoveForwardCommand(), times);

        public RouteBuilder Backward(int times = 1) => Add(new MoveBackwardCommand(), times);

        // ---- other movement ----

        public RouteBuilder Random() => Add(new MoveRandomCommand());

        public RouteBuilder TowardPlayer() => Add(new MoveTowardPlayerCommand());

        public RouteBuilder AwayFromPlayer() => Add(new MoveAwayFromPlayerCommand());

        /// <summary>Jump by the number of tiles to the right (x) and down (y); negative numbers are to the left and up.</summary>
        public RouteBuilder Jump(int x = 0, int y = 0) => Add(new MoveJumpCommand { XDelta = x, YDelta = y });

        /// <summary>Wait for the number of frames (60 are a second).</summary>
        public RouteBuilder Wait(int frames = 60) => Add(new MoveWaitCommand { Frames = frames });

        // ---- turns ----

        public RouteBuilder TurnDown() => Add(new MoveTurnDownCommand());

        public RouteBuilder TurnLeft() => Add(new MoveTurnLeftCommand());

        public RouteBuilder TurnRight() => Add(new MoveTurnRightCommand());

        public RouteBuilder TurnUp() => Add(new MoveTurnUpCommand());

        public RouteBuilder Turn90Right() => Add(new MoveTurn90DegreesRightCommand());

        public RouteBuilder Turn90Left() => Add(new MoveTurn90DegreesLeftCommand());

        public RouteBuilder Turn180() => Add(new MoveTurn180DegreesCommand());

        public RouteBuilder Turn90RightOrLeft() => Add(new MoveTurn90DegreesRightOrLeftCommand());

        public RouteBuilder TurnAtRandom() => Add(new MoveTurnAtRandomCommand());

        public RouteBuilder TurnTowardPlayer() => Add(new MoveTurnTowardPlayerCommand());

        public RouteBuilder TurnAwayFromPlayer() => Add(new MoveTurnAwayFromPlayerCommand());

        // ---- switches and the settings of the character ----

        public RouteBuilder SwitchOn(int switchId) => Add(new MoveSwitchOnCommand { SwitchId = switchId });

        public RouteBuilder SwitchOff(int switchId) => Add(new MoveSwitchOffCommand { SwitchId = switchId });

        public RouteBuilder Speed(MoveChangeSpeedCommand.SpeedType speed) => Add(new MoveChangeSpeedCommand { Speed = speed });

        public RouteBuilder Frequency(MoveChangeFrequencyCommand.FrequencyType frequency) =>
            Add(new MoveChangeFrequencyCommand { Frequency = frequency });

        public RouteBuilder WalkingAnimation(bool on) => on ? Add(new MoveWalkingAnimationOnCommand()) : Add(new MoveWalkingAnimationOffCommand());

        public RouteBuilder SteppingAnimation(bool on) => on ? Add(new MoveSteppingAnimationOnCommand()) : Add(new MoveSteppingAnimationOffCommand());

        public RouteBuilder DirectionFix(bool on) => on ? Add(new MoveDirectionFixOnCommand()) : Add(new MoveDirectionFixOffCommand());

        public RouteBuilder Through(bool on) => on ? Add(new MoveThroughOnCommand()) : Add(new MoveThroughOffCommand());

        public RouteBuilder Transparent(bool on) => on ? Add(new MoveTransparentOnCommand()) : Add(new MoveTransparentOffCommand());

        /// <summary>The image of the character: the name of the file (empty is none) and the place in it (0-7).</summary>
        public RouteBuilder Image(string name, int index = 0) => Add(new MoveChangeImageCommand { Image = name, Offset = index });

        /// <param name="opacity">From 0 to 255.</param>
        public RouteBuilder Opacity(int opacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(opacity);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(opacity, 255);

            return Add(new MoveChangeOpacityCommand { Opacity = opacity });
        }

        /// <summary>The blend mode of the character; Screen is MV only.</summary>
        public RouteBuilder Blend(BlendMode mode) => Add(new MoveChangeBlendModeCommand { BlendMode = mode });

        /// <summary>A sound. The volume is from 0 to 100, the pitch from 50 to 150, the pan (MV only, left to right) from -100 to 100.</summary>
        public RouteBuilder PlaySe(string name, int volume = 90, int pitch = 100, int pan = 0)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentOutOfRangeException.ThrowIfLessThan(volume, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(volume, 100);
            ArgumentOutOfRangeException.ThrowIfLessThan(pitch, 50);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(pitch, 150);
            ArgumentOutOfRangeException.ThrowIfLessThan(pan, -100);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(pan, 100);

            // the sound is of the maker of the list; VX Ace has no pan
            var sound = _types.New<ISE>();
            sound.Name = name;
            sound.Volume = volume;
            sound.Pitch = pitch;
            if (sound.HasProperty(nameof(sound.Pan)))
                sound.Pan = pan;

            return Add(new MovePlaySeCommand { SoundData = sound });
        }

        public RouteBuilder Script(string script) => Add(new MoveScriptCommand { Script = script });
    }

    // ---- Shop Processing ----

    /// <summary>
    /// Shop Processing: the goods are given in the order they are sold (<see cref="ShopBuilder.Item"/>, <see cref="ShopBuilder.Weapon"/>,
    /// <see cref="ShopBuilder.Armor"/>). The first one is the command of the shop and the others are the lines after it. A shop is not a block, so
    /// nothing follows the lines.
    /// </summary>
    /// <param name="define">Makes the goods; there has to be at least one.</param>
    /// <param name="purchaseOnly">The shop buys nothing.</param>
    public CommandBuilder Shop(Action<ShopBuilder> define, bool purchaseOnly = false)
    {
        var shop = new ShopBuilder();
        define(shop);
        if (shop.Goods.Count == 0)
            throw new ArgumentException("A shop needs at least one thing to sell.", nameof(define));

        var first = shop.Goods[0];
        Add(new ShopProcessingCommand
        {
            Type = first.Type,
            ItemId = first.Id,
            HasCustomPrice = first.Price != null,
            CustomPrice = first.Price ?? 0,
            PurchaseOnly = purchaseOnly,
        });
        foreach (var goods in shop.Goods.Skip(1))
        {
            Add(new ShopProcessingLineCommand
            {
                Type = goods.Type,
                ItemId = goods.Id,
                HasCustomPrice = goods.Price != null,
                CustomPrice = goods.Price ?? 0,
            });
        }

        return this;
    }

    /// <summary>The goods of <see cref="Shop"/>.</summary>
    public sealed class ShopBuilder
    {
        internal List<(ShopProcessingCommand.ItemType Type, int Id, int? Price)> Goods { get; } = [];

        /// <summary>An item. Without a price it is sold at the price in the database.</summary>
        public ShopBuilder Item(int itemId, int? price = null) => Add(ShopProcessingCommand.ItemType.Item, itemId, price);

        public ShopBuilder Weapon(int weaponId, int? price = null) => Add(ShopProcessingCommand.ItemType.Weapon, weaponId, price);

        public ShopBuilder Armor(int armorId, int? price = null) => Add(ShopProcessingCommand.ItemType.Armor, armorId, price);

        private ShopBuilder Add(ShopProcessingCommand.ItemType type, int id, int? price)
        {
            Goods.Add((type, id, price));

            return this;
        }
    }

    /// <summary>The choices of <see cref="Choices"/> with what each of them does.</summary>
    public sealed class ChoicesBuilder
    {
        internal List<(string Text, Action<CommandBuilder>? Branch)> Options { get; } = [];

        internal bool HasCancel { get; private set; }

        internal Action<CommandBuilder>? CancelBranch { get; private set; }

        /// <summary>A choice and the commands that it runs; no commands makes an empty branch.</summary>
        public ChoicesBuilder When(string text, Action<CommandBuilder>? branch = null)
        {
            ArgumentNullException.ThrowIfNull(text);
            Options.Add((text, branch));

            return this;
        }

        /// <summary>Cancel is a branch of its own (When Cancel), with the commands that it runs, if any.</summary>
        public ChoicesBuilder Cancel(Action<CommandBuilder>? branch = null)
        {
            if (HasCancel)
                throw new InvalidOperationException("Cancel is already set.");

            HasCancel = true;
            CancelBranch = branch;

            return this;
        }
    }
}
