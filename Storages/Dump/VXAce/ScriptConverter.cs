using System.Text.RegularExpressions;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RubyMarshalCS.SpecialTypes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.VXAce;

public class ScriptConverter : IYamlTypeConverter
{
    private readonly IYamlTypeConverter _inner;

    public bool Accepts(Type type)
    {
        return typeof(Script) == type;
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        var script = new Script();

        parser.Consume<MappingStart>();

        for (var i = 0; i < 3; ++i)
        {

            var field = parser.Consume<Scalar>();
            switch (field.Value)
            {
                case "Stamp":
                    script.Stamp = Convert.ToInt32(parser.Consume<Scalar>().Value);
                    break;
                case "Name":
                    script.Name = parser.Consume<Scalar>().Value;
                    break;
                case "Code":
                    script.Code = Regex.Replace(parser.Consume<Scalar>().Value, @"^#(.+)", "$1",
                        RegexOptions.Multiline);
                    break;
            }
        }

        parser.Consume<MappingEnd>();

        return script;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        var script = (Script)value;

        var ms = new MappingStart(null, "!Script", false, MappingStyle.Block);

        emitter.Emit(ms);

        emitter.Emit(new Scalar("Stamp"));
        emitter.Emit(new Scalar(script.Stamp.ToString()));
        emitter.Emit(new Scalar("Name"));
        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, script.Name, ScalarStyle.DoubleQuoted, true, true));
        emitter.Emit(new Scalar("Code"));

        var code = Regex.Replace(script.Code, @"^(.+)", "#$1", RegexOptions.Multiline);
        var scalar = new Scalar(AnchorName.Empty, TagName.Empty, code, ScalarStyle.Literal, true, false);
        emitter.Emit(scalar);

        emitter.Emit(new MappingEnd());
    }
}
