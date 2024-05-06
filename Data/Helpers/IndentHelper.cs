using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class IndentHelper
{
    public delegate void IndentWalker(int currIndent, int commandIndex, IEventCommand? prevCommand, IEventCommand currCommand);
    
    public static void ValidateIndents(ICommandContainer container, StorageVersion version)
    {
        WalkForIndents(container, version, (indent, index, prevCommand, currCommand) =>
        {
            if (indent != currCommand.Indent)
            {
                throw new Exception(
                    $"Wrong indent '{currCommand.Indent}' for command {currCommand.Code} at index {currCommand}, expected {indent}, prevCommand: {prevCommand?.Code}");
            }
        });
    }

    public static void SetupIndents(ICommandContainer container, StorageVersion version)
    {
        WalkForIndents(container, version, (indent, _, _, command) => { command.Indent = indent; });
    }

    private static void WalkForIndents(ICommandContainer container, StorageVersion version, IndentWalker walker)
    {
        int currIndent = 0;

        IEventCommand? prevCommand = null;
        
        foreach (var (i, c) in container.Commands.WithIndex())
        {
            switch (c.Code)
            {
                case CommandCodes.EndIf:
                    --currIndent;
                    break;
                case CommandCodes.Else:
                    --currIndent;
                    break;
                case CommandCodes.EndLoop:
                    --currIndent;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    --currIndent;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    --currIndent;
                    break;
                case CommandCodes.BattleProcessingEnd:
                    --currIndent;
                    break;
                case CommandCodes.ChoicesEnd:
                    --currIndent;
                    break;
            }

            walker(currIndent, i, prevCommand, c);

            prevCommand = c;

            switch (c.Code)
            {
                case CommandCodes.If:
                    ++currIndent;
                    break;
                case CommandCodes.Loop:
                    ++currIndent;
                    break;
                case CommandCodes.Else:
                    ++currIndent;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    ++currIndent;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    ++currIndent;
                    break;
                case CommandCodes.BattleProcessing:
                    ++currIndent;
                    break;
                case CommandCodes.ShowChoices:
                    ++currIndent;
                    break;
            }
        }
    }
}