using Godot;

namespace Multinet.LagCompensation
{
    public static class NetworkVariableExtensions
    {
        public static NetworkVariable UpdateValue(this NetworkVariable networkVariable, Variant value)
        {
            networkVariable.Update(value);
            return networkVariable;
        }
    }
}
