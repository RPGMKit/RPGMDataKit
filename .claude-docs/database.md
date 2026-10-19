# Database entities

The records of the database (`Actors`, `Classes`, `Skills`, `Items`, `Weapons`, `Armors`, `Enemies`, `States`, `Animations`, `System`; `Tilesets`, `Troops` and
`CommonEvents` are in [maps-and-tiles.md](maps-and-tiles.md) and [events.md](events.md)), with the field of each maker, what the engine does with it, and what the interfaces give.
The field tables are taken from the classes in `Data/MV/Classes` and `Data/VXAce/Classes`; the meaning is from the engine of MV 1.6.1 and the stock scripts of VX Ace.
How the files are read: [storage.md](storage.md).

## The shape

- Every table is a list indexed by the **id**; slot 0 is not used (`null`). `Storage.Actors.Get(3)` is the actor 3. The id of an entity is its place (see storage.md).
- **Hierarchy**: `IBaseItem` (`Id`, `Name`, `Note`) is the base of actors, classes, skills, items, weapons, armors, enemies and states. Above it: `IUsableItem` (skills and items:
  scope, occasion, damage, effects), `IEquipItem` (weapons and armors: price, equipment type, parameters), `IHasFeatures` (actors, classes, weapons, armors, enemies, states), `IHasIcon`, `IHasDescription`.
  Animations, tilesets, troops, common events and map infos are `IHasId` only. VX Ace has the real classes `RPG::BaseItem` ... ; MV has no such classes, the library's `BaseItem` of MV only mirrors them.
- The interfaces carry what both makers have. What is in one maker only is on the class of that maker (for example `Actor.Equips`, `Class.Params`, `Enemy.DropItems` are class members, not interface ones); what exists in
  one maker but is on the interface throws `DoesNotExistInVersionException` in the other (`IActor.BattlerName` in VX Ace). `HasProperty(name)` tells.
- **Features (VX Ace) = traits (MV)**: the same thing under two names. The interface `IHasFeatures.Features` is read-only; to change them use `Features` (VX Ace classes) or `Traits` (MV classes).

## Actors

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `BattlerName` | `string` | `battlerName` | - | the sideview battler picture (MV only) |
| `CharacterIndex` | `int` | `characterIndex` | `@character_index` | 0-7, which character of the sheet |
| `CharacterName` | `string` | `characterName` | `@character_name` | the map sprite sheet; `$`/`!` prefixes as for events |
| `ClassId` | `int` | `classId` | `@class_id` | the class (`Classes`) |
| `Equips` | `List<int>` | `equips` | `@equips` | the item id for each equipment slot, in the order of the slots: weapon, shield (or second weapon with dual wield), head, body, accessory; 0 = nothing |
| `FaceIndex` | `int` | `faceIndex` | `@face_index` | 0-7 |
| `FaceName` | `string` | `faceName` | `@face_name` | the face sheet |
| `Traits` | `List<Trait>` | `traits` | - | MV: the traits (VX Ace: `@features`) |
| `InitialLevel` | `int` | `initialLevel` | `@initial_level` |  |
| `MaxLevel` | `int` | `maxLevel` | `@max_level` |  |
| `NickName` | `string` | `nickname` | `@nickname` |  |
| `Profile` | `string` | `profile` | - | the text of the status window (MV). VX Ace keeps it in `@description` |
| `IconIndex` | `int` | - | `@icon_index` | VX Ace: a field of every entity; not used for actors |
| `Description` | `string` | - | `@description` | VX Ace: the profile text of the actor |
| `Features` | `List<Feature>` | - | `@features` | VX Ace: the features (MV: `traits`) |

## Classes

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `Traits` | `List<Trait>` | `traits` | - |  |
| `ExpParams` | `List<int>` | `expParams` | `@exp_params` | `[base, extra, accelerationA, accelerationB]`; the experience for a level is `round(base * (level-1)^(0.9 + A/250) * level * (level+1) / (6 + level^2 / 50 / B) + (level-1) * extra)` (MV `expForLevel`; the formula of VX Ace is inside RGSS3 and was not looked at) |
| `Params` | MV `List<object>`, VX Ace `Table` | `params` | `@params` | the 8 parameters by level: MV a list of 8 lists of 100 numbers, VX Ace a `Table` of 8 x 100, both indexed `[parameter][level]` with level 1-99 (index 0 is a placeholder: 1 in the files of the MV editor, 0 in VX Ace). Order: max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK |
| `Learnings` | `List<Learning>` | `learnings` | `@learnings` | the skills learned by level |
| `IconIndex` | `int` | - | `@icon_index` |  |
| `Description` | `string` | - | `@description` |  |
| `Features` | `List<Feature>` | - | `@features` |  |

