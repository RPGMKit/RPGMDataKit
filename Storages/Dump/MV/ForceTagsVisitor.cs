using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace RPGMakerLibCoreV2.Storages.Dump.MV;

public class ForceTagsVisitor : ChainedObjectGraphVisitor
{
    public ForceTagsVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor) { }
    
    public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context, ObjectSerializer serializer)
    {
        if (key == null && value.Value != null) // Check if it's the root object
        {
            context.Emit(new DocumentStart()); // Ensure document starts if necessary.
            context.Emit(new MappingStart(null, TagFromType(value.Type), false, MappingStyle.Block));
            return base.EnterMapping(key, value, context, serializer);
        }
        else if (value.Value != null && !value.Type.IsValueType && value.Type != typeof(string))
        {
            //  Skip primitive types and strings
            context.Emit(new MappingStart(null, TagFromType(value.Type), false, MappingStyle.Block));
            return base.EnterMapping(key, value, context, serializer);

        }

        return base.EnterMapping(key, value, context, serializer);
    }
    
    private static TagName TagFromType(Type type)
    {
        return new TagName("!" + type.FullName + ", " + type.Assembly.GetName().Name);
    }
}