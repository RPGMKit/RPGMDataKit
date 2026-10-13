using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.VXAce.Classes;

namespace RPGMDataKit.Data.VXAce;

/// <summary>Base of VXAce entities with a list of movement commands.</summary>
public abstract class VxMoveCommandContainer : RubyCommandContainer<IMoveCommand>
{
}