`Learning` (`Level`, `SkillId`, `Note`) is the same in both makers.

## Skills and items (usable items)

Skills and items share the part of `UsableItem`; each adds its own fields.

### Skills

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `IconIndex` | `int` | `iconIndex` | `@icon_index` |  |
| `Description` | `string` | `description` | `@description` |  |
| `Scope` | `int` | `scope` | `@scope` | `ItemScope` |
| `Occasion` | `int` | `occasion` | `@occasion` | `ItemOccasion` |
| `Speed` | `int` | `speed` | `@speed` | the priority of the action (added to the speed of the user) |
| `AnimationId` | `int` | `animationId` | `@animation_id` | the animation: -1 = the animation of the normal attack, 0 = none |
| `SuccessRate` | `int` | `successRate` | `@success_rate` | percent |
| `Repeats` | `int` | `repeats` | `@repeats` | times the action is done |
| `TpGain` | `int` | `tpGain` | `@tp_gain` | TP the user gets |
| `HitType` | `int` | `hitType` | `@hit_type` | `HitType` |
| `Damage` | `ItemDamage` | `damage` | `@damage` | see "The damage and the effects" |
| `Effects` | `List<Effect>` | `effects` | `@effects` | see "The damage and the effects" |
| `STypeId` | `int` | `stypeId` | `@stype_id` | the skill type (`System.SkillTypes`) |
| `MpCost` | `int` | `mpCost` | `@mp_cost` |  |
| `TpCost` | `int` | `tpCost` | `@tp_cost` |  |
| `Message1` | `string` | `message1` | `@message1` | shown when it is used: `<user name><Message1 with the skill name>`; `Message2` is a second line |
| `Message2` | `string` | `message2` | `@message2` |  |
| `RequiredWTypeId1` | `int` | `requiredWtypeId1` | `@required_wtype_id1` | the weapon type the user must have equipped (0 = none) |
| `RequiredWTypeId2` | `int` | `requiredWtypeId2` | `@required_wtype_id2` | a second one |
| `Features` | `List<Feature>` | - | `@features` |  |

### Items

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `IconIndex` | `int` | `iconIndex` | `@icon_index` |  |
| `Description` | `string` | `description` | `@description` |  |
| `Scope` | `int` | `scope` | `@scope` | `ItemScope` |
| `Occasion` | `int` | `occasion` | `@occasion` | `ItemOccasion` |
| `Speed` | `int` | `speed` | `@speed` | the priority of the action (added to the speed of the user) |
| `AnimationId` | `int` | `animationId` | `@animation_id` | the animation: -1 = the animation of the normal attack, 0 = none |
| `SuccessRate` | `int` | `successRate` | `@success_rate` | percent |
| `Repeats` | `int` | `repeats` | `@repeats` | times the action is done |
| `TpGain` | `int` | `tpGain` | `@tp_gain` | TP the user gets |
| `HitType` | `int` | `hitType` | `@hit_type` | `HitType` |
| `Damage` | `ItemDamage` | `damage` | `@damage` | see "The damage and the effects" |
| `Effects` | `List<Effect>` | `effects` | `@effects` | see "The damage and the effects" |
| `ITypeId` | `int` | `itypeId` | `@itype_id` | 1 = regular item, 2 = key item (the choice lists of Select Item and the menu use it) |
| `Price` | `int` | `price` | `@price` |  |
| `Consumable` | `bool` | `consumable` | `@consumable` | is used up |
| `Features` | `List<Feature>` | - | `@features` |  |

