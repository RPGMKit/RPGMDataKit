# Events, pages, common events and troops

What the three places that hold event commands look like in the data, what each field means to the engine, and what the library gives. The commands themselves are in
[commands.md](commands.md) and [command-formats.md](command-formats.md). The engine facts are from `rpg_objects.js` of MV 1.6.1 and the stock scripts of VX Ace (the two
agree unless a difference is written).

## Where commands live

| Holder | Interface | Where in the data | Commands |
|---|---|---|---|
| a map event page | `IEventPage` (in `IEvent.Pages`, in `IMap.Events`) | `Map001`: the event's `pages[]` | `page.NamedCommands` |
| a common event | `ICommonEvent` (`Storage.CommonEvents`) | `CommonEvents` | `commonEvent.NamedCommands` |
| a troop page | `ITroopPage` (in `ITroop.Pages`, `Storage.Troops`) | `Troops` | `page.NamedCommands` |
| a movement route | `IMoveRoute` (in a page, and in Set Movement Route) | `moveRoute` / `@move_route` | `route.NamedCommands` (route commands) |

All four are `ICommandContainer`s: `NamedCommands` is a list of `NamedCommand`; assigning it replaces every command. A list always ends with the empty command (code 0, indent 0).

## Map events

`IMap.Events` is an `IDictionary<int, IEvent?>`: the key is the event id, and `IEvent.Id` is the same number. In MV the file is an array whose index is the id (slot 0 and
deleted events are `null`); VX Ace keeps a hash. A `null` value is an empty slot.

```csharp
foreach (var (id, ev) in map.Events)
    if (ev != null) Console.WriteLine($"{id} {ev.Name} at {ev.X},{ev.Y}, {ev.Pages.Count} pages");
```

`IEvent`: `Id`, `Name`, `X`, `Y`, `Pages`. (MV events also have a `note`; it is kept, but it is not on the interface.) `storage.New<IEvent>()` makes an event with one page; the
dictionary has no `NextId`, take the id yourself (the highest key + 1) and set `Id`.

**`map.CreateEvent(x, y, name = null, build = null)`** (`MapEventExtensions`, on `IMap`) does that and puts the event on the map: the id is the highest key (an empty slot counts) + 1, `Id` is set,
the name is `EV001`... by the id when none is given (checked: the MV editor named the event of map 2 of the test project `EV001`; the VX Ace editor, with all events of a map deleted, opened a new event as `ID:001`, named `EV001`, and with event 001 present as `ID:002` / `EV002`), the first page is a new page whose commands the optional
`build` (a `CommandBuilder` function, as in `Append`) makes. A square outside of the map, or one with an event on it, is refused (in the editors a second event cannot be put on a square either: the double click opens the first one) (`ArgumentOutOfRangeException`, `InvalidOperationException`).
The maker is told by the class of the map. **`map.ClearEvents()`** removes every event (the empty slots too; it returns how many events there were), and the next `CreateEvent` numbers from 1.
Further pages: `ev.Pages.Add(storage.New<IEventPage>())`.

### Pages (`IEventPage`)

| Property | MV key / VX Ace ivar | Meaning |
|---|---|---|
| `Condition` | `conditions` / `@condition` | when the page is active (below) |
| `Graphic` | `image` / `@graphic` | what is drawn (below) |
| `MoveType` | `moveType` | 0 fixed, 1 random, 2 approach, 3 custom (`EventMoveType`) |
| `MoveSpeed` | `moveSpeed` | 1-6 (1 = 8x slower ... 4 normal ... 6 = 4x faster); a new page: 3 |
| `MoveFrequency` | `moveFrequency` | 1-5 (3 normal); a new page: 3 |
| `MoveRoute` | `moveRoute` | the route that a custom move type walks (`Repeat`, `Skippable`, `Wait`, route commands) |
| `WalkAnime`, `StepAnime` | `walkAnime`, `stepAnime` | walking / stepping animation; new page: true / false |
| `DirectionFix`, `Through` | | |
| `PriorityType` | `priorityType` | 0 below characters, 1 same as characters, 2 above (`EventPriority`) |
| `Trigger` | `trigger` | 0 action button, 1 player touch, 2 event touch, 3 autorun, 4 parallel (`EventTriggerType`) |

The numbers are the same in both makers. A new page (`New<IEvent>()`): no condition, no picture, a fixed event, speed 3, frequency 3, walking animation, below characters,
action button, and a list that is the empty command only.

**Which page is active**: the **last** page (the highest index; index 0 is page 1) whose conditions are all met; none if no page is (the event is not drawn and does nothing).

### The conditions (`IEventPageCondition`)

A condition counts only if its `...Valid` flag is true; all valid ones must hold (the engine, `Game_Event#meetsConditions`):

