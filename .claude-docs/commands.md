# Event commands: how they are built, and how to add one

How a command works inside the library, in what order things happen when a command is created, read, copied or put into a list, and the steps to add a
new command. The layouts of the parameters (what each command holds) are in [command-formats.md](command-formats.md); the values the editors give to a new
command are in [editor-defaults.md](editor-defaults.md). How to use commands (the builder, blocks, indents) is in the [README](../README.md).

## The model

- A **raw command** is what the file holds: a code, an indent and a list of parameters. `ICommand` (`Code`, `Indent`, `Parameters` as `List<object?>`).
  VX Ace: a Ruby object (`RPG::EventCommand`), MV: a JSON object. The raw classes are of the maker and come from the `TypeMap` of the storage
  (`Types.Factory<TCommand>()`).
- A **`NamedCommand`** is a *view* over a raw command. Its properties read and write `Raw.Parameters[i]` directly (`Get<T>(i)` / `Set(i, value)`), so
  nothing is unpacked, packed or kept in sync. Editing a property edits the file's data.
- The parameters are read through an `IParameterProvider`: `SimpleParameterProvider` for VX Ace (Ruby values), `JsonParameterProvider` for MV
  (JSON tokens are converted to the type asked for).
- A list of commands is a `CommandList<TCommand>` (a `Collection<NamedCommand>`) held by a container (event page, common event, troop page, move
  route). It builds the named commands over the raw ones when the file is read (`LoadRaw`), and gives the raw ones back when it is saved (`ToRaw`).
  A list that is absent in the file stays absent until a command is added.
- `CommandFactory.Create(raw, version)` picks the class by the **code** (the table is a `switch` in `Data/Structures/Commands/Factories`). A code the
  library does not know becomes `UnknownCommand(code)`, which keeps the raw parameters as they are. `If` (code 111) is picked by the first parameter
  (`IfCommand.IfType`), the lines that continue a command (text lines, script lines, ...) are classes of their own with `IsSecondary = true`.
- Movement routes have their own commands (`MovementRouteCommandCodes`, in `Commands/MovementCommands`, base `AbstractMoveCommand`) with their own factory.
- One list holds one maker's commands. A command that is in a list belongs to it (`Owner`): it cannot be put into another list or twice into the same
  one (`InvalidOperationException`); use `Clone()`.

## Three states of a command

| State | What `Raw` is | Parameters hold |
|---|---|---|
| **Loaded** | the raw command of the file | what the file has; nothing is added or changed |
| **Detached** (`new ChangeGoldCommand()`) | a private `DetachedCommand` | what was set in code, in a **neutral form** that does not depend on the maker (no maker is known yet) |
| **Made in code, bound** (the detached command was put into a list) | the raw command of the list's maker | the maker's form, completed like a file's command |

A command read from a file is **never** touched by the machinery below (`loaded: true`); only a detached command goes through it.

## What happens when a detached command is put into a list (`NamedCommand.Bind`)

The list calls `Bind(Types.Factory<TCommand>()(), version)`. In this order:

1. The indent and the parameters set in code are moved to the new raw command, and its `Code` is set.
2. **`ApplyEditorDefaults`**, unless the command is a `Clone()`: for every property with `[EditorDefault…]` attributes that applies to this maker, if
   the parameter was **not set in code** (`IsParameterSet(i)`: the value is non-null), the editor's value is written. See [editor-defaults.md](editor-defaults.md).
3. **`OnBinding(version)`**: the command turns what was set in code (the neutral form) into the form of `version`, and gives the values that an attribute
   cannot (those that depend on something else). Examples: Show Choices converts `Cancel` to VX Ace numbers; Change Equipment adds 1 to the slot for MV;
   Get Location Info maps the region number; If Button turns the button into the name MV uses.
4. **`CompleteMadeInCode`**: every parameter property is read and written back, so the raw command has *all* its parameters like a command of a file
   (an unset number becomes 0, an unset string `""`...). A property that returns an audio file (`IAudioFile`) and is null gets an empty one of the maker.
   The read of a property that the maker does not have is skipped (`HasProperty`).
5. **`OnMadeInCodeBound(version)`**: drops the parameters that the maker does not write (the 3rd of Change Class in VX Ace, the 4th of If Enemy "has
   appeared" in MV, the seconds of Control Timer "off", the unused parameters of Control Variable), and sets those that differ (the 2nd of Move Picture:
   0 in MV, nil in VX Ace).

`Clone()` copies the property values with their own objects (`Copies.Of`: sounds, tones, routes are duplicated), skips the properties that the clone's
maker will not have, and marks the copy `_copy` so step 2 is skipped: **a copy has what the original has, not what the editor gives to a new command**.

## The rules that keep this working