### The damage and the effects

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Type` | `int` | `type` | `@type` | `DamageType` |
| `ElementId` | `int` | `elementId` | `@element_id` | -1 = the element of the normal attack of the user, 0 = none |
| `Formula` | `string` | `formula` | `@formula` | **JavaScript in MV, Ruby in VX Ace** (`a` the user, `b` the target, `v` the variables): `a.atk * 4 - b.def * 2` |
| `Variance` | `int` | `variance` | `@variance` | percent |
| `Critical` | `bool` | `critical` | `@critical` | can be a critical hit |

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Code` | `int` | `code` | `@code` | `EffectCodes` |
| `DataId` | `int` | `dataId` | `@data_id` | depends on the code |
| `Value1` | `object` | `value1` | `@value1` | depends on the code |
| `Value2` | `object` | `value2` | `@value2` | depends on the code |

How the engine uses an effect (`Game_Action#itemEffect*`, `EffectCodes`):

| Code | Name | `DataId` | `Value1` | `Value2` |
|---|---|---|---|---|
| 11 | Recover HP | - | fraction of max HP | fixed amount (the result is multiplied by the target's recovery rate, and by the user's pharmacology for an item) |
| 12 | Recover MP | - | fraction of max MP | fixed amount |
| 13 | Gain TP | - | amount | - |
| 21 | Add state | the state (0 = the states of the normal attack of the user) | chance (1 = 100%) | - |
| 22 | Remove state | the state | chance | - |
| 31 | Add buff | the parameter (0-7) | turns | - |
| 32 | Add debuff | the parameter | turns | - |
| 33 | Remove buff | the parameter | - | - |
| 34 | Remove debuff | the parameter | - | - |
| 41 | Special effect | 0 = escape | - | - |
| 42 | Grow | the parameter | amount added permanently | - |
| 43 | Learn skill | the skill | - | - |
| 44 | Common event | the common event | - | - |

`DamageType`: 0 none, 1 HP damage, 2 MP damage, 3 HP recover, 4 MP recover, 5 HP drain, 6 MP drain. `ItemScope`: 0 none, 1 one enemy, 2 all enemies, 3-6 one to four random enemies, 7 one ally, 8 all allies,
9 one dead ally, 10 all dead allies, 11 the user. `ItemOccasion`: 0 always, 1 battle only, 2 menu only, 3 never. `HitType`: 0 certain hit, 1 physical, 2 magical.

## Weapons and armors (equipment)

### Weapons

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `IconIndex` | `int` | `iconIndex` | `@icon_index` |  |
| `Description` | `string` | `description` | `@description` |  |
| `Price` | `int` | `price` | `@price` |  |
| `ETypeId` | `int` | `etypeId` | `@etype_id` | the equipment type; VX Ace counts from 0 (weapon), MV from 1 |
| `Params` | `List<int>` | `params` | `@params` | 8 numbers added to the parameters: max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK |
| `Traits` | `List<Trait>` | `traits` | - |  |
| `WTypeId` | `int` | `wtypeId` | `@wtype_id` | the weapon type (`System.WeaponTypes`) |
| `AnimationId` | `int` | `animationId` | `@animation_id` | the animation of an attack with it (0 = none) |
| `Features` | `List<Feature>` | - | `@features` |  |

### Armors

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `IconIndex` | `int` | `iconIndex` | `@icon_index` |  |
| `Description` | `string` | `description` | `@description` |  |
| `Price` | `int` | `price` | `@price` |  |
| `ETypeId` | `int` | `etypeId` | `@etype_id` | the equipment type; VX Ace counts from 0 (weapon), MV from 1 |
| `Params` | `List<int>` | `params` | `@params` | 8 numbers added to the parameters: max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK |
| `Traits` | `List<Trait>` | `traits` | - |  |
| `ATypeId` | `int` | `atypeId` | `@atype_id` | the armor type (`System.ArmorTypes`) |
| `Features` | `List<Feature>` | - | `@features` |  |

The equipment types: VX Ace counts from 0 (0 weapon, 1 shield, 2 head, 3 body, 4 accessory), MV from 1 (1 weapon, 2 shield, ...); `System.EquipmentTypes` (MV) lists their names. The 8 parameters, in order everywhere
(equipment, enemies, classes): max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK.

