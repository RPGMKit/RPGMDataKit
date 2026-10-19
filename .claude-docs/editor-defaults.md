# What a new command is given (editor defaults)

A command made in code (`new WaitCommand()`) and put into a list gets the values that **the editor of that maker** gives a new command, not zeros and
nulls: the item 1 and the count 1 for Change Items, 60 frames for Wait, the choices "Yes" and "No" for Show Choices. The user-facing summary is in the
README ("What a new command has"); this page is for the person who adds or changes a value. When it happens in the life of a command:
[commands.md](commands.md#what-happens-when-a-detached-command-is-put-into-a-list-namedcommandbind).

## The rule

- A default is **what the editor writes** when the dialog is opened and OK is pressed with nothing changed. It is read from the saved file, never guessed.
- A value that the user sets in code wins, **including 0 / false / ""** (`IsParameterSet`: a default is applied only to a parameter that is null).
- A copy (`Clone()`) is not given defaults: it has what the original has.
- Where the editor takes the value from the **project** (the map of Transfer Player, the sounds, the graphics, the name of a label) the library cannot know it. The
  choices that were made: the map is **1**; sounds and graphics are **empty** (`""`); the window color of VX Ace is `(-34, 0, 68)`, of MV `(0, 0, 0, 0)`.
- Where the editor will not create the command at all without a value (a Label or a Jump to Label with an empty name, a Shop Processing without goods) there is
  no default. The builder (`Shop`) refuses an empty shop.

## The attributes (`Data/Structures/Attributes`)

Put on the **property** of the command, one per maker or one for all (no makers = every maker; use it only for a value that both editors give, or where
`null` is never right: texts `""`, ids and counts 1):

| Attribute | Makes | Example |
|---|---|---|
| `[EditorDefault(value, params versions)]` | a number, bool, text or enum (an enum is made from the number) | `[EditorDefault(60)]` on `Wait.FrameCount`; `[EditorDefault(-1, StorageVersion.VXAce)]` + `[EditorDefault(-2, StorageVersion.MV)]` on `ForceAction.Target` |
| `[EditorDefaultTone(r, g, b, gray, params versions)]` | an `ITone` of the type map | `[EditorDefaultTone(-34, 0, 68, 0, StorageVersion.VXAce)]` |
| `[EditorDefaultColor(r, g, b, alpha, params versions)]` | an `IColor` | `[EditorDefaultColor(255, 255, 255, 170, StorageVersion.MV)]` (Flash Screen) |
| `[EditorDefaultList("a", "b")]` | a `List<string>` | `[EditorDefaultList("Yes", "No")]` on `ShowChoices.Choices` |

`EditorDefaultBaseAttribute.CreateValue(TypeMap, Type)` is the extension point (derive from it for a new kind of value). The choice: the attribute whose
`Versions` contains the maker of the list wins, else the one without versions; the property must exist for the maker (`HasProperty`).

`EditorDefaultTests.EveryDefaultIsWhatANewCommandHasInTheMakerItIsFor` walks every command class by reflection, makes it in a list of each maker and checks that
every property with an attribute holds the attribute's value. A new attribute on a class with a public parameterless constructor is covered at once.

### When an attribute is not enough (`OnBinding`)

A default that needs code goes into `OnBinding` and is given **only if `!IsParameterSet(i)`**:

| Command | The default | Why not an attribute |
|---|---|---|
| Show Choices | `Cancel` = the second choice (Choice2); in MV the window `Position` = Right | `Cancel` converts what it is given by the maker of the list; the value here is the neutral one, converted in the same method |
| Play SE, Play BGS | VX Ace: a sound with the volume **80** (the factory of VX Ace gives 100); MV: the factory's sound (volume 90, pan 0) | the sound is an object of the type map |
| Route command Play SE | a sound with the volume 80 in VX Ace, 90 in MV (`MovePlaySeCommand.DefaultVolume`) | the same |
| Set Movement Route | the player (`-1`), a route that is empty (only the empty command), not repeated, not skipped, waited for | an object |
| If Enemy "has appeared" | VX Ace writes a 4th parameter `1` | depends on the check type |
| Route commands: Switch ON/OFF (switch 1), Wait (60), Speed and Frequency (normal), Opacity (255), Image and Script (`""`), blend mode | the older way: set in `OnBinding`; they could be attributes | - |

Every audio command gets an **empty sound of its maker** if it has none (`CompleteMadeInCode`), because the editors never write nothing.

## The values that differ by maker

All the others are the same in both editors. The list is the attributes with makers on them:

| Command / property | VX Ace | MV |
|---|---|---|
| Flash Screen, color | (255, 255, 255, **255**) | (255, 255, 255, **170**) |
| Set Weather Effect | power **9**, duration (not set: 0) | power **5**, duration **60** |
| Name Input Processing, max characters | **6** | **8** |
| Force Action, target | **-1** (Random) | **-2** (Last Target) |
| Show Battle Animation | troop member **-1** (entire troop) | enemy 0, and the third parameter `EntireTroop` = **false** |
| Rotate Picture, speed | **5** | 0 |
| Change Parameter, Change Skill, the source id | **1** | 0 (the entire party) |
| Select Item, type | (no such parameter) | **Key Item** |
| Change Window Color | (-34, 0, 68, 0) | (0, 0, 0, 0) |
| Change Actor Images | no sideview battler | `SwBattler` = `""` |
| If Button | **Down** | **OK** |
| Audio: the volume of a new sound | 100 (BGM/ME); **80** in Play SE / Play BGS and the route SE | **90**, pan 0 |
| Show Choices | `[["Yes","No"], cancel = Choice2]` | `[["Yes","No"], cancel = Choice2, default 0, position Right, background 0]` |
| Conditional Branch in the editor | the dialog has "Set handling when conditions do not apply" **on** (an Else is created); the builder does not create it | "Create Else Branch" is **off** |

(Values taken from the editor of VX Ace and from the editors of MV that were available: an older one and 1.6.3. The Conditional Branch row is about the editor, not about a value in the command.)

## How the values were found

1. Open the project in the editor, make a common event, and add the command from the dialog **without changing anything** (OK as it opens). Do this for
   every command in one event, in the editor of each maker; save.
2. Read the saved file: `rvdata2` through the library (`VXAceStorage` + `CommonEvents.Get()`), `Data/CommonEvents.json` for MV.
3. Compare with `new XCommand()` put into a list of that maker (a throw-away harness: for every command class, print the raw parameters), command by command; the
   differences are either a missing default or a value that comes from the project.
4. After an editor changes (the MV editor 1.6.3 writes Show Battle Animation with 3 parameters; the older one wrote 2), check the commands again.

Things met on the way:

- The editor refuses OK for an empty name in Label / Jump to Label and for a Shop Processing without goods: there is no default for them.
- The key Enter does not press OK in these dialogs; the mouse does.
- Do not use a common event that belongs to the user to test; use your own.
- Reading values back from the UI is not reliable; read the saved file.
- `Control Variable`, `If Actor` / `If Enemy`, `Control Timer` write only the parameters they use: the library drops the others (`OnMadeInCodeBound`).

## Not probed / not known

- Label and Jump to Label (the name is required by the editors), Shop Processing (the goods are required).
- Break Loop and the commands without parameters have nothing to give.
- The values that come from the project: the map of Transfer Player / Set Vehicle Location (the library uses 1), the sounds, the graphics.
- The MV editor was read in two versions (the older one for the first pass, 1.6.3 later); a value that changed between them was taken from 1.6.3.
