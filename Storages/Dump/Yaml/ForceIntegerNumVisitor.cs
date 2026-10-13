using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace RPGMDataKit.Storages.Dump.Yaml;

public class ForceIntegerEnumVisitor : ChainedObjectGraphVisitor
{
    public ForceIntegerEnumVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor) { }
    
    public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context, ObjectSerializer serializer)
    {
        if (value.Type.IsEnum)
        {
            context.Emit(new Scalar(null, null, Convert.ToString(Convert.ToInt32(value.Value)), ScalarStyle.Plain, true,
                false));
            return false;
        }

        return base.EnterMapping(key, value, context, serializer);
    }

    public override bool Enter(IPropertyDescriptor? propertyDescriptor, IObjectDescriptor value, IEmitter context,
        ObjectSerializer serializer)
    {
        if (value.Type.IsEnum)
        {
            context.Emit(new Scalar(null, null, Convert.ToString(Convert.ToInt32(value.Value)), ScalarStyle.Plain, true,
                false));
            return false;
        }
        
        return base.Enter(propertyDescriptor, value, context, serializer);
    }

    public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context, ObjectSerializer serializer)
    {
        if (value.Type.IsEnum)
        {
            context.Emit(new Scalar(null, null, Convert.ToString(Convert.ToInt32(value.Value)), ScalarStyle.Plain, true,
                false));
            return false;
        }
        
        return base.EnterMapping(key, value, context, serializer);
    }
}