## Enemies

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `BattlerName` | `string` | `battlerName` | `@battler_name` | the battler picture |
| `BattlerHue` | `int` | `battlerHue` | `@battler_hue` | 0-360 |
| `Params` | `List<int>` | `params` | `@params` | 8 numbers: max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK |
| `Exp` | `int` | `exp` | `@exp` |  |
| `Gold` | `int` | `gold` | `@gold` |  |
| `DropItems` | `List<DropItem>` | `dropItems` | `@drop_items` | the drops (the editor writes 3): `Kind` (`DropItemKind`), `DataId` (the item, weapon or armor id), `Denominator` (the chance is 1/n; doubled by a "double drops" party ability) |
| `Actions` | `List<Action>` | `actions` | `@actions` | what the enemy does in battle (below) |
| `Traits` | `List<Trait>` | `traits` | - |  |
| `IconIndex` | `int` | - | `@icon_index` |  |
| `Description` | `string` | - | `@description` |  |
| `Features` | `List<Feature>` | - | `@features` |  |

`DropItem` (`Kind`, `DataId`, `Denominator`) and `Action` (`SkillId`, `ConditionType`, `ConditionParam1`, `ConditionParam2`, `Rating`) are the same in both makers.
`EnemyActionCondition`: 0 always, 1 turn (`Param1 + Param2 * X`), 2 HP between `Param1` and `Param2` (fractions), 3 MP between, 4 the state `Param1`, 5 the party level is `Param1` or more, 6 the switch `Param1` is on.
`DropItemKind`: 0 none, 1 item, 2 weapon, 3 armor.

