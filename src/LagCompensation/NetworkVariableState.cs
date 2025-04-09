using Godot;

namespace Multinet.LagCompensation;

public partial class NetworkVariableState<T> : GodotObject
{
    public NetworkVariableState(float timestamp, T value)
    {
        Timestamp = timestamp;
        Value = value;
    }

    public float Timestamp { get; private set; }
    public T Value { get; private set; }
}
