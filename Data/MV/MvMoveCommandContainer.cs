using RPGMDataKit.Data.MV.Classes;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV;

/// <summary>Base of MV entities with a list of movement commands.</summary>
public abstract class MvMoveCommandContainer : JsonCommandContainer<IMoveCommand>
{
}
