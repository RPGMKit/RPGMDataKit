using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class CommandFactory
{
    public static NamedCommand Create(ICommand command, StorageVersion version)
    {
        NamedCommand returnCommand;
        var parameterProvider = command.ParameterProvider;

        switch (command.Code)
        {
            case CommandCodes.Empty:
                returnCommand = new EmptyCommand();
                break;
            case CommandCodes.Comment:
                returnCommand = new CommentCommand();
                break;
            case CommandCodes.If:
                returnCommand = IfCommand.Factory(command.ParameterProvider.GetParameter<IfCommand.IfType>(0));
                break;
            case CommandCodes.CommentContinue:
                returnCommand = new CommentContinueCommand();
                break;
            case CommandCodes.ShowTextBase:
                returnCommand = new ShowTextBaseCommand();
                break;
            case CommandCodes.ShowTextLine:
                returnCommand = new ShowTextLineCommand();
                break;
            case CommandCodes.ControlSwitches:
                returnCommand = new ControlSwitchCommand();
                break;
            case CommandCodes.ControlVariables:
                returnCommand = new ControlVariableCommand();
                break;
            case CommandCodes.ControlSelfSwitch:
                returnCommand = new ControlSelfSwitchCommand();
                break;
            case CommandCodes.ControlTimer:
                returnCommand = new ControlTimerCommand();
                break;
            case CommandCodes.Loop:
                returnCommand = new LoopCommand();
                break;
            case CommandCodes.BreakLoop:
                returnCommand = new BreakLoopCommand();
                break;
            case CommandCodes.InputNumber:
                returnCommand = new InputNumberCommand();
                break;
            case CommandCodes.SelectItem:
                returnCommand = new SelectItemCommand();
                break;
            case CommandCodes.ShowScrollingTextBase:
                returnCommand = new ShowScrollingTextBaseCommand();
                break;
            case CommandCodes.ChangeGold:
                returnCommand = new ChangeGoldCommand();
                break;
            case CommandCodes.ChangeItems:
                returnCommand = new ChangeItemsCommand();
                break;
            case CommandCodes.ChangeWeapons:
                returnCommand = new ChangeWeaponsCommand();
                break;
            case CommandCodes.ChangeArmors:
                returnCommand = new ChangeArmorsCommand();
                break;
            case CommandCodes.ChangePartyMember:
                returnCommand = new ChangePartyMemberCommand();
                break;
            case CommandCodes.ChangeBattleBGM:
                returnCommand = new ChangeBattleBGMCommand();
                break;
            case CommandCodes.ChangeVictoryME:
                returnCommand = new ChangeVictoryMECommand();
                break;
            case CommandCodes.ChangeSaveAccess:
                returnCommand = new ChangeSaveAccessCommand();
                break;
            case CommandCodes.ChangeMenuAccess:
                returnCommand = new ChangeMenuAccessCommand();
                break;
            case CommandCodes.ChangeEncounter:
                returnCommand = new ChangeEncounterCommand();
                break;
            case CommandCodes.ChangeFormationAccess:
                returnCommand = new ChangeFormationAccessCommand();
                break;
            case CommandCodes.ChangeWindowColor:
                returnCommand = new ChangeWindowColorCommand();
                break;
            case CommandCodes.ChangeDefeatME:
                returnCommand = new ChangeDefeatME();
                break;
            case CommandCodes.ChangeVehicleBGM:
                returnCommand = new ChangeVehicleBGMCommand();
                break;
            case CommandCodes.ChangeHp:
                returnCommand = new ChangeHpCommand();
                break;
            case CommandCodes.ChangeMp:
                returnCommand = new ChangeMpCommand();
                break;
            case CommandCodes.ChangeTp:
                returnCommand = new ChangeTpCommand();
                break;
            case CommandCodes.ChangeState:
                returnCommand = new ChangeStateCommand();
                break;
            case CommandCodes.RecoverAll:
                returnCommand = new RecoverAllCommand();
                break;
            case CommandCodes.ChangeExp:
                returnCommand = new ChangeExperienceCommand();
                break;
            case CommandCodes.ChangeLevel:
                returnCommand = new ChangeLevelCommand();
                break;
            case CommandCodes.ChangeParameter:
                returnCommand = new ChangeParameterCommand();
                break;
            case CommandCodes.ChangeSkill:
                returnCommand = new ChangeSkillCommand();
                break;
            case CommandCodes.ChangeEquipment:
                returnCommand = new ChangeEquipmentCommand();
                break;
            case CommandCodes.ChangeName:
                returnCommand = new ChangeNameCommand();
                break;
            case CommandCodes.ChangeClass:
                returnCommand = new ChangeClassCommand();
                break;
            case CommandCodes.ChangeActorImages:
                returnCommand = new ChangeActorImagesCommand();
                break;
            case CommandCodes.ChangeVehicleImage:
                returnCommand = new ChangeVehicleImageCommand();
                break;
            case CommandCodes.ChangeActorNickname:
                returnCommand = new ChangeActorNicknameCommand();
                break;
            case CommandCodes.ChangeActorProfile:
                returnCommand = new ChangeActorProfileCommand();
                break;
            case CommandCodes.TransferPlayer:
                returnCommand = new TransferPlayerCommand();
                break;
            case CommandCodes.SetVehicleLocation:
                returnCommand = new SetVehicleLocationCommand();
                break;
            case CommandCodes.SetEventLocation:
                returnCommand = new SetEventLocationCommand();
                break;
            case CommandCodes.ScrollMap:
                returnCommand = new ScrollMapCommand();
                break;
            case CommandCodes.ShowPicture:
                returnCommand = new ShowPictureCommand();
                break;
            case CommandCodes.MovePicture:
                returnCommand = new MovePictureCommand();
                break;
            case CommandCodes.RotatePicture:
                returnCommand = new RotatePictureCommand();
                break;
            case CommandCodes.TintPicture:
                returnCommand = new TintPictureCommand();
                break;
            case CommandCodes.ErasePicture:
                returnCommand = new ErasePictureCommand();
                break;
            case CommandCodes.SetWeatherEffect:
                returnCommand = new SetWeatherEffectCommand();
                break;
            case CommandCodes.BattleProcessing:
                returnCommand = new BattleProcessingCommand();
                break;
            case CommandCodes.ShopProcessing:
                returnCommand = new ShopProcessingCommand();
                break;
            case CommandCodes.NameInputProcessing:
                returnCommand = new NameInputProcessingCommand();
                break;
            case CommandCodes.GetLocationInfo:
                returnCommand = new GetLocationInfoCommand();
                break;
            case CommandCodes.ChangeEnemyHp:
                returnCommand = new ChangeEnemyHpCommand();
                break;
            case CommandCodes.ChangeEnemyMp:
                returnCommand = new ChangeEnemyMpCommand();
                break;
            case CommandCodes.ChangeEnemyState:
                returnCommand = new ChangeEnemyStateCommand();
                break;
            case CommandCodes.EnemyRecoverAll:
                returnCommand = new EnemyRecoverAllCommand();
                break;
            case CommandCodes.EnemyAppear:
                returnCommand = new EnemyAppearCommand();
                break;
            case CommandCodes.EnemyTransform:
                returnCommand = new EnemyTransformCommand();
                break;
            case CommandCodes.ShowBattleAnimation:
                returnCommand = new ShowBattleAnimationCommand();
                break;
            case CommandCodes.ForceAction:
                returnCommand = new ForceActionCommand();
                break;
            case CommandCodes.AbortBattle:
                returnCommand = new AbortBattleCommand();
                break;
            case CommandCodes.ChangeEnemyTp:
                returnCommand = new ChangeEnemyTpCommand();
                break;
            case CommandCodes.OpenMenuScreen:
                returnCommand = new OpenMenuScreenCommand();
                break;
            case CommandCodes.OpenSaveScreen:
                returnCommand = new OpenSaveScreenCommand();
                break;
            case CommandCodes.GameOver:
                returnCommand = new GameOverCommand();
                break;
            case CommandCodes.ReturnToTitle:
                returnCommand = new ReturnToTitleCommand();
                break;
            case CommandCodes.Else:
                returnCommand = new ElseCommand();
                break;
            case CommandCodes.EndIf:
                returnCommand = new EndIfCommand();
                break;
            case CommandCodes.EndLoop:
                returnCommand = new EndLoopCommand();
                break;
            case CommandCodes.MovementRouteCommand:
                returnCommand = new MovementRouteCommand();
                break;
            case CommandCodes.CallCommonEvent:
                returnCommand = new CallCommonEventCommand();
                break;
            case CommandCodes.Script:
                returnCommand = new ScriptCommand();
                break;
            case CommandCodes.PluginCommand:
                returnCommand = new PluginCommand();
                break;
            case CommandCodes.FadeoutScreen:
                returnCommand = new FadeoutScreenCommand();
                break;
            case CommandCodes.FadeinScreen:
                returnCommand = new FadeinScreenCommand();
                break;
            case CommandCodes.TintScreen:
                returnCommand = new TintScreenCommand();
                break;
            case CommandCodes.FlashScreen:
                returnCommand = new FlashScreenCommand();
                break;
            case CommandCodes.ShakeScreen:
                returnCommand = new ShakeScreenCommand();
                break;
            case CommandCodes.Wait:
                returnCommand = new WaitCommand();
                break;
            case CommandCodes.PlayBgm:
                returnCommand = new PlayBGMCommand();
                break;
            case CommandCodes.SaveBgm:
                returnCommand = new SaveBGMCommand();
                break;
            case CommandCodes.ReplayBgm:
                returnCommand = new ReplayBGMCommand();
                break;
            case CommandCodes.PlayBgs:
                returnCommand = new PlayBGSCommand();
                break;
            case CommandCodes.FadeoutBgs:
                returnCommand = new FadeoutBGSCommand();
                break;
            case CommandCodes.ShowChoices:
                returnCommand = new ShowChoicesCommand();
                break;
            case CommandCodes.ChoiceCase:
                returnCommand = new ChoiceCaseCommand();
                break;
            case CommandCodes.ChoiceCaseCancel:
                returnCommand = new ChoiceCaseCancelCommand();
                break;
            case CommandCodes.ChoicesEnd:
                returnCommand = new EndChoicesCommand();
                break;
            case CommandCodes.ShowScrollingTextLine:
                returnCommand = new ShowScrollingTextLine();
                break;
            case CommandCodes.Label:
                returnCommand = new LabelCommand();
                break;
            case CommandCodes.JumpToLabel:
                returnCommand = new JumpToLabelCommand();
                break;
            case CommandCodes.FadeoutBgm:
                returnCommand = new FadeoutBGMCommand();
                break;
            case CommandCodes.PlayMe:
                returnCommand = new PlayMECommand();
                break;
            case CommandCodes.PlaySe:
                returnCommand = new PlaySECommand();
                break;
            case CommandCodes.StopSE:
                returnCommand = new StopSECommand();
                break;
            case CommandCodes.PlayMovie:
                returnCommand = new PlayMovieCommand();
                break;
            case CommandCodes.ChangeMapNameDisplay:
                returnCommand = new ChangeMapNameDisplayCommand();
                break;
            case CommandCodes.ChangeTileset:
                returnCommand = new ChangeTilesetCommand();
                break;
            case CommandCodes.ChangeBattleBackground:
                returnCommand = new ChangeBattleBackground();
                break;
            case CommandCodes.ChangeParallaxBackground:
                returnCommand = new ChangeParallaxCommand();
                break;
            case CommandCodes.ExitEventProcessing:
                returnCommand = new ExitEventProcessingCommand();
                break;
            case CommandCodes.ShowBaloonIcon:
                returnCommand = new ShowBalloonCommand();
                break;
            case CommandCodes.EraseEvent:
                returnCommand = new EraseEventCommand();
                break;
            case CommandCodes.ChangePlayerFollowers:
                returnCommand = new ChangePlayerFollowers();
                break;
            case CommandCodes.GatherFollowers:
                returnCommand = new GatherFollowersCommand();
                break;
            case CommandCodes.SetMovementRoute:
                returnCommand = new SetMovementRouteCommand();
                break;
            case CommandCodes.GetOnOffVehicle:
                returnCommand = new GetOnOffVehicleCommand();
                break;
            case CommandCodes.BattleProcessingIfWin:
                returnCommand = new BattleProcessingIfWinCommand();
                break;
            case CommandCodes.BattleProcessingIfEscape:
                returnCommand = new BattleProcessingIfEscapeCommand();
                break;
            case CommandCodes.BattleProcessingIfLose:
                returnCommand = new BattleProcessingIfLoseCommand();
                break;
            case CommandCodes.BattleProcessingEnd:
                returnCommand = new BattleProcessingEndCommand();
                break;
            case CommandCodes.ChangeTransparency:
                returnCommand = new ChangeTransparencyCommand();
                break;
            case CommandCodes.ShowAnimation:
                returnCommand = new ShowAnimationCommand();
                break;
            case CommandCodes.ShopProcessingLine:
                returnCommand = new ShopProcessingLineCommand();
                break;
            case CommandCodes.ScriptLineCommand:
                returnCommand = new ScriptLineCommand();
                break;
            default:
                returnCommand = new UnknownCommand(command.Code, parameterProvider);
                break;
        }

        returnCommand.Unpack(parameterProvider, version);

        returnCommand.Indent = command.Indent;

        return returnCommand;
    }
}
