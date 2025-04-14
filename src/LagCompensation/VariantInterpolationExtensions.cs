using Godot;
using System;
using System.Linq;

namespace Multinet.LagCompensation;

public static class VariantInterpolationExtensions
{
    public static Variant Interpolate(this NetworkBuffer buffer, float renderTime)
    {
        float interpolationFactor = (renderTime - buffer.ElementAt(0).Key) / (buffer.ElementAt(1).Key - buffer.ElementAt(0).Key);

        switch (buffer.VariantType)
        {
            case Variant.Type.Float:
                return Mathf.Lerp(buffer.ElementAt(0).Value.AsSingle(), buffer.ElementAt(1).Value.AsSingle(), interpolationFactor);
            case Variant.Type.Vector2:
                return buffer.ElementAt(0).Value.AsVector2().Lerp(buffer.ElementAt(1).Value.AsVector2(), interpolationFactor);
            case Variant.Type.Vector3:
                return buffer.ElementAt(0).Value.AsVector3().Lerp(buffer.ElementAt(1).Value.AsVector3(), interpolationFactor);
            default:
                throw new NotSupportedException($"Interpolation for type {buffer.VariantType} is not supported.");
        }
    }
}
