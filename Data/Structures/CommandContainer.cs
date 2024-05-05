using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures;

public class CommandContainer : LookupList<EventCommand>
{
    public IEnumerable<NamedCommand> Commands => this.Select(CommandFactory.Factory);

    private delegate void IndentWalker(int currIndent, int commandIndex, EventCommand command);
    
    private void WalkForIndents(IndentWalker walker)
    {
        int currIndent = 0;
        
        foreach (var (i, c) in this.WithIndex())
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

            walker(currIndent, i, c);

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

    public void ValidateIndents()
    {
        WalkForIndents((indent, index, command) =>
        {
            if (indent != command.Indent)
            {
                var prevC = this[index - 1];
                throw new Exception(
                    $"Wrong indent '{command.Indent}' for command {command.Code} at index {command}, expected {indent}, prevCommand: {prevC.Code}");
            }
        });
    }

    public void SetupIndents()
    {
        WalkForIndents((indent, index, command) => { command.Indent = indent; });
    }
}
