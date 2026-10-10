using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.VXAce;

/// <summary>Base of VXAce entities with a list of movement commands.</summary>
public abstract class VxMoveCommandContainer : RubyCommandContainer<IMoveCommand>
{
}
