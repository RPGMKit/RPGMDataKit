using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

using MoveCommandMV = MV.Classes.MoveCommand;
using MoveCommandVX = VXAce.Classes.MoveCommand;

public class MovementRouteCommand : NamedCommand
{
    public override int Code => CommandCodes.MovementRouteCommand;
    public override string GetCommandName(StorageVersion version) => "                  ";
        
    public MovementRouteCommand()
    {
        IsSecondary = true;
    }
        
    NamedCommand MoveCommand { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        var eventCommand = version == StorageVersion.MV
            ? parameterProvider.GetParameter<MoveCommandMV>(0)
            : parameterProvider.GetParameter<IMoveCommand>(0);
            
        MoveCommand = MovementCommandFactory.Create(eventCommand, version);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        IMoveCommand command = version == StorageVersion.MV ? new MoveCommandMV() : new MoveCommandVX();
        
        command.Code = MoveCommand.Code;
        MoveCommand.Pack(command.ParameterProvider, version);
        parameterProvider.SetParameter(0, command);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return MoveCommand.AsString(context, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, ExtendedFormat = options.ExtendedFormat});
    }
}