using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV;

/// <summary>Base of MV entities with a list of event commands.</summary>
public abstract class MvCommandContainer : JsonCommandContainer<ICommand>
{
}
