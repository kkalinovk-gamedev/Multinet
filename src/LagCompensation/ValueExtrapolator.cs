using Godot;
using Godot.Collections;
using System;

namespace Multinet.LagCompensation;

public static class ValueExtrapolator<T>
{
    public static T Extrapolate(float renderTime, Array<NetworkVariableState<T>> buffer)
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(float):
                return (T)(object)(buffer as Array<NetworkVariableState<float>>).Extrapolate(renderTime);
            case Type t when t == typeof(Vector2):
                return (T)(object)(buffer as Array<NetworkVariableState<Vector2>>).Extrapolate(renderTime);
            case Type t when t == typeof(Vector3):
                return (T)(object)(buffer as Array<NetworkVariableState<Vector3>>).Extrapolate(renderTime);
            default:
                throw new NotSupportedException($"Interpolation for type {typeof(T)} is not supported.");
        }
    }
}
