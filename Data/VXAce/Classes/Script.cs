using System.Text;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes;

public class Script : LookupList<object>
{
    private string _code;
    private string _name;

    public int Stamp
    {
        get => (int)this[0];
        set => Set(0, value);
    }

    public string Name
    {
        get
        {
            if (_name != null)
                return _name;

            _name = ((BinaryString)this[1]).Value;

            return _name;
        }
        set
        {
            _name = value;

            Set(1, new BinaryString(value));
        }
    }

    public string Code
    {
        get
        {
            if (_code != null)
                return _code;

            _code = Encoding.UTF8.GetString(RubyDeflate.Inflate(((BinaryString)this[2]).Bytes));

            return _code;
        }
        set
        {
            _code = value;

            var str = new BinaryString(RubyDeflate.Deflate(Encoding.UTF8.GetBytes(_code)), Encoding.UTF8);

            Set(2, str);
        }
    }
}
