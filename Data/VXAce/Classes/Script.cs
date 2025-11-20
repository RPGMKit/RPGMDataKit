using System.Text;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

public class Script : LookupList<object>
{
    private string? _code;

    public int Stamp
    {
        get => (int)this[0];
        set => Set(0, value);
    }

    public string Name
    {
        get => ((BinaryString)this[1]).Value;
        set
        {
            var str = Get(1) as BinaryString;
            if (str == null)
            {
                str = new BinaryString(value);
                Set(1, str);
            }
            else
                str.Value = value;
        }
    }

    public string Code
    {
        get
        {
            if (_code != null)
                return _code;

            var binaryString = (BinaryString)this[2];
            var enc = binaryString.Encoding ?? Encoding.UTF8;
            
            _code = enc.GetString(RubyDeflate.Inflate(binaryString.Bytes));

            return _code;
        }
        set
        {
            _code = value;

            var binaryString = (BinaryString)this[2];
            var enc = binaryString?.Encoding ?? Encoding.UTF8;

            var bytes = RubyDeflate.Deflate(enc.GetBytes(_code));
            binaryString = new BinaryString(bytes, enc);

            Set(2, binaryString);
        }
    }
}
