# Event command formats

What every command holds, parameter by parameter, in VX Ace and in MV, and where the two differ. The command codes are the same in both makers;
the **parameters** are `command.parameters[i]` in the files, and each is a property of the command class (the number in the lists below is the index).
How the classes work: [commands.md](commands.md). What a new command is given: [editor-defaults.md](editor-defaults.md). Using the builder, blocks and
indents: [README](../README.md).

## Reading the lists

- `**code** Name (`Class`)`: then each parameter as `index `Property` Type (enum values) [notes]`. Notes: `default X` is the value the editor gives to a new
  command (`default X, VXAce` for one maker only), `not in VXAce` means the maker does not have the parameter, `raw value` is the number as the maker
  writes it (the property next to it holds the meaning).
- A name with a slash is `MV name / VX Ace name` (the editors call the command differently).
- An `If` is code **111**: parameter 0 is the kind of condition (the number after the slash, `IfCommand.IfType`), the rest depends on the kind.
- The values are what the **code** says (enums in `Data/Enums` and nested in the classes). Where the enum has two names for one number (`Black` and `Normal`
  of a fade, `Sub` and `Multiply` of a blend mode) it is the same number for the two makers' names.
- This part of the page is taken from the sources of the classes (`Get`/`Set` indexes, attributes, enums). If a class changes, change its line.

## How a block looks in the list

A command list is flat: the engines find a block by the indents, and the editors need an empty command (code 0) as the last command of every block.
The commands that go together:

| Starts with | Continues / branches with | Ends with |
|---|---|---|
| 101 Text | 401 (one per line of text) | - |
| 105 Scrolling text | 405 (a line each) | - |
| 108 Comment | 408 | - |
| 355 Script | 655 | - |
| 102 Show Choices | 402 When (one per choice), 403 When Cancel | 404 End |
| 111 If | 411 Else | 412 End |
| 112 Loop | - | 413 Repeat Above |
| 301 Battle Processing | 601 If Win, 602 If Escape, 603 If Lose | 604 End |
| 302 Shop Processing | 605 (the other goods) | - |
| 205 Set Movement Route | 505 (a line for each step of the route) | - |

`UnknownCommand(code)` is made for a code that is not in the list; its parameters are kept as they are.

## Where VX Ace and MV differ

| Command | VX Ace | MV |
|---|---|---|
| 102 Show Choices | parameters `[choices, cancel]`; up to 4 choices; cancel: 0 = not allowed, 5 = branch, n = choice n (1-based) | `[choices, cancel, default, position, background]`; up to 6 choices; cancel: -1 = not allowed, -2 = branch, n-1 = choice n (0-based). The property `Cancel` hides this; `CancelChoiceIndex` is the raw number |
| 403 When Cancel | no parameters | `[6, null]` |
| 104 Select Item | `[variable]` | `[variable, type]`: `KeyItemType` Regular 1, Key 2, HiddenA 3, HiddenB 4 |
| 111 If Button | the number of the button (2, 4, 6, 8 for the arrows, 11-18 for A, B, C, X, Y, Z, L, R) | the name (`ok`, `cancel`, `shift`, `down`, `left`, `right`, `up`, `pageup`, `pagedown`); X, Y, Z have no name, `MvName` is null |
| 111 If Enemy, "has appeared" | `[5, enemy, 0, 1]` (a fourth parameter) | `[5, enemy, 0]` |
| 133 | Change Battle End ME | Change Victory ME |
| 139 Change Defeat ME, 140 Change Vehicle BGM | not there (the class throws if put into a VX Ace list) | there |
| 285 Get Location Info | type: 0 terrain tag, 1 event id, 2-4 tile id of layers 1-3, **5 region** | the same plus tile id of layer 4: **6 region**. The enum `InfoType` has the numbers of MV; the property converts |
| 319 Change Equipment | the slot counts from 0 | the slot counts from 1 (`SlotIndex` hides it; `TypeId` is the raw number). VX Ace names the slots by the type of the actor's equipment (`Weapon 1`, `Weapon 2` with two weapons) |
| 321 Change Class | `[actor, class]` | `[actor, class, saveLevel]` |
| 322 Change Actor Graphic / Images | five parameters | six: the sideview battler (`SwBattler`) is added |
| 337 Show Battle Animation | `[troop member, animation]` (-1 = the whole troop) | `[enemy, animation, entireTroop]` since MV 1.3.5 (older files have two; the third reads as false and the enemy is ignored when it is true) |
| 231/232 Show/Move Picture | blend mode: 0 normal, 1 add, 2 subtract | blend mode: 0 normal, 1 add, 2 multiply, 3 screen (3 exists only in MV: refused for a VX Ace list). Move Picture: the second parameter is nil in VX Ace, 0 in MV |
| 124 Control Timer | start `[0, seconds]`, stop `[1]` (the library writes stop without seconds for both makers) | the same |
| 122 Control Variable | the number of parameters depends on the operand: constant, variable and script 5, random 6, game data 7 | the same |
| sounds (BGM, BGS, ME, SE) | a Ruby object `RPG::BGM` (name, volume, pitch) | a JSON object (name, volume, pitch, **pan**); the pan is MV only |
| names shown in the editors | e.g. "Change Actor Graphic", "Change Vehicle Graphic", "Change Parallax Back" | "Change Actor Images", "Change Vehicle Image", "Change Parallax" (`GetCommandName(version)`) |

