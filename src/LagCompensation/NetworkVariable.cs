using Godot;
using Godot.Collections;

namespace Multinet.LagCompensation;

public partial class NetworkVariable<T> : GodotObject
{
    public int InterpolationOffset { get; set; } = 30;

    public T Value => GetValue();

    protected float ServerTime => MultinetManager.Instance.ServerTime;
    protected float ClientTime => MultinetManager.Instance.ClientTime;
    protected Array<NetworkVariableState<T>> Buffer => buffer;
    protected T DefaultValue;

    private Array<NetworkVariableState<T>> buffer = new();

    public NetworkVariable(T defaultValue = default)
    {
        DefaultValue = defaultValue;
    }

    public void Update(T value)
    {
        var state = new NetworkVariableState<T>(ServerTime, value);
        buffer.Add(state);
    }

    public void ClearBuffer()
    {
        buffer.Clear();
    }

    private T GetValue()
    {
        T result = default;

        if (buffer.Count == 0 || ServerTime > buffer[buffer.Count - 1].Timestamp)
        {
            buffer.Add(new NetworkVariableState<T>(ServerTime, DefaultValue));
        }

        float renderTime = ClientTime - InterpolationOffset;

        if (buffer.Count > 2)
        {
            while (buffer.Count > 2 && renderTime > buffer[1].Timestamp)
            {
                buffer.RemoveAt(0);
            }

            result = ValueInterpolator<T>.Interpolate(renderTime, buffer);
        }
        else if (buffer.Count == 2 && renderTime > buffer[1].Timestamp)
        {
            result = ValueExtrapolator<T>.Extrapolate(renderTime, buffer);
        }

        return result;
    }
}