## States

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Note` | `string` | `note` | `@note` |  |
| `IconIndex` | `int` | `iconIndex` | `@icon_index` |  |
| `Restriction` | `int` | `restriction` | `@restriction` | `StateRestriction` |
| `Priority` | `int` | `priority` | `@priority` | 0-100: the state with the highest priority shows its icon, motion and overlay |
| `Overlay` | `int` | `overlay` | - | MV: the overlay picture of the state (0 = none) |
| `RemoveAtBattleEnd` | `bool` | `removeAtBattleEnd` | `@remove_at_battle_end` |  |
| `RemoveByRestriction` | `bool` | `removeByRestriction` | `@remove_by_restriction` | the state is removed when the battler becomes restricted (gets a state with a restriction) |
| `AutoRemovalTiming` | `int` | `autoRemovalTiming` | `@auto_removal_timing` | `AutoRemovalTiming`; with `MinTurns` and `MaxTurns` (the length is random between them) |
| `MinTurns` | `int` | `minTurns` | `@min_turns` |  |
| `MaxTurns` | `int` | `maxTurns` | `@max_turns` |  |
| `Motion` | `int` | `motion` | - | MV: the sideview motion of the actor: 0 normal, 1 abnormal, 2 sleep, 3 dead |
| `RemoveByDamage` | `bool` | `removeByDamage` | `@remove_by_damage` | with `ChanceByDamage` percent |
| `ChanceByDamage` | `int` | `chanceByDamage` | `@chance_by_damage` |  |
| `RemoveByWalking` | `bool` | `removeByWalking` | `@remove_by_walking` | with `StepsToRemove` steps |
| `StepsToRemove` | `int` | `stepsToRemove` | `@steps_to_remove` |  |
| `Message1` | `string` | `message1` | `@message1` | when an actor gets it, `Message2` an enemy, `Message3` it is still there, `Message4` it is removed |
| `Message2` | `string` | `message2` | `@message2` |  |
| `Message3` | `string` | `message3` | `@message3` |  |
| `Message4` | `string` | `message4` | `@message4` |  |
| `Traits` | `List<Trait>` | `traits` | - |  |
| `Description` | `string` | - | `@description` | VX Ace: a field of every entity; not used for states |
| `Features` | `List<Feature>` | - | `@features` |  |

`StateRestriction`: 0 none, 1 attack enemies, 2 attack anyone, 3 attack allies, 4 cannot move. `AutoRemovalTiming`: 0 none, 1 action end, 2 turn end.

## Features (traits)

A feature has `Code`, `DataId` and `Value` (MV key `traits[]` with `code`, `dataId`, `value`; VX Ace `RPG::BaseItem::Feature`). `FeatureCodes` names the codes; the meaning of `DataId` and `Value` depends on the code:

| Code | Name | `DataId` | `Value` |
|---|---|---|---|
| 11 | Element rate | the element | rate (1 = 100%); several are multiplied |
| 12 | Debuff rate | the parameter (0-7) | rate |
| 13 | State rate | the state | rate |
| 14 | State resist | the state | - |
| 21 | Parameter | 0-7 (max HP, max MP, ATK, DEF, MAT, MDF, AGI, LUK) | rate (multiplied) |
| 22 | Extra parameter | 0 hit, 1 evasion, 2 critical, 3 critical evasion, 4 magic evasion, 5 magic reflection, 6 counter, 7 HP regeneration, 8 MP regeneration, 9 TP regeneration | added |
| 23 | Special parameter | 0 target rate, 1 guard effect, 2 recovery effect, 3 pharmacology, 4 MP cost rate, 5 TP charge rate, 6 physical damage rate, 7 magical damage rate, 8 floor damage rate, 9 experience rate | rate (multiplied) |
| 31 | Attack element | the element | - |
| 32 | Attack state | the state | chance |
| 33 | Attack speed | - | added |
| 34 | Attack times | - | added |
| 41 / 42 | Skill type add / seal | the skill type | - |
| 43 / 44 | Skill add / seal | the skill | - |
| 51 / 52 | Equip weapon type / armor type | the type | - |
| 53 / 54 | Fix / seal equipment | the equipment type | - |
| 55 | Slot type (dual wield) | 1 | - |
| 61 | Extra action | - | chance |
| 62 | Special flag | 0 auto battle, 1 guard, 2 substitute, 3 preserve TP | - |
| 63 | Collapse type | 0 normal, 1 boss, 2 instant, 3 none | - |
| 64 | Party ability | 0 encounter half, 1 no encounters, 2 cancel surprise, 3 raise preemptive, 4 double gold, 5 double drops | - |

(Checked against `Game_BattlerBase` of MV and VX Ace: the constants and the names of the parameters agree.)

## Animations

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Id` | `int` | `id` | `@id` |  |
| `Name` | `string` | `name` | `@name` |  |
| `Animation1Name` | `string` | `animation1Name` | `@animation1_name` | the first picture sheet (`Graphics/Animations`) |
| `Animation1Hue` | `int` | `animation1Hue` | `@animation1_hue` | 0-360 |
| `Animation2Name` | `string` | `animation2Name` | `@animation2_name` |  |
| `Animation2Hue` | `int` | `animation2Hue` | `@animation2_hue` |  |
| `Position` | `int` | `position` | `@position` | `AnimationPosition` |
| `Frames` | MV `List<object>`, VX Ace `List<Frame>` | `frames` | `@frames` | the pictures of every frame: MV a list of lists of cells, VX Ace `Frame` objects with a `Table` of cells (`CellMax`, `CellData`) |
| `Timings` | `List<Timing>` | `timings` | `@timings` | the sounds and flashes at frames |
| `FrameMax` | `int` | - | `@frame_max` | VX Ace: the number of frames |

