using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

public static class IndentHelper
{
    private delegate void IndentWalker(int currIndent, int commandIndex, NamedCommand? prevCommand, NamedCommand currCommand);
    
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
        NamedCommand? prevCommand = null;

        var commands = container.NamedCommands.ToList();

        int i = 0, indentLevel = 0;
        foreach (var c in commands)
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
                    var nextCommand = commands.ElementAtOrDefault(i + 1);
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