| Flag | Needs |
|---|---|
| `Switch1Valid`, `Switch2Valid` | the switch `Switch1Id` / `Switch2Id` is on |
| `VariableValid` | the variable `VariableId` is **greater or equal** to `VariableValue` |
| `SelfSwitchValid` | the self switch `SelfSwitchCh` ("A".."D") of this event is on |
| `ItemValid` | the party has the item `ItemId` |
| `ActorValid` | the actor `ActorId` is in the party |

A new condition has every flag false and the ids at 1 (switch 1, variable 1, self switch "A", item 1, actor 1).

### The graphic (`IPageGraphic`)

- `TileId > 0`: a tile from the map's tileset (the id as in [maps-and-tiles.md](maps-and-tiles.md)); `CharacterName` is then ignored.
- else a character sheet: `CharacterName` (a file of `Graphics/Characters` / `img/characters`, `""` = nothing), `CharacterIndex` (0-7, which of the 8 characters on the sheet), `Direction`
  (2 down, 4 left, 6 right, 8 up), `Pattern` (0-2, the frame). A name that starts with `$` is a sheet with **one** character; one that starts with `!` is drawn without the half-tile
  shift of an object (`ImageManager.isBigCharacter` / `isObjectCharacter`); `$` and `!` can be combined.
- **A tile event** is an event whose current page has a tile and the priority "below characters" (`PriorityType == 0`): the engine draws it as part of the map and uses its tile in the passage
  checks (see maps-and-tiles.md).

### How an event starts

- The action button, player touch and event touch (triggers 0, 1, 2) start the event only if its list has **more than the empty command**; the event is locked while it runs.
- Autorun (3) starts by itself, and starts again for as long as the page stays active; parallel (4) runs beside the game (it starts again whenever it has finished).

## Common events (`ICommonEvent`)

`Name`, `Trigger`, `SwitchId` and the commands. The id is the place in the list (`CommonEvents.Get(id)`), as the command Common Event (117) calls it.
`Trigger` is 0 = none (only called by a command), 1 = autorun, 2 = parallel. An autorun or parallel common event runs **while the switch `SwitchId` is on**.

## Troops (`ITroop`)

- `Members` (`IMember`): `EnemyId`, `X`, `Y` (the position on the battle screen, in pixels of the maker's screen), `Hidden` (appears hidden at the start).
- `Pages` (`ITroopPage`): `Span`, `Condition`, commands. A troop page is a battle event.

`Span`: 0 once per battle, 1 once per turn, 2 every time the condition holds ("moment"). In the engine the pages whose span is 0 or 1 are marked as done after they start; a span 1 page is
cleared again at the next turn.

### The conditions (`ITroopPageCondition`)

All of the valid ones must hold, **and at least one must be set** (a page with no condition never starts; `Game_Troop#meetsConditions`):

| Flag | Meaning |
|---|---|
| `TurnEnding` | the turn is ending |
| `TurnValid` | `TurnA` and `TurnB`: with `TurnB = 0` exactly turn `TurnA`; with `TurnB > 0` turn `TurnA + k * TurnB` (and not before turn 1) |
| `EnemyValid` | the enemy with the index `EnemyIndex` (its place in the troop) has HP of `EnemyHp` percent **or less** |
| `ActorValid` | the actor `ActorId` has HP of `ActorHp` percent or less |
| `SwitchValid` | the switch `SwitchId` is on |

## Reaching them from code

- `MapEventHelper.WalkMapEventPages(storage, walker)`, `TroopHelper.WalkTroopPages`, `CommonEventHelper.WalkCommonEvents`: walk every place that holds commands. The walker gets a context
  (`MapEventContext`: `MapId`, `Map`, `Event`, `PageId`, `EventPage`; `TroopContext`; `CommonEventContext`). A walker returns nothing (a `Task`) and sets `context.StopWalking = true` to stop the walk after the current place.
  They load what they need and report progress.
- `Storage.SearchCommands(walker)` walks the commands of the common events, the map events and the troops with their `EntityContext`.
- `EntityContext` (storage + the context of the place) is what `GetParametersDescription` and `NameFormatter` use to print `[0005: Name]`-style references; the dump needs one.

## Not checked

- `CreateEvent`: that the VX Ace editor numbers a new event after the highest id (and not into a free gap) was not tried; the MV editor's numbering was not tried (the project owner says it is the same as in VX Ace).

- Anything about the VX Ace page conditions that the stock scripts do not show (the check is in `Game_Event`; the table above is the MV engine's and the data fields are the same).
- The battle screen position of troop members in pixels of VX Ace: the numbers are copied by the converter and scaled with the screen, but the origin was not looked at here.