A string of VX Ace is a Ruby string: the editor writes it without an encoding flag, the library writes it as UTF-8. This was looked at and left as it is.

## Event commands (code order)

- **0** Empty: ends a block and the list (`EmptyCommand`) (no parameters)
- **101** Text (`ShowTextBaseCommand`): 0 `Face` string [default ""] · 1 `Offset` int · 2 `Background` BackgroundType (Window=0, Dim=1, Transparent=2) · 3 `Position` PositionType (Top=0, Middle=1, Bottom=2) [default 2]
- **102** Show Choices (`ShowChoicesCommand`): 0 `Choices` List<string> [default "Yes", "No"] · 1 `Cancel` CancelType (Disallow=-1, Branch=-2, Choice1=0, Choice2=1, Choice3=2, Choice4=3, Choice5=4, Choice6=5) · 1 `CancelChoiceIndex` int [raw value] · 2 `DefaultChoiceIndex` int [not in VXAce] · 3 `Position` PositionType (Left=0, Middle=1, Right=2) [not in VXAce] · 4 `Background` BackgroundType (Window=0, Dim=1, Transparent=2) [not in VXAce]
- **103** Input Number (`InputNumberCommand`): 0 `VariableId` int [default 1] · 1 `DigitsCount` int [default 1]
- **104** Select Item / Select Key Item (`SelectItemCommand`): 0 `VariableId` int [default 1] · 1 `Type` KeyItemType (Regular=1, Key=2, HiddenA=3, HiddenB=4) [not in VXAce; default KeyItemType.Key, MV]
- **105** Text(S) (`ShowScrollingTextBaseCommand`): 0 `Speed` int [default 2] · 1 `NoFastForward` bool
- **108** Comment (`CommentCommand`): 0 `Comment` string [default ""]
- **111/0** If Switch (`IfSwitchCommand`): 1 `SwitchId` int [default 1] · 2 `State` OnOffState (On=0, Off=1)
- **111/1** If Variable (`IfVariableCommand`): 1 `VariableId` int [default 1] · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int · 4 `Operator` OperatorType (Eq=0, Gte=1, Lte=2, Gt=3, Lt=4, NEq=5)
- **111/2** If SelfSwitch (`IfSelfSwitchCommand`): 1 `Switch` string [default "A"] · 2 `State` OnOffState (On=0, Off=1)
- **111/3** If Timer (`IfTimerCommand`): 1 `Seconds` int · 2 `Operator` OperatorType (Gte=0, Lte=1)
- **111/4** If Actor (`IfActorCommand`): 1 `ActorId` int [default 1] · 2 `Check` CheckType (InParty=0, Name=1, Class=2, Skill=3, WeaponEquipped=4, ArmorEquipped=5, State=6) · 3 `Param` object
- **111/5** If Enemy (`IfEnemyCommand`): 1 `TroopId` int · 2 `Check` CheckType (Appeared=0, State=1) · 3 `Param` int
- **111/6** If Event (`IfEventCommand`): 1 `EventId` int [default -1] · 2 `Direction` Direction (Retain=0, Down=2, Left=4, Right=6, Up=8) [default Direction.Down]
- **111/7** If Gold (`IfGoldCommand`): 1 `Amount` int · 2 `Operator` OperatorType (Gte=0, Lte=1, Lt=2)
- **111/8** If Item (`IfItemCommand`): 1 `ItemId` int [default 1]
- **111/9** If Weapon (`IfWeaponCommand`): 1 `WeaponId` int [default 1] · 2 `IncludeEquipment` bool
- **111/10** If Armor (`IfArmorCommand`): 1 `ArmorId` int [default 1] · 2 `IncludeEquipment` bool
- **111/11** If Button (`IfButtonCommand`): 1 `Button` Buttons (Down=2, Left=4, Right=6, Up=8, A=11, B=12, C=13, X=14, Y=15, Z=16, L=17, R=18) [default Buttons.Down, VXAce; default Buttons.C, MV]
- **111/12** If Script (`IfScriptCommand`): 1 `Script` string [default ""]
- **111/13** If Vehicle (`IfVehicleCommand`): 1 `VehicleType` VehicleType (Boat=0, Ship=1, Airship=2)
- **112** Loop (`LoopCommand`) (no parameters)
- **113** Break Loop (`BreakLoopCommand`) (no parameters)
- **115** Exit Event Processing (`ExitEventProcessingCommand`) (no parameters)
- **117** Common Event / Call Common Event (`CallCommonEventCommand`): 0 `EventId` int [default 1]
- **118** Label (`LabelCommand`): 0 `Label` string
- **119** Jump to Label (`JumpToLabelCommand`): 0 `Label` string
- **121** Control Switches (`ControlSwitchCommand`): 0 `StartSwitchId` int [default 1] · 1 `EndSwitchId` int [default 1] · 2 `State` OnOffState (On=0, Off=1)
- **122** Control Variables (`ControlVariableCommand`): 0 `StartVariable` int [default 1] · 1 `EndVariable` int [default 1] · 2 `Operator` OperatorType (Set=0, Add=1, Dec=2, Mul=3, Div=4, Mod=5) · 3 `Operand` OperandType (Constant=0, Variable=1, Random=2, GameData=3, Script=4) · 4 `Value1` object [default 0] · 5 `Value2` object [default 0] · 6 `Value3` object [default 0]
- **123** Control Self Switch (`ControlSelfSwitchCommand`): 0 `Switch` string [default "A"] · 1 `State` OnOffState (On=0, Off=1)
- **124** Control Timer (`ControlTimerCommand`): 0 `State` OnOffState (On=0, Off=1) · 1 `Seconds` int
- **125** Change gold (`ChangeGoldCommand`): 0 `Operator` OperatorType (Increase=0, Decrease=1) · 1 `Operand` OperandType (Constant=0, Variable=1) · 2 `Value` int [default 1]
- **126** Change items / Change Items (`ChangeItemsCommand`): 0 `ItemId` int [default 1] · 1 `Operator` OperatorType (Add=0, Take=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1]
- **127** Change Weapons (`ChangeWeaponsCommand`): 0 `WeaponId` int [default 1] · 1 `Operator` OperatorType (Add=0, Take=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1] · 4 `IncludeEquipment` bool
- **128** Change Armors / Change Armor (`ChangeArmorsCommand`): 0 `ArmorId` int [default 1] · 1 `Operator` OperatorType (Add=0, Take=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1] · 4 `IncludeEquipment` bool
- **129** Change Party Member (`ChangePartyMemberCommand`): 0 `ActorId` int [default 1] · 1 `Type` ChangeType (Add=0, Remove=1) · 2 `DoInitialize` bool
- **132** Change Battle BGM (`ChangeBattleBGMCommand`): 0 `SoundData` IBGM
- **133** Change Victory ME / Change Battle End ME (`ChangeVictoryMECommand`): 0 `SoundData` IME
- **134** Change Save Access (`ChangeSaveAccessCommand`): 0 `State` DisableEnableState (Disable=0, Enable=1)
- **135** Change Menu Access (`ChangeMenuAccessCommand`): 0 `State` DisableEnableState (Disable=0, Enable=1)
- **136** Change Encounter (`ChangeEncounterCommand`): 0 `State` DisableEnableState (Disable=0, Enable=1)
- **137** Change Formation Access (`ChangeFormationAccessCommand`): 0 `State` DisableEnableState (Disable=0, Enable=1)
- **138** Change Window Color (`ChangeWindowColorCommand`): 0 `Tone` ITone [default -34, 0, 68, 0, VXAce; default 0, 0, 0, 0]
- **139** Change Defeat ME (`ChangeDefeatME`): 0 `SoundData` IME
- **140** Change Vehicle BGM (`ChangeVehicleBGMCommand`): 0 `VehicleType` VehicleType (Boat=0, Ship=1, Airship=2) · 1 `SoundData` IBGM
- **201** Transfer Player (`TransferPlayerCommand`): 0 `Designation` DesignationType (Direct=0, ByVariable=1) · 1 `Param1` int [default 1] · 2 `Param2` int · 3 `Param3` int · 4 `Direction` Direction (Retain=0, Down=2, Left=4, Right=6, Up=8) · 5 `Fade` FadeType (Black=0, Normal=0, White=1, None=2)
- **202** Set Vehicle Location (`SetVehicleLocationCommand`): 0 `VehicleType` VehicleType (Boat=0, Ship=1, Airship=2) · 1 `Designation` DesignationType (Direct=0, ByVariable=1) · 2 `Param1` int [default 1] · 3 `Param2` int · 4 `Param3` int
- **203** Set Event Location (`SetEventLocationCommand`): 0 `EventId` int · 1 `Designation` DesignationType (Direct=0, ByVariable=1, Exchange=2) · 2 `Param1` int · 3 `Param2` int · 4 `Direction` Direction (Retain=0, Down=2, Left=4, Right=6, Up=8)
- **204** Scroll Map (`ScrollMapCommand`): 0 `Direction` Direction (Retain=0, Down=2, Left=4, Right=6, Up=8) [default 2] · 1 `Distance` int [default 1] · 2 `Speed` int [default 4]
- **205** Set Movement Route (`SetMovementRouteCommand`): 0 `EventId` int · 1 `Route` IMoveRoute
- **206** Get on/off Vehicle (`GetOnOffVehicleCommand`) (no parameters)
- **211** Change Transparency (`ChangeTransparencyCommand`): 0 `State` OnOffState (On=0, Off=1)
- **212** Show Animation (`ShowAnimationCommand`): 0 `EventId` int [default -1] · 1 `AnimationId` int [default 1] · 2 `WaitForCompletion` bool
- **213** Show Balloon Icon (`ShowBalloonCommand`): 0 `EventId` int [default -1] · 1 `Icon` IconType (IconType: 15 values, see source) [default 1] · 2 `WaitForCompletion` bool
- **214** Erase Event (`EraseEventCommand`) (no parameters)
- **216** Change Player Followers (`ChangePlayerFollowers`): 0 `State` OnOffState (On=0, Off=1)
- **217** Gather Followers (`GatherFollowersCommand`) (no parameters)
- **221** Fadeout Screen (`FadeoutScreenCommand`) (no parameters)
- **222** Fadein Screen (`FadeinScreenCommand`) (no parameters)
- **223** Tint Screen (`TintScreenCommand`): 0 `Tone` ITone [default 0, 0, 0, 0] · 1 `Duration` int [default 60] · 2 `WaitForCompletion` bool [default true]
- **224** Flash Screen (`FlashScreenCommand`): 0 `Color` IColor [default 255, 255, 255, 255, VXAce; default 255, 255, 255, 170, MV] · 1 `Duration` int [default 60] · 2 `WaitForCompletion` bool [default true]
- **225** Shake Screen (`ShakeScreenCommand`): 0 `Power` int [default 5] · 1 `Speed` int [default 5] · 2 `Duration` int [default 60] · 3 `WaitForCompletion` bool [default true]
- **230** Wait (`WaitCommand`): 0 `FrameCount` int [default 60]
- **231** Show picture (`ShowPictureCommand`): 0 `Layer` int [default 1] · 1 `Picture` string [default ""] · 2 `Reference` ReferenceType (UpperLeft=0, Center=1) · 3 `Designation` DesignationType (Direct=0, ByVariable=1) · 4 `Param1` int · 5 `Param2` int · 6 `Width` int [default 100] · 7 `Height` int [default 100] · 8 `Transparency` int [default 255] · 9 `BlendMode` BlendMode (Normal=0, Add=1, Sub=2, Multiply=2, Screen=3)
- **232** Move picture (`MovePictureCommand`): 0 `Layer` int [default 1] · 2 `Reference` ReferenceType (UpperLeft=0, Center=1) · 3 `Designation` DesignationType (Direct=0, ByVariable=1) · 4 `Param1` int · 5 `Param2` int · 6 `Width` int [default 100] · 7 `Height` int [default 100] · 8 `Transparency` int [default 255] · 9 `BlendMode` BlendMode (Normal=0, Add=1, Sub=2, Multiply=2, Screen=3) · 10 `Frames` int [default 60] · 11 `WaitCompletion` bool [default true]
- **233** Rotate Picture (`RotatePictureCommand`): 0 `Layer` int [default 1] · 1 `Speed` int [default 5, VXAce]
- **234** Tint Picture (`TintPictureCommand`): 0 `Layer` int [default 1] · 1 `Tone` ITone [default 0, 0, 0, 0] · 2 `Duration` int [default 60] · 3 `WaitForCompletion` bool [default true]
- **235** Erase Picture (`ErasePictureCommand`): 0 `Layer` int [default 1]
- **236** Set Weather Effect / Set Weather Effects (`SetWeatherEffectCommand`): 0 `EffectType` string [default TypeNone] · 1 `Power` int [default 9, VXAce; default 5, MV] · 2 `Duration` int [default 60, MV] · 3 `WaitForCompletion` bool [default true]
- **241** Play BGM (`PlayBGMCommand`): 0 `SoundData` IBGM
- **242** Fadeout BGM (`FadeoutBGMCommand`): 0 `Seconds` int [default 10]
- **243** Save BGM (`SaveBGMCommand`) (no parameters)
- **244** Replay BGM (`ReplayBGMCommand`) (no parameters)
- **245** Play BGS (`PlayBGSCommand`): 0 `SoundData` IBGS
- **246** Fadeout BGS (`FadeoutBGSCommand`): 0 `Seconds` int [default 10]
- **249** Play ME (`PlayMECommand`): 0 `SoundData` IME
- **250** Play SE (`PlaySECommand`): 0 `SoundData` ISE
- **251** Stop SE (`StopSECommand`) (no parameters)
- **261** Play Movie (`PlayMovieCommand`): 0 `Movie` string [default ""]
- **281** Change Map Name Display (`ChangeMapNameDisplayCommand`): 0 `State` OnOffState (On=0, Off=1)
- **282** Change Tileset (`ChangeTilesetCommand`): 0 `TileSetId` int [default 1]
- **283** Change Battle Back (`ChangeBattleBackground`): 0 `Image1` string [default ""] · 1 `Image2` string [default ""]
- **284** Change Parallax / Change Parallax Back (`ChangeParallaxCommand`): 0 `Image` string [default ""] · 1 `LoopHorizontally` bool · 2 `LoopVertically` bool · 3 `HorizontalScroll` int · 4 `VerticalScroll` int
- **285** Get Location Info (`GetLocationInfoCommand`): 0 `VariableId` int [default 1] · 1 `Type` InfoType (TerrainTag=0, EventId=1, TileIdLayer1=2, TileIdLayer2=3, TileIdLayer3=4, TileIdLayer4=5, Region=6) · 2 `Designation` DesignationType (Direct=0, ByVariables=1) · 3 `Param1` int · 4 `Param2` int
- **301** Battle Processing (`BattleProcessingCommand`): 0 `DsgType` DesignationType (Direct=0, ByVariable=1, SameAsRandomEncounter=2) · 1 `SourceId` int [default 1] · 2 `CanEscape` bool · 3 `CanLoose` bool
- **302** Shop Processing (`ShopProcessingCommand`): 0 `Type` ItemType (Item=0, Weapon=1, Armor=2) · 1 `ItemId` int · 2 `HasCustomPrice` bool · 3 `CustomPrice` int · 4 `PurchaseOnly` bool
- **303** Name Input Processing (`NameInputProcessingCommand`): 0 `ActorId` int [default 1] · 1 `MaxCharacters` int [default 6, VXAce; default 8, MV]
- **311** Change HP (`ChangeHpCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Increase=0, Decrease=1) · 3 `Operand` OperandType (Constant=0, Variable=1) · 4 `Value` int [default 1] · 5 `AllowKnockout` bool
- **312** Change MP (`ChangeMpCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Increase=0, Decrease=1) · 3 `Operand` OperandType (Constant=0, Variable=1) · 4 `Value` int [default 1]
- **313** Change State (`ChangeStateCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Add=0, Remove=1) · 3 `StateId` int [default 1]
- **314** Recover All (`RecoverAllCommand`): 0 `Source` SourceType (Actor=0, Variable=1) · 1 `SourceId` int
- **315** Change experience / Change EXP (`ChangeExperienceCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Increase=0, Decrease=1) · 3 `Operand` OperandType (Constant=0, Variable=1) · 4 `Value` int [default 1] · 5 `ShowLevelUp` bool
- **316** Change level (`ChangeLevelCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Increase=0, Decrease=1) · 3 `Operand` OperandType (Constant=0, Variable=1) · 4 `Value` int [default 1] · 5 `ShowLevelUp` bool
- **317** Change Parameter / Change Parameters (`ChangeParameterCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int [default 1, VXAce] · 2 `ActionParameter` ActionParameterType (MaxHp=0, MaxMp=1, Attack=2, Defense=3, MAttack=4, MDefense=5, Agility=6, Luck=7) · 3 `Operator` OperatorType (Increase=0, Decrease=1) · 4 `Operand` OperandType (Constant=0, Variable=1) · 5 `Value` int [default 1]
- **318** Change Skill / Change Skills (`ChangeSkillCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int [default 1, VXAce] · 2 `Operator` OperatorType (Learn=0, Forget=1) · 3 `SkillId` int [default 1]
- **319** Change Equipment (`ChangeEquipmentCommand`): 0 `ActorId` int [default 1] · 1 `TypeId` int [raw value] · 1 `SlotIndex` int · 2 `ItemId` int
- **320** Change Name (`ChangeNameCommand`): 0 `ActorId` int [default 1] · 1 `Name` string [default ""]
- **321** Change Class (`ChangeClassCommand`): 0 `ActorId` int [default 1] · 1 `ClassId` int [default 1] · 2 `SaveLevel` bool [not in VXAce]
- **322** Change Actor Images / Change Actor Graphic (`ChangeActorImagesCommand`): 0 `ActorId` int [default 1] · 1 `CharacterImage` string [default ""] · 2 `CharacterImageOffset` int · 3 `FaceImage` string [default ""] · 4 `FaceImageOffset` int · 5 `SwBattler` string [default "", MV]
- **323** Change Vehicle Image / Change Vehicle Graphic (`ChangeVehicleImageCommand`): 0 `VehicleType` VehicleType (Boat=0, Ship=1, Airship=2) · 1 `Image` string [default ""] · 2 `Offset` int
- **324** Change Nickname / Change Actor Nickname (`ChangeActorNicknameCommand`): 0 `ActorId` int [default 1] · 1 `NickName` string [default ""]
- **325** Change Profile (`ChangeActorProfileCommand`): 0 `ActorId` int [default 1] · 1 `Text` string [default ""]
- **326** Change TP (`ChangeTpCommand`): 0 `SourceT` SourceType (Actor=0, Variable=1) · 1 `SourceId` int · 2 `Operator` OperatorType (Increase=0, Decrease=1) · 3 `Operand` OperandType (Constant=0, Variable=1) · 4 `Value` int [default 1]
- **331** Change Enemy HP (`ChangeEnemyHpCommand`): 0 `TroopId` int [default -1] · 1 `Operator` OperatorType (Increase=0, Decrease=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1] · 4 `AllowKnockout` bool
- **332** Change Enemy MP (`ChangeEnemyMpCommand`): 0 `TroopId` int [default -1] · 1 `Operator` OperatorType (Increase=0, Decrease=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1]
- **333** Change Enemy State (`ChangeEnemyStateCommand`): 0 `TroopId` int [default -1] · 1 `Operator` OperatorType (Add=0, Remove=1) · 2 `StateId` int [default 1]
- **334** Enemy Recover All (`EnemyRecoverAllCommand`): 0 `TroopId` int [default -1]
- **335** Enemy Appear (`EnemyAppearCommand`): 0 `TroopId` int
- **336** Enemy Transform (`EnemyTransformCommand`): 0 `TroopId` int · 1 `EnemyId` int [default 1]
- **337** Show Battle Animation (`ShowBattleAnimationCommand`): 0 `TroopId` int [default -1, VXAce] · 1 `AnimationId` int [default 1] · 2 `EntireTroop` bool [default false, MV]
- **339** Force Action (`ForceActionCommand`): 0 `Subject` SubjectType (Enemy=0, Actor=1) · 1 `SubjectId` int · 2 `SkillId` int [default 1] · 3 `Target` TargetType (LastTarget=-2, Random=-1, Index1=0, Index2=1, Index3=2, Index4=3, Index5=4, Index6=5, Index7=6, Index8=7) [default -1, VXAce; default -2, MV]
- **340** Abort Battle (`AbortBattleCommand`) (no parameters)
- **342** Change enemy TP (`ChangeEnemyTpCommand`): 0 `TroopId` int [default -1] · 1 `Operator` OperatorType (Increase=0, Decrease=1) · 2 `Operand` OperandType (Constant=0, Variable=1) · 3 `Value` int [default 1]
- **351** Open Menu Screen (`OpenMenuScreenCommand`) (no parameters)
- **352** Open Save Screen (`OpenSaveScreenCommand`) (no parameters)
- **353** Game Over (`GameOverCommand`) (no parameters)
- **354** Return to Title Screen (`ReturnToTitleCommand`) (no parameters)
- **355** Script (`ScriptCommand`): 0 `Script` string [default ""]
- **356** Plugin Command (`PluginCommand`): 0 `Command` string [default ""]
- **401** (continuation line) (`ShowTextLineCommand`): 0 `Text` string
- **402** When (`ChoiceCaseCommand`): 0 `ChoiceIndex` int · 1 `ChoiceName` string
- **403** When (`ChoiceCaseCancelCommand`): 0 `Unk0` object · 1 `Unk1` object (meaning unknown: MV writes `[6, null]`, VX Ace nothing)
- **404** End / Branch End (`EndChoicesCommand`) (no parameters)
- **405** (continuation line) (`ShowScrollingTextLine`): 0 `Text` string
- **408** (continuation line) (`CommentContinueCommand`): 0 `Comment` string
- **411** Else (`ElseCommand`) (no parameters)
- **412** End / Branch End (`EndIfCommand`) (no parameters)
- **413** Repeat Above (`EndLoopCommand`) (no parameters)
- **505** Movement route line (`MovementRouteCommand`): 0 `MoveCommand` NamedCommand
- **601** If Win (`BattleProcessingIfWinCommand`) (no parameters)
- **602** If Escape (`BattleProcessingIfEscapeCommand`) (no parameters)
- **603** If Lose (`BattleProcessingIfLoseCommand`) (no parameters)
- **604** End / Branch End (`BattleProcessingEndCommand`) (no parameters)
- **605** (continuation line) (`ShopProcessingLineCommand`): 0 `Type` ShopProcessingCommand.ItemType (Item=0, Weapon=1, Armor=2) · 1 `ItemId` int · 2 `HasCustomPrice` bool · 3 `CustomPrice` int
- **655** (continuation line) (`ScriptLineCommand`): 0 `Text` string

## Movement route commands

- **route 0** Empty (`MoveEmptyCommand`) (no parameters)
- **route 1** Move Down (`MoveDownCommand`) (no parameters)
- **route 2** Move Left (`MoveLeftCommand`) (no parameters)
- **route 3** Move Right (`MoveRightCommand`) (no parameters)
- **route 4** Move Up (`MoveUpCommand`) (no parameters)
- **route 5** Move Lower Left (`MoveLowerLeftCommand`) (no parameters)
- **route 6** Move Lower Right (`MoveLowerRightCommand`) (no parameters)
- **route 7** Move Upper Left (`MoveUpperLeftCommand`) (no parameters)
- **route 8** Move Upper Right (`MoveUpperRightCommand`) (no parameters)
- **route 9** Move at Random (`MoveRandomCommand`) (no parameters)
- **route 10** Move toward Player (`MoveTowardPlayerCommand`) (no parameters)
- **route 11** Move away from Player (`MoveAwayFromPlayerCommand`) (no parameters)
- **route 12** 1 Step Forward (`MoveForwardCommand`) (no parameters)
- **route 13** 1 Step Backward (`MoveBackwardCommand`) (no parameters)
- **route 14** Jump (`MoveJumpCommand`): 0 `XDelta` int · 1 `YDelta` int
- **route 15** Wait (`MoveWaitCommand`): 0 `Frames` int
- **route 16** Turn Down (`MoveTurnDownCommand`) (no parameters)
- **route 17** Turn Left (`MoveTurnLeftCommand`) (no parameters)
- **route 18** Turn Right (`MoveTurnRightCommand`) (no parameters)
- **route 19** Turn Up (`MoveTurnUpCommand`) (no parameters)
- **route 20** Turn 90° Right (`MoveTurn90DegreesRightCommand`) (no parameters)
- **route 21** Turn 90° Left (`MoveTurn90DegreesLeftCommand`) (no parameters)
- **route 22** Turn 180° (`MoveTurn180DegreesCommand`) (no parameters)
- **route 23** Turn 90° Right or Left (`MoveTurn90DegreesRightOrLeftCommand`) (no parameters)
- **route 24** Turn at Random (`MoveTurnAtRandomCommand`) (no parameters)
- **route 25** Turn toward Player (`MoveTurnTowardPlayerCommand`) (no parameters)
- **route 26** Turn away from Player (`MoveTurnAwayFromPlayerCommand`) (no parameters)
- **route 27** Switch ON (`MoveSwitchOnCommand`): 0 `SwitchId` int
- **route 28** Switch OFF (`MoveSwitchOffCommand`): 0 `SwitchId` int
- **route 29** Speed (`MoveChangeSpeedCommand`): 0 `Speed` SpeedType (Slowerx8=1, Slowerx4=2, Slowerx2=3, Normal=4, Fasterx2=5, Fasterx4=6)
- **route 30** Frequency (`MoveChangeFrequencyCommand`): 0 `Frequency` FrequencyType (Lowest=1, Lower=2, Normal=3, Higher=4, Highest=5)
- **route 31** Walking Animation ON (`MoveWalkingAnimationOnCommand`) (no parameters)
- **route 32** Walking Animation OFF (`MoveWalkingAnimationOffCommand`) (no parameters)
- **route 33** Stepping Animation ON (`MoveSteppingAnimationOnCommand`) (no parameters)
- **route 34** Stepping Animation OFF (`MoveSteppingAnimationOffCommand`) (no parameters)
- **route 35** Direction Fix ON (`MoveDirectionFixOnCommand`) (no parameters)
- **route 36** Direction Fix OFF (`MoveDirectionFixOffCommand`) (no parameters)
- **route 37** Through ON (`MoveThroughOnCommand`) (no parameters)
- **route 38** Through OFF (`MoveThroughOffCommand`) (no parameters)
- **route 39** Transparent ON (`MoveTransparentOnCommand`) (no parameters)
- **route 40** Transparent OFF (`MoveTransparentOffCommand`) (no parameters)
- **route 41** Image / Graphic (`MoveChangeImageCommand`): 0 `Image` string · 1 `Offset` int
- **route 42** Opacity / Change Opacity (`MoveChangeOpacityCommand`): 0 `Opacity` int
- **route 43** Blend Mode / Change Blending (`MoveChangeBlendModeCommand`): 0 `BlendMode` BlendMode (Normal=0, Add=1, Sub=2, Multiply=2, Screen=3)
- **route 44** SE (`MovePlaySeCommand`): 0 `SoundData` ISE
- **route 45** Script (`MoveScriptCommand`): 0 `Script` string
