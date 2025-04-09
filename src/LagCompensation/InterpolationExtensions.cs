using Godot;
using Godot.Collections;

namespace Multinet.LagCompensation;

public static class InterpolationExtensions
{
    public static float Interpolate(this Array<NetworkVariableState<float>> buffer, float renderTime)
    {
        float interpolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        return Mathf.Lerp(buffer[0].Value, buffer[1].Value, interpolationFactor);
    }

    public static Vector2 Interpolate(this Array<NetworkVariableState<Vector2>> buffer, float renderTime)
    {
        float interpolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        return buffer[0].Value.Lerp(buffer[1].Value, interpolationFactor);
    }

    public static Vector3 Interpolate(this Array<NetworkVariableState<Vector3>> buffer, float renderTime)
    {
        float interpolationFactor = (renderTime - buffer[0].Timestamp) / (buffer[1].Timestamp - buffer[0].Timestamp);

        return buffer[0].Value.Lerp(buffer[1].Value, interpolationFactor);
    }
}
