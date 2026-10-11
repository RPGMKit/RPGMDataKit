using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;

public static class MovementCommandFactory
{
    public static NamedCommand Create(IMoveCommand command, StorageVersion version)
    {
        NamedCommand returnCommand;

        switch (command.Code)
        {
            case MovementRouteCommandCodes.End:
                returnCommand = new MoveEmptyCommand();
                break;
            case MovementRouteCommandCodes.MoveDown:
                returnCommand = new MoveDownCommand();
                break;
            case MovementRouteCommandCodes.MoveLeft:
                returnCommand = new MoveLeftCommand();
                break;
            case MovementRouteCommandCodes.MoveRight:
                returnCommand = new MoveRightCommand();
                break;
            case MovementRouteCommandCodes.MoveUp:
                returnCommand = new MoveUpCommand();
                break;
            case MovementRouteCommandCodes.MoveLowerLeft:
                returnCommand = new MoveLowerLeftCommand();
                break;
            case MovementRouteCommandCodes.MoveLowerRight:
                returnCommand = new MoveLowerRightCommand();
                break;
            case MovementRouteCommandCodes.MoveUpperLeft:
                returnCommand = new MoveUpperLeftCommand();
                break;
            case MovementRouteCommandCodes.MoveUpperRight:
                returnCommand = new MoveUpperRightCommand();
                break;
            case MovementRouteCommandCodes.MoveRandom:
                returnCommand = new MoveRandomCommand();
                break;
            case MovementRouteCommandCodes.MoveTowardPlayer:
                returnCommand = new MoveTowardPlayerCommand();
                break;
            case MovementRouteCommandCodes.MoveAwayFromPlayer:
                returnCommand = new MoveAwayFromPlayerCommand();
                break;
            case MovementRouteCommandCodes.MoveForward:
                returnCommand = new MoveForwardCommand();
                break;
            case MovementRouteCommandCodes.MoveBackward:
                returnCommand = new MoveBackwardCommand();
                break;
            case MovementRouteCommandCodes.Jump:
                returnCommand = new MoveJumpCommand();
                break;
            case MovementRouteCommandCodes.Wait:
                returnCommand = new MoveWaitCommand();
                break;
            case MovementRouteCommandCodes.TurnDown:
                returnCommand = new MoveTurnDownCommand();
                break;
            case MovementRouteCommandCodes.TurnLeft:
                returnCommand = new MoveTurnLeftCommand();
                break;
            case MovementRouteCommandCodes.TurnRight:
                returnCommand = new MoveTurnRightCommand();
                break;
            case MovementRouteCommandCodes.TurnUp:
                returnCommand = new MoveTurnUpCommand();
                break;
            case MovementRouteCommandCodes.Turn90DegreesRight:
                returnCommand = new MoveTurn90DegreesRightCommand();
                break;
            case MovementRouteCommandCodes.Turn90DegreesLeft:
                returnCommand = new MoveTurn90DegreesLeftCommand();
                break;
            case MovementRouteCommandCodes.Turn180Degrees:
                returnCommand = new MoveTurn180DegreesCommand();
                break;
            case MovementRouteCommandCodes.Turn90DegreesRightOrLeft:
                returnCommand = new MoveTurn90DegreesRightOrLeftCommand();
                break;
            case MovementRouteCommandCodes.TurnAtRandom:
                returnCommand = new MoveTurnAtRandomCommand();
                break;
            case MovementRouteCommandCodes.TurnTowardPlayer:
                returnCommand = new MoveTurnTowardPlayerCommand();
                break;
            case MovementRouteCommandCodes.TurnAwayFromPlayer:
                returnCommand = new MoveTurnAwayFromPlayerCommand();
                break;
            case MovementRouteCommandCodes.SwitchOn:
                returnCommand = new MoveSwitchOnCommand();
                break;
            case MovementRouteCommandCodes.SwitchOff:
                returnCommand = new MoveSwitchOffCommand();
                break;
            case MovementRouteCommandCodes.ChangeSpeed:
                returnCommand = new MoveChangeSpeedCommand();
                break;
            case MovementRouteCommandCodes.ChangeFrequency:
                returnCommand = new MoveChangeFrequencyCommand();
                break;
            case MovementRouteCommandCodes.WalkingAnimationOn:
                returnCommand = new MoveWalkingAnimationOnCommand();
                break;
            case MovementRouteCommandCodes.WalkingAnimationOff:
                returnCommand = new MoveWalkingAnimationOffCommand();
                break;
            case MovementRouteCommandCodes.SteppingAnimationOn:
                returnCommand = new MoveSteppingAnimationOnCommand();
                break;
            case MovementRouteCommandCodes.SteppingAnimationOff:
                returnCommand = new MoveSteppingAnimationOffCommand();
                break;
            case MovementRouteCommandCodes.DirectionFixOn:
                returnCommand = new MoveDirectionFixOnCommand();
                break;
            case MovementRouteCommandCodes.DirectionFixOff:
                returnCommand = new MoveDirectionFixOffCommand();
                break;
            case MovementRouteCommandCodes.ThroughOn:
                returnCommand = new MoveThroughOnCommand();
                break;
            case MovementRouteCommandCodes.ThroughOff:
                returnCommand = new MoveThroughOffCommand();
                break;
            case MovementRouteCommandCodes.TransparentOn:
                returnCommand = new MoveTransparentOnCommand();
                break;
            case MovementRouteCommandCodes.TransparentOff:
                returnCommand = new MoveTransparentOffCommand();
                break;
            case MovementRouteCommandCodes.ChangeImage:
                returnCommand = new MoveChangeImageCommand();
                break;
            case MovementRouteCommandCodes.ChangeOpacity:
                returnCommand = new MoveChangeOpacityCommand();
                break;
            case MovementRouteCommandCodes.ChangeBlendMode:
                returnCommand = new MoveChangeBlendModeCommand();
                break;
            case MovementRouteCommandCodes.PlaySe:
                returnCommand = new MovePlaySeCommand();
                break;
            case MovementRouteCommandCodes.Script:
                returnCommand = new MoveScriptCommand();
                break;
            default:
                throw new Exception($"Unknown movement command code '{command.Code}'");
        }

        returnCommand.Bind(command, version, loaded: true);

        return returnCommand;
    }
}