- **Properties are views.** A property is `get => Get<T>(i); set => Set(i, value);`. `Get`/`Set` remember the index (`_lastIndex`) and check
  `[DoesNotExistInVersion]`; that is how `ApplyEditorDefaults` finds which parameter a property is.
- **Neutral form.** A property of a detached command must accept and return the same thing for both makers. A property whose raw value differs between the makers
  converts it by the maker of the list (`IsInMv` / `Version`) when the command is in a list, and keeps the neutral form while it is detached. The
  conversion of the stored value happens once, in `OnBinding`.
- **`[RawValue]`** marks a property that is the raw value of another property (`CancelChoiceIndex` of Show Choices). The copy and the completion skip it,
  otherwise the raw number of one maker would overwrite the value made for the other.
- **`[DoesNotExistInVersion(StorageVersion.VXAce)]`** on a property of a command whose class is shared: reading or writing it in a list of that maker throws
  `DoesNotExistInVersionException`; `HasProperty(name)` tells without throwing. A detached command has the property; it is dropped when the command gets a
  maker that has not. A value (not a property) that one maker lacks is refused in `OnBinding` with `RequireVersion` or the exception.
- **An object parameter** (sound, tone, color, route) is stored as the class of the type map. `Set` converts an interface value to the class of the map
  (`Tones.For`, `Colors.For`, `MoveRoutes.For`, `AudioFiles.For`) when the command is in a list; a detached command keeps the value as it is. Use
  `GetEntity<T>(i)` to read them (it replaces the JSON object of MV with the class on the first read).
- **`IsParameterSet(i)`** is "the parameter is non-null", not "the user set it": that is how a default is told from a value. So a default is never
  applied over an explicit value, and an explicit `0` stays `0`.
- **`Materialize()`** reads every property once, so the objects in the parameters are objects of their classes and not the JSON objects of the file
  (used before a dump or a comparison).
- **`GetParametersDescription`** gives the text the editor shows for the command; the YAML/text dump is built from it and from `GetCommandName(version)`
  (the name can differ by maker).

## Adding a command

1. **The code**: a `const int` in `CommandCodes` (`Data/Enums/CommandCode.cs`); for a route command in `MovementRouteCommandCodes`.
2. **The class** in `Data/Structures/Commands`: `public override int Code => CommandCodes.X;`, `GetCommandName(version)`, the enums the parameters use (nested),
   one property per parameter with its index, and `GetParametersDescription`. Look at a neighbour with the same shape (`ChangeGoldCommand` for numbers,
   `ChangeBattleBGMCommand` for a sound, `ShowChoicesCommand` for a maker difference).
3. **The factory**: a `case` in `CommandFactory.Create` (or in `IfCommand.Factory`, or the route factory).
4. **Maker differences**, by what differs:
   - a parameter only one maker has: `[DoesNotExistInVersion(...)]` on the property, and drop it in `OnMadeInCodeBound` when the command gets the maker without it;
   - the same parameter written differently: a neutral value in the property, the conversion in `OnBinding`, the conversion by the list's maker in the
     property; mark the raw property `[RawValue]`;
   - a command that only one maker has: `OnMadeInCodeBound` throws for the other (`ChangeDefeatMECommand`, `ChangeVehicleBGMCommand`).
5. **The editor's values for a new command**: `[EditorDefault…]` on the property (see [editor-defaults.md](editor-defaults.md)), and probe the editor to find them.
   Never guess a default: write down which editor it was read from.
6. **Tests** (`tests/RPGMDataKit.Tests`): `EditorDefaultTests` checks every attribute by reflection (a new command class with attributes is covered when it has a
   public constructor); add the command to the meaning tests (`CommandMeaningTests`) if it has a maker difference; `RoundTripTests` and `HasPropertyTests` are
   the places for the loaded path and the maker-specific properties. Run the whole project: `dotnet test tests/RPGMDataKit.Tests`.
7. **Docs**: regenerate or edit the line of the command in [command-formats.md](command-formats.md); put what is new about a maker in its notes.

## Traps

- The order of the steps in `Bind` matters: a default is applied *before* `OnBinding`, so in `OnBinding` the value is the neutral one even when it is a default.
  A default set by an attribute on a property that converts by maker (`Cancel`) would be converted twice, so that one is given in `OnBinding`.
- A property that is only a view must not do work in its getter that changes the data (`CompleteMadeInCode` and `ApplyEditorDefaults` call getters).
- A property with both a getter and a setter is a parameter property: copy and completion call it. A read-only property (a convenience such as
  `SetMovementRouteCommand.Commands`) is skipped by them. Mind that a getter can answer differently from the raw value
  (`IncludeEquipment` is false unless the operator is Take) while the setter writes it as it is.
- Properties of a base class that is not `NamedCommand` itself are parameter properties too (`FindParameterProperties`: public, readable and writable,
  not declared in `NamedCommand`, not `[RawValue]`).
