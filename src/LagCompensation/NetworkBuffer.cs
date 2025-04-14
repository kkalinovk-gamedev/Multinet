using Godot;
using Godot.Collections;
using System.Linq;
using Type = Godot.Variant.Type;

namespace Multinet.LagCompensation
{
    public class NetworkBuffer : Dictionary<float, Variant>
    {
        public Type VariantType { get; private set; } = Type.Nil;

        public NetworkBuffer(Type valueType)
        {
            VariantType = valueType;
        }

        public void AddValue(float serverTime, Variant value)
        {
            if (ContainsKey(serverTime))
                this[serverTime] = value;
            else
                Add(serverTime, value);
        }

        public void RemoveValueByTime(float time)
        {
            if (ContainsKey(time))
                Remove(time);
        }

        public void RemoveValueAt(int position)
        {
            if (position < 0 || position >= Count)
                return;

            var elementToRemove = this.ElementAt(position);
            Remove(elementToRemove.Key);
        }
    }
}