`Timing`:

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Frame` | `int` | `frame` | `@frame` |  |
| `SE` | `SE` | `se` | `@se` | the sound |
| `Condition` | `int` | `conditions` | `@condition` | when it fires (MV key `conditions`) |
| `FlashScope` | `int` | `flashScope` | `@flash_scope` | `FlashScope` |
| `FlashColor` | `Color` | `flashColor` | `@flash_color` |  |
| `FlashDuration` | `int` | `flashDuration` | `@flash_duration` |  |

## System

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Airship` | `Vehicle` | `airship` | `@airship` |  |
| `ArmorTypes` | `List<string>` | `armorTypes` | `@armor_types` |  |
| `AttackMotions` | `List<AttackMotion>` | `attackMotions` | - |  |
| `BattleBgm` | `BGM` | `battleBgm` | - | MV key `battleBgm` (VX Ace `@battle_bgm`) |
| `Battleback1Name` | `string` | `battleback1Name` | `@battleback1_name` |  |
| `Battleback2Name` | `string` | `battleback2Name` | `@battleback2_name` |  |
| `BattlerHue` | `int` | `battlerHue` | `@battler_hue` |  |
| `BattlerName` | `string` | `battlerName` | `@battler_name` |  |
| `Boat` | `Vehicle` | `boat` | `@boat` | `Vehicle`: map, position, picture, BGM |
| `CurrencyUnit` | `string` | `currencyUnit` | `@currency_unit` |  |
| `DefeatMe` | `ME` | `defeatMe` | - |  |
| `EditMapId` | `int` | `editMapId` | `@edit_map_id` |  |
| `Elements` | `List<string>` | `elements` | `@elements` |  |
| `EncryptionKey` | `string` | `encryptionKey` | - | MV: key of the encrypted resources |
| `GameTitle` | `string` | `gameTitle` | `@game_title` |  |
| `GameOverMe` | `ME` | `gameoverMe` | - |  |
| `HasEncryptedAudio` | `bool` | `hasEncryptedAudio` | - | MV |
| `HasEncryptedImages` | `bool` | `hasEncryptedImages` | - | MV |
| `Locale` | `string` | `locale` | - | MV |
| `MagicSkills` | `List<object>` | `magicSkills` | - |  |
| `MenuCommands` | `List<bool>` | `menuCommands` | - |  |
| `OptDisplayTp` | `bool` | `optDisplayTp` | - | show TP |
| `OptDrawTitle` | `bool` | `optDrawTitle` | `@opt_draw_title` | draw the game title on the title screen |
| `OptExtraExp` | `bool` | `optExtraExp` | `@opt_extra_exp` | experience of members that did not fight |
| `OptFloorDeath` | `bool` | `optFloorDeath` | `@opt_floor_death` | floor damage can kill |
| `OptFollowers` | `bool` | `optFollowers` | `@opt_followers` | followers walk after the player |
| `OptSideView` | `bool` | `optSideView` | - | MV: sideview battle |
| `OptSlipDeath` | `bool` | `optSlipDeath` | `@opt_slip_death` | poison can kill |
| `OptTransparent` | `bool` | `optTransparent` | `@opt_transparent` | the player is transparent at the start |
| `PartyMembers` | `List<int>` | `partyMembers` | `@party_members` | the actors at the start of a new game |
| `Ship` | `Vehicle` | `ship` | `@ship` |  |
| `SkillTypes` | `List<string>` | `skillTypes` | `@skill_types` |  |
| `Sounds` | `List<ISE?>` | `sounds` | `@sounds` | the 24 system sounds by index (cursor, OK, cancel, ...) |
| `StartMapId` | `int` | `startMapId` | `@start_map_id` | with `StartX`, `StartY` |
| `StartX` | `int` | `startX` | `@start_x` |  |
| `StartY` | `int` | `startY` | `@start_y` |  |
| `Switches` | `List<string>` | `switches` | `@switches` | the names of the switches; the id is the index (slot 0 is empty) |
| `Terms` | `Terms` | `terms` | `@terms` | the words of the interface |
| `TestBattlers` | `List<TestBattler>` | `testBattlers` | - | MV: the party of the battle test (VX Ace: `Battlers`) |
| `TestTroopId` | `int` | `testTroopId` | `@test_troop_id` |  |
| `Title1Name` | `string` | `title1Name` | `@title1_name` |  |
| `Title2Name` | `string` | `title2Name` | `@title2_name` |  |
| `TitleBgm` | `BGM` | `titleBgm` | - |  |
| `EquipmentTypes` | `List<string>` | `equipTypes` | - | MV: `equipTypes`; VX Ace keeps them in `Terms.etypes` |
| `Variables` | `List<string>` | `variables` | `@variables` | the same for variables |
| `VersionId` | `int` | `versionId` | `@version_id` | a number the editor changes when the game data is saved; a save file that was made with another number makes the engine reload the map on loading (`Scene_Load#reloadMapIfUpdated`) |
| `VictoryMe` | `ME` | `victoryMe` | - | MV; VX Ace: `BattleEndME` (`@battle_end_me`) |
| `WeaponTypes` | `List<string>` | `weaponTypes` | `@weapon_types` | by id (slot 0 empty) |
| `_WindowTone` | MV `List<int>`, VX Ace `Tone` | `windowTone` | `@window_tone` | MV: a list of 4 numbers (r, g, b, gray); VX Ace: a `Tone` object. `WindowTone` of the interface hides it |
| `Unk_` | `int` | - | `@_` |  |
| `Battlers` | `List<TestBattler>` | - | `@test_battlers` | VX Ace: the party of the battle test |
| `BattleEndME` | `ME` | - | `@battle_end_me` | VX Ace; MV: `VictoryMe` |
| `BattleBGM` | `BGM` | - | `@battle_bgm` |  |
| `MagicNumber` | `int` | - | `@magic_number` |  |
| `GameoverME` | `ME` | - | `@gameover_me` |  |
| `TitleBGM` | `BGM` | - | `@title_bgm` |  |
| `Japanese` | `bool` | - | `@japanese` |  |
| `OptUseMidi` | `bool` | - | `@opt_use_midi` |  |
| `OptDisplayTP` | `bool` | - | `@opt_display_tp` |  |

