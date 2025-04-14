using Godot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Multinet.LagCompensation;

public partial class NetworkVariable : GodotObject
{
    public Variant Value => GetValue();
    public Variant DefaultValue { get; private set; }
    public ReadOnlyDictionary<float, Variant> Buffer => buffer.AsReadOnly();
    protected NetworkBuffer buffer;

    protected float ServerTime => MultinetManager.Instance.ServerTime;
    protected float ServerTimerUpdateInterval => MultinetManager.Instance.Server.TimerUpdateInterval;

    protected float ClientTime => MultinetManager.Instance.ClientTime;
    protected float ClientTimerOffset => MultinetManager.Instance.Client.TimerOffset;

    public static NetworkVariable operator +(NetworkVariable networkVariable, Variant variant) => networkVariable.UpdateValue(variant);

    public NetworkVariable(Variant defaultValue)
    {
        DefaultValue = defaultValue;
        buffer = new(DefaultValue.VariantType);
    }

    public void Update(Variant value)
    {
        buffer.AddValue(ServerTime, value);
    }

    public void ClearBuffer()
    {
        buffer.Clear();
    }

    public T GetValue<[MustBeVariant] T>()
    {
        return GetValue().As<T>();
    }

    private Variant GetValue()
    {
        if (buffer.Count == 0)
        {
            return DefaultValue;
        }

        if (MultinetManager.Instance.IsServer)
        {
            return GetValueServer();
        }
        else
        {
            return GetValueClient();
        }
    }

    protected virtual Variant GetValueClient()
    {
        Variant result = default;

        // TODO: Client-Side Extrapolation
        // TODO: Client-Side Interpolation

        return result;
    }

    protected virtual Variant GetValueServer()
    {
        // TODO: Server-Side Compensation

        return buffer.Last().Value;
    }
}
