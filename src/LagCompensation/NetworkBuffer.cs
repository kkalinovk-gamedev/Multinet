using Godot;
using Godot.Collections;
using static Godot.Variant;

namespace Multinet.LagCompensation
{
    public class NetworkBuffer : Dictionary<float, Variant>
    {
        public Type VariantType { get; private set; } = Type.Nil;

        public NetworkBuffer(Type valueType)
        {
            VariantType = valueType;
        }
    }
}
