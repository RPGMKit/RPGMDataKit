using RPGMDataKit.Data.VXAce.Classes;

namespace RPGMDataKit.Storages.Dump.TextSerializers;

public class ScriptSerializer : IToTextSerializer
{
    public string Serialize<T>(T obj)
    {
        var script = obj as Script;

        return string.Join("\r\n", [
            script.Stamp,
            script.Name,
            script.Code
        ]);
    }

    public T Deserialize<T>(string text)
    {
        Script script = new();

        using var sr = new StringReader(text);
        script.Stamp = Convert.ToInt32(sr.ReadLine());
        script.Name = sr.ReadLine();

        List<string> scriptLines = new();
        for (;;)
        {
            var line = sr.ReadLine();
            if (line == null)
                break;

            scriptLines.Add(line);
        }
        script.Code = string.Join("\r\n", scriptLines);

        return (T)(object)script;
    }
}