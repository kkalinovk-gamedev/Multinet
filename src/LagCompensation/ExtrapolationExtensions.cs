using Godot;
using Godot.Collections;

namespace Multinet.LagCompensation;

public static class ExtrapolationExtensions
{
    public static float Extrapolate(this Array<NetworkVariableState<float>> buffer, float renderTime)
    {
        float extrapolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        var delta = buffer[1].Value - buffer[0].Value;
        var newValue = buffer[1].Value + delta * extrapolationFactor;
        return Mathf.Lerp(buffer[1].Value, newValue, extrapolationFactor);
    }

    public static Vector2 Extrapolate(this Array<NetworkVariableState<Vector2>> buffer, float renderTime)
    {
        float extrapolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        var delta = buffer[1].Value - buffer[0].Value;
        var newValue = buffer[1].Value + delta * extrapolationFactor;
        return buffer[1].Value.Lerp(newValue, extrapolationFactor);
    }

    public static Vector3 Extrapolate(this Array<NetworkVariableState<Vector3>> buffer, float renderTime)
    {
        float extrapolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        var delta = buffer[1].Value - buffer[0].Value;
        var newValue = buffer[1].Value + delta * extrapolationFactor;
        return buffer[1].Value.Lerp(newValue, extrapolationFactor);
    }
}
