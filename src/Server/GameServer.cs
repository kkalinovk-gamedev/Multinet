using Godot;

namespace Multinet.Server;

/// <summary>
/// Server implementation for the multiplayer game.
/// </summary>
public partial class GameServer : GodotObject
{
    /// <summary>
    /// Signal emitted when the server fails to start.
    /// </summary>
    /// <param name="error">The error code from Godot Multiplayer API.</param>
    [Signal]
    public delegate void StartServerFailedEventHandler(int error);

    private ServerConfiguration configuration = new();

    /// <summary>
    /// The interval in ms for syncing the server time in ms.
    /// </summary>
    public float TimeUpdateInterval => configuration.ServerTimeUpdateInterval;

    /// <summary>
    /// Sets the server configuration for the multiplayer game.
    /// </summary>
    public void SetConfiguration(ServerConfiguration newConfiguration)
    {
        configuration = newConfiguration;
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public Error Start()
    {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();

        GD.Print($"Starting server...");
        var error = peer.CreateServer(configuration.ServerPort);

        if (error != Error.Ok)
        {
            GD.Print($"Starting server failed with code {error}.");
            EmitSignal(SignalName.StartServerFailed, (int)error);
            return error;
        }

        peer.Host.Compress(configuration.ConnectionCompression);

        (Engine.GetMainLoop() as SceneTree).Root.Multiplayer.MultiplayerPeer = peer;

        GD.Print("Server started.");
        return Error.Ok;
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        (Engine.GetMainLoop() as SceneTree).Root.Multiplayer.MultiplayerPeer?.Close();
    }
}
