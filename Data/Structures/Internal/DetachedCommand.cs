using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>Storage of a <see cref="NamedCommand"/> that was created in code and is not in a container yet.</summary>
internal sealed class DetachedCommand : ICommand
{
    public int Code { get; set; }

    public int? Indent { get; set; }

    public List<object?>? Parameters { get; set; }

    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);
}
