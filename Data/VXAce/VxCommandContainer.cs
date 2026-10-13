using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.VXAce.Classes;

namespace RPGMDataKit.Data.VXAce;

/// <summary>Base of VXAce entities with a list of event commands.</summary>
public abstract class VxCommandContainer : RubyCommandContainer<ICommand>
{
}
