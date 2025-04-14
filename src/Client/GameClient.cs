using Godot;

namespace Multinet.Client;

/// <summary>
/// Client implementation for the multiplayer game.
/// </summary>
public partial class GameClient : GodotObject
{
    /// <summary>
    /// Signal emitted when the client fails to establish a connection to the server.
    /// </summary>
    /// <param name="error">The error code from Godot Multiplayer API.</param>
    [Signal]
    public delegate void JoinServerFailedEventHandler(int error);

    /// <summary>
    /// The time in ms that all clients are offset from the server.
    /// </summary>
    public float TimerOffset => configuration.ClientTimerOffset;

    private ClientConfiguration configuration = new();

    /// <summary>
    /// Sets the client configuration for the multiplayer game.
    /// </summary>
    public void SetConfiguration(ClientConfiguration newConfiguration)
    {
        configuration = newConfiguration;
    }

    /// <summary>
    /// Connects to the server.
    /// </summary>
    /// <returns>The generated peer ID from Godot Multipayer API.</returns>
    public int Connect()
    {
        var peer = new ENetMultiplayerPeer();

        GD.Print("Connecting to sever...");

        var error = peer.CreateClient(configuration.ServerAddress, configuration.ServerPort);

        if (error != Error.Ok)
        {
            GD.Print($"Connecting to server failed: {error}");
            EmitSignal(SignalName.JoinServerFailed, (int)error);
            return 0;
        }

        peer.Host.Compress(configuration.ConnectionCompression);
        (Engine.GetMainLoop() as SceneTree).Root.Multiplayer.MultiplayerPeer = peer;

        return peer.GetUniqueId();
    }

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    public void Disconnect()
    {
        (Engine.GetMainLoop() as SceneTree).Root.Multiplayer.MultiplayerPeer?.Close();
    }
}
