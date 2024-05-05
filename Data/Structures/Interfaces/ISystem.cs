using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ISystem
{
    public string EncryptionKey { get; set; }

    public LookupList<string> Switches { get; set; }

    public LookupList<string> Variables { get; set; }
}
