using Godot;
using System.Linq;

namespace Multinet.LagCompensation;

public partial class NetworkVariable : GodotObject
{
    public int InterpolationOffset { get; set; } = 30;

    public Variant Value => GetValue();

    protected float ServerTime => MultinetManager.Instance.ServerTime;
    protected float ClientTime => MultinetManager.Instance.ClientTime;
    protected NetworkBuffer Buffer => buffer;
    protected Variant DefaultValue;

    private NetworkBuffer buffer;

    public NetworkVariable(Variant defaultValue)
    {
        DefaultValue = defaultValue;
        buffer = new(DefaultValue.VariantType);
    }

    public void Update(Variant value)
    {
        AddValue(ServerTime, value);
    }

    public void ClearBuffer()
    {
        buffer.Clear();
    }

    private Variant GetValue()
    {
        if (buffer.Count == 0)
        {
            return DefaultValue;
        }

        var result = buffer.Last().Value;
        buffer.Clear();
        AddValue(ServerTime, result);
        return result;

        //if (MultinetManager.Instance.IsServer)
        //{
        //    if (buffer.Count == 0)
        //    {
        //        return DefaultValue;
        //    }

        //    return buffer.Last().Value;
        //}

        //Variant result = default;

        //if (buffer.Count == 0 || ServerTime > buffer.ElementAt(buffer.Count - 1).Key)
        //{
        //    if (buffer.Count == 0)
        //        AddValue(ServerTime, DefaultValue);
        //    else
        //        AddValue(ServerTime, buffer.Last().Value);
        //}

        //float renderTime = ClientTime - InterpolationOffset;

        //if (buffer.Count > 2)
        //{
        //    while (buffer.Count > 2 && renderTime > buffer.ElementAt(1).Key)
        //    {
        //        buffer.Remove(buffer.First().Key);
        //    }

        //    result = buffer.Interpolate(renderTime);
        //}
        //else if (buffer.Count == 2 && renderTime > buffer.ElementAt(1).Key)
        //{
        //    result = buffer.Extrapolate(renderTime);
        //}

        //return result;
    }

    private void AddValue(float serverTime, Variant value)
    {
        if (buffer.ContainsKey(serverTime))
            buffer[serverTime] = value;
        else
            buffer.Add(serverTime, value);
    }
}
