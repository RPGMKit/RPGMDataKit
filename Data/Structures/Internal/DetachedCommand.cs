using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>Storage of a <see cref="NamedCommand"/> that was created in code and is not in a container yet.</summary>
internal sealed class DetachedCommand : ICommand
{
    public int Code { get; set; }

    public int? Indent { get; set; }

    public List<object?>? Parameters { get; set; }

    public IParameterProvider ParameterProvider => new SimpleParameterProvider(this);
}
