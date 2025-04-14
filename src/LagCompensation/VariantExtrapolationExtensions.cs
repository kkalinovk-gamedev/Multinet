using Godot;
using System;
using System.Linq;

namespace Multinet.LagCompensation;

public static class VariantExtrapolationExtensions
{
    public static Variant Extrapolate(this NetworkBuffer buffer, float renderTime)
    {
        float extrapolationFactor = (renderTime - buffer.ElementAt(0).Key) / (buffer.ElementAt(1).Key - buffer.ElementAt(0).Key);

        switch (buffer.VariantType)
        {
            case Variant.Type.Float:
                return buffer.ExtrapolateFloat(extrapolationFactor);
            case Variant.Type.Vector2:
                return buffer.ExtrapolateVector2(extrapolationFactor);
            case Variant.Type.Vector3:
                return buffer.ExtrapolateVector3(extrapolationFactor);
            default:
                throw new NotSupportedException($"Interpolation for type {buffer.VariantType} is not supported.");
        }
    }

    private static float ExtrapolateFloat(this NetworkBuffer buffer, float extrapolationFactor)
    {
        var targetValue = buffer.ElementAt(1).Value.AsSingle();
        var previousValue = buffer.ElementAt(0).Value.AsSingle();

        var valueDelta = targetValue - previousValue;

        return Mathf.Lerp(targetValue, targetValue + valueDelta, extrapolationFactor);
    }

    private static Vector2 ExtrapolateVector2(this NetworkBuffer buffer, float extrapolationFactor)
    {
        var targetValue = buffer.ElementAt(1).Value.AsVector2();
        var previousValue = buffer.ElementAt(0).Value.AsVector2();

        var valueDelta = targetValue - previousValue;

        return targetValue.Lerp(targetValue + valueDelta, extrapolationFactor);
    }

    private static Vector3 ExtrapolateVector3(this NetworkBuffer buffer, float extrapolationFactor)
    {
        var targetValue = buffer.ElementAt(1).Value.AsVector3();
        var previousValue = buffer.ElementAt(0).Value.AsVector3();

        var valueDelta = targetValue - previousValue;

        return targetValue.Lerp(targetValue + valueDelta, extrapolationFactor);
    }
}
