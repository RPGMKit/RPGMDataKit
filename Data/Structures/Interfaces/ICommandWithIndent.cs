namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommandWithIndent : ICommand
{
    public int? Indent { get; set; }
}
