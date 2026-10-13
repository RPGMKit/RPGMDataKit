using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::AudioFile")]
public class AudioFile : RubySerializationEntity, IAudioFile
{
    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@volume")]
    public int Volume { get; set; }

    [RubyProperty("@pitch")]
    public int Pitch { get; set; }

    public int Pan
    {
        get => throw new DoesNotExistInVersionException();
        set => throw new DoesNotExistInVersionException();
    }

    [RubyProperty("@features", (CandidateFlags.In | CandidateFlags.Dynamic))]
    public object _1 { get; set; }
}
