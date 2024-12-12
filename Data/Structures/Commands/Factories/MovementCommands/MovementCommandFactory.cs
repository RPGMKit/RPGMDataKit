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
            case MovementRouteCommandCodes.Empty:
                returnCommand = new MoveEmptyCommand();
                break;
            case MovementRouteCommandCodes.Down:
                returnCommand = new MoveDownCommand();
                break;
            case MovementRouteCommandCodes.Left:
                returnCommand = new MoveLeftCommand();
                break;
            case MovementRouteCommandCodes.Right:
                returnCommand = new MoveRightCommand();
                break;
            case MovementRouteCommandCodes.Up:
                returnCommand = new MoveUpCommand();
                break;
            case MovementRouteCommandCodes.LowerLeft:
                returnCommand = new MoveLowerLeftCommand();
                break;
            case MovementRouteCommandCodes.LowerRight:
                returnCommand = new MoveLowerRightCommand();
                break;
            case MovementRouteCommandCodes.UpperLeft:
                returnCommand = new MoveUpperLeftCommand();
                break;
            case MovementRouteCommandCodes.UpperRight:
                returnCommand = new MoveUpperRightCommand();
                break;
            case MovementRouteCommandCodes.AtRandom:
                returnCommand = new MoveAtRandomCommand();
                break;
            case MovementRouteCommandCodes.TowardPlayer:
                returnCommand = new MoveTowardPlayerCommand();
                break;
            case MovementRouteCommandCodes.AwayFromPlayer:
                returnCommand = new MoveAwayFromPlayerCommand();
                break;
            case MovementRouteCommandCodes.OneStepForward:
                returnCommand = new MoveOneStepForwardCommand();
                break;
            case MovementRouteCommandCodes.OneStepBackward:
                returnCommand = new MoveOneStepBackwardCommand();
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
            case MovementRouteCommandCodes.Speed:
                returnCommand = new MoveSpeedCommand();
                break;
            case MovementRouteCommandCodes.Frequency:
                returnCommand = new MoveFrequencyCommand();
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
            case MovementRouteCommandCodes.Image:
                returnCommand = new MoveImageCommand();
                break;
            case MovementRouteCommandCodes.Opacity:
                returnCommand = new MoveOpacityCommand();
                break;
            case MovementRouteCommandCodes.BlendMode:
                returnCommand = new MoveBlendModeCommand();
                break;
            case MovementRouteCommandCodes.Se:
                returnCommand = new MoveSeCommand();
                break;
            case MovementRouteCommandCodes.Script:
                returnCommand = new MoveScriptCommand();
                break;
            default:
                throw new Exception($"Unknown movement command code '{command.Code}'");
        }

        returnCommand.Unpack(command.ParameterProvider, version);
        returnCommand.Indent = null;

        return returnCommand;
    }
}
