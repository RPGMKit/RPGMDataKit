using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class IndentHelper
{
    private delegate void IndentWalker(int currIndent, int commandIndex, ICommand? prevCommand, ICommand currCommand);
    
    public static void ValidateIndents(ICommandContainer<ICommand> container, StorageVersion version)
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

    public static void SetupIndents(ICommandContainer<ICommand> container, StorageVersion version)
    {
        WalkForIndents(container, version, (indent, _, _, command) => { command.Indent = indent; });
    }

    private static void WalkForIndents(ICommandContainer<ICommand> container, StorageVersion version, IndentWalker walker)
    {
        ICommand? prevCommand = null;

        int i = 0, indentLevel = 0;
        foreach (var c in container.Commands)
        {
            switch (c.Code)
            {
                case CommandCodes.EndIf:
                    --indentLevel;
                    break;
                case CommandCodes.Else:
                    --indentLevel;
                    break;
                case CommandCodes.EndLoop:
                    --indentLevel;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    --indentLevel;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    --indentLevel;
                    break;
                case CommandCodes.BattleProcessingEnd:
                    --indentLevel;
                    break;
                case CommandCodes.ChoicesEnd:
                    --indentLevel;
                    break;
            }

            walker(indentLevel, i, prevCommand, c);

            prevCommand = c;

            switch (c.Code)
            {
                case CommandCodes.If:
                    ++indentLevel;
                    break;
                case CommandCodes.Loop:
                    ++indentLevel;
                    break;
                case CommandCodes.Else:
                    ++indentLevel;
                    break;
                case CommandCodes.ChoiceCase:
                case CommandCodes.ChoiceCaseCancel:
                    ++indentLevel;
                    break;
                case CommandCodes.BattleProcessingIfLose:
                case CommandCodes.BattleProcessingIfEscape:
                case CommandCodes.BattleProcessingIfWin:
                    ++indentLevel;
                    break;
                case CommandCodes.BattleProcessing:
                    var nextCommand = container.Commands.ElementAtOrDefault(i + 1);
                    if (nextCommand != null && (nextCommand.Code == CommandCodes.BattleProcessingIfLose
                                                || nextCommand.Code == CommandCodes.BattleProcessingIfEscape
                                                || nextCommand.Code == CommandCodes.BattleProcessingIfWin))
                    {
                        ++indentLevel;
                    }

                    break;
                case CommandCodes.ShowChoices:
                    ++indentLevel;
                    break;
            }

            ++i;
        }
    }
}