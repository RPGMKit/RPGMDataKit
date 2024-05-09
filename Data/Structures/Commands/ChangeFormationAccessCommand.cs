using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeFormationAccessCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeFormationAccess;
    public override string GetCommandName(StorageVersion version) => "Change Formation Access";
}