using System.Text;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

public class ScriptSerializer : IToTextSerializer
{
    public string Serialize<T>(T obj)
    {
        var script = obj as Script;

        return string.Join("\r\n", [
            script.Stamp,
            ((BinaryString)script[1]).Encoding?.CodePage ?? 0,
            script.Name,
            ((BinaryString)script[2]).Encoding?.CodePage ?? 0,
            script.Code
        ]);
    }

    public T Deserialize<T>(string text)
    {
        Script script = new();

        using var sr = new StringReader(text);
        script.Stamp = Convert.ToInt32(sr.ReadLine());
        var enc = Convert.ToInt32(sr.ReadLine());
        var name = sr.ReadLine();

        script.Set(1, new BinaryString(name, enc > 0 ? Encoding.GetEncoding(enc) : null));

        enc = Convert.ToInt32(sr.ReadLine());
        List<string> scriptLines = new();
        for (;;)
        {
            var line = sr.ReadLine();
            if (line == null)
                break;

            scriptLines.Add(line);
        }

        script.Set(2, new BinaryString("", enc > 0 ? Encoding.GetEncoding(enc) : null));
        script.Code = string.Join("\r\n", scriptLines);

        return (T)(object)script;
    }
}