`Terms`:

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `Basic` | `List<string>` | `basic` | - | level, HP, MP, TP, EXP and their short forms |
| `Commands` | `List<string?>` | `commands` | - | the menu and command words |
| `Messages` | `Dictionary<string, string>` | `messages` | - | MV: the battle and system messages by key (VX Ace keeps them in the script `Vocab`) |
| `Params` | `List<string>` | `params` | - | the names of the 8 parameters (VX Ace: `@params`) |
| `_params` | `List<string>` | - | `@params` | VX Ace: `@params` |
| `etypes` | `List<string>` | - | `@etypes` | VX Ace: the equipment type names (MV: `System.equipTypes`) |
| `commands` | `List<string>` | - | `@commands` |  |
| `basic` | `List<string>` | - | `@basic` |  |

`Vehicle` (`Boat`, `Ship`, `Airship`):

| Property | Type | MV key | VX Ace field | Notes |
|---|---|---|---|---|
| `BGM` | `BGM` | `bgm` | `@bgm` |  |
| `CharacterIndex` | `int` | `characterIndex` | - |  |
| `CharacterName` | `string` | `characterName` | - |  |
| `StartMapId` | `int` | `startMapId` | - | with `StartX`, `StartY` |
| `StartX` | `int` | `startX` | `@start_x` |  |
| `StartY` | `int` | `startY` | `@start_y` |  |
| `character_index` | `int` | - | `@character_index` |  |
| `character_name` | `string` | - | `@character_name` |  |
| `start_map_id` | `int` | - | `@start_map_id` |  |

`TestBattler` (the battle test party): `ActorId`, `Level`, `Equips` in both makers.

MV also keeps `AttackMotions` (`Type`, `WeaponImageId`: the sideview motions by weapon type) and `MagicSkills`, `MenuCommands` (which menu commands are shown), and `Terms.Messages`.

## What differs between the makers (summary)

- traits (MV) / features (VX Ace); the equipment type ids start at 1 (MV) / 0 (VX Ace); damage formulas are JavaScript (MV) / Ruby (VX Ace);
- the actor: `Profile` and `BattlerName` (MV only), `Description` carries the profile in VX Ace; `IconIndex` and `Description` exist in VX Ace's base class for every entity but the editor uses them only for skills, items, weapons, armors (and states have no description in MV);
- the state: `Motion` and `Overlay` (MV only, for the sideview battle);
- the class: parameters are lists in MV and a `Table` in VX Ace; the animation: cells are lists in MV and `Frame` objects with a `Table` in VX Ace;
- `System`: MV adds the sideview option, locale, the encryption fields, the attack motions, the menu commands and the equipment type names; VX Ace has `@japanese`, `@opt_use_midi`, the magic number, and the equipment type names in `Terms`;
- the sounds (the system sounds, the sounds of an animation): the pan exists in MV only.

## What a new entity has

`storage.New<T>()` makes an entity with the values the editor gives to a new one (`INewEntity.InitializeNew`): for example a new class has the experience curve `[30, 20, 30, 30]`, the parameter curves of the editor and
its basic features (hit 95%, evasion 5%, ...). The values that differ between the makers or are not 0/empty are listed in `FactoryDefaultsTests`; they were compared with the editors on real projects.

## Not checked

- The formula of the experience of VX Ace (it is inside RGSS3).
- The animation cells and the flash timing in detail (`Frames`, `Timings`): only the structure is listed.
- The sideview `AttackMotions`, `MagicSkills` and `MenuCommands` of MV: listed, not described.
