using Godot;
using Multinet.Client;
using Multinet.Server;
using System.Collections.Generic;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;

namespace Multinet;

/// <summary>
/// A centralized manager for handling multiplayer connections in a multiplayer game.
/// </summary>
public partial class MultinetManager : Node
{
    /// <summary>
    /// The singleton instance of the multiplayer manager.
    /// </summary>
    public static MultinetManager Instance { get; private set; }

    /// <summary>
    /// Signal emitted when the server starts successfully.
    /// </summary>
    /// <remarks>
    /// Executed on: Server
    /// </remarks>
    [Signal]
    public delegate void ServerStartedEventHandler();

    /// <summary>
    /// Signal emitted when the server is manually stopped.
    /// </summary>
    /// <remarks>
    /// Executed on: Server
    /// </remarks>
    [Signal]
    public delegate void ServerStoppedEventHandler();

    /// <summary>
    /// Signal emitted when the server fails to start.
    /// </summary>
    /// <param name="error"> The error code from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Server
    /// </remarks>
    [Signal]
    public delegate void StartServerFailedEventHandler(int error);

    /// <summary>
    /// Signal emitted when the client establishes a successful connection to the server.
    /// </summary>
    /// <param name="peerId"> The generated peer ID from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Client
    /// </remarks>
    [Signal]
    public delegate void JoinedServerEventHandler(int peerId);

    /// <summary>
    /// Signal emitted when the client leaves the server connection manually.
    /// </summary>
    /// <remarks>
    /// Executed on: Client
    /// </remarks>
    [Signal]
    public delegate void LeftServerEventHandler();

    /// <summary>
    /// Signal emitted when the client fails to establish a connection to the server.
    /// </summary>
    /// <param name="error"> The error code from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Client
    /// </remarks>
    [Signal]
    public delegate void JoinServerFailedEventHandler(int error);

    /// <summary>
    /// Signal emitted when the active connection to the server fails.
    /// </summary>
    /// <param name="error"> The error code from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Client
    /// </remarks>
    [Signal]
    public delegate void ConnectionToServerFailedEventHandler(int error);

    /// <summary>
    /// Signal emitted when a new client establishes a successful connection to the server.
    /// </summary>
    /// <param name="peerId"> The generated peer ID from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Server & All other client
    /// </remarks>
    [Signal]
    public delegate void ClientConnectedEventHandler(int peerId);

    /// <summary>
    /// Signal emitted when a client disconnects from the server.
    /// </summary>
    /// <param name="peerId"> The generated peer ID from the Godot multiplayer API.</param>
    /// <remarks>
    /// Executed on: Server & All other clients
    /// </remarks>
    [Signal]
    public delegate void ClientDisconnectedEventHandler(int peerId);

    /// <summary>
    /// Signal emitted when a player is added to the <seealso cref="SpawnedPlayers"/> collection.
    /// </summary>
    /// <param name="peerId">The peer ID from the Godot Multiplayer API.</param>
    /// <param name="playerNodePath">The node path to the player node relative to the root.</param>
    /// <remarks>
    /// Executed on: Server & All other clients
    /// </remarks>
    [Signal]
    public delegate void PlayerSpawnedEventHandler(int peerId, NodePath playerNodePath);

    /// <summary>
    /// Signal emitted when a player is removed from the <seealso cref="SpawnedPlayers"/> collection.
    /// </summary>
    /// <param name="peerId">The peer ID from the Godot Multiplayer API.</param>
    /// <param name="playerNodePath">The node path to the player node relative to the root.</param>
    /// <remarks>
    /// Executed on: Server & All other clients
    /// </remarks>
    [Signal]
    public delegate void PlayerDespawnedEventHandler(int peerId, NodePath playerNodePath);

    /// <summary>
    /// The peer ID from the Godot multiplayer API.
    /// </summary>
    public int PeerId { get; private set; } = 0;

    /// <summary>
    /// Indicates whether the current instance is a server.
    /// </summary>
    public bool IsServer => PeerId == 1;

    /// <summary>
    /// The time passed since game start in ms for the server.
    /// </summary>
    public float ServerTime { get; private set; } = 0f;

    /// <summary>
    /// The time passed since game start in ms for the client.
    /// </summary>
    public float ClientTime { get; private set; } = 0f;

    /// <summary>
    /// A list of currently connected peers.
    /// </summary>
    /// <remarks>
    /// This list is automatically managed by the <seealso cref="MultinetManager"/>
    /// </remarks>
    public IReadOnlyList<int> ConnectedPlayers => connectedPlayers.AsReadOnly();
    private List<int> connectedPlayers = new();

    /// <summary>
    /// The collection of spawned players.
    /// Element key is peer ID and value is the path to the player node relative to the root.
    /// </summary>
    /// <remarks>
    /// This collection is manually managed and should be updated when a player is spawned or removed. <br/><br/>
    /// Add a spawned player: 
    /// <code>
    /// MultinetManager.Instance.AddSpawnedPlayer(peerId, spawnedPlayerNode);
    /// </code>
    /// Remove a spawned player: 
    /// <code>
    /// MultinetManager.Instance.RemoveSpawnedPlayer(peerId);
    /// </code>
    /// </remarks>
    public IReadOnlyDictionary<int, NodePath> SpawnedPlayers => spawnedPlayers.AsReadOnly();
    private Dictionary<int, NodePath> spawnedPlayers = new();

    /// <summary>
    /// The server instance for the multiplayer game.
    /// </summary>
    public GameServer Server = new GameServer();

    /// <summary>
    /// The client instance for the multiplayer game.
    /// </summary>
    public GameClient Client = new GameClient();

    private float timeSinceLastUpdate = 0f;

    public override void _Ready()
    {
        Instance = this;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDiconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;

        Client.JoinServerFailed += OnServerJoinFailed;
        Server.StartServerFailed += OnServerStartFailed;
    }

    /// <summary>
    /// Runs every physics frame and updates the multiplayer timers.
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        if (IsServer)
        {
            var deltaInMs = (float)delta * 1000f;
            timeSinceLastUpdate += deltaInMs;
            ServerTime += deltaInMs;

            if (timeSinceLastUpdate >= Server.TimerUpdateInterval)
            {
                Rpc(nameof(UpdateServerTime), ServerTime);
                timeSinceLastUpdate = 0f;
            }
        }
        else
        {
            ClientTime += (float)delta * 1000f;

            if (ServerTime - ClientTime > 1000f)
                ClientTime = ServerTime;
        }
    }

    /// <summary>
    /// Sets the configuration for the Godot multiplayer server.
    /// </summary>
    public void SetServerConfiguration(ServerConfiguration configuration)
    {
        GD.Print($"Applying server configuration:");
        GD.Print(configuration);

        Server.SetConfiguration(configuration);
    }

    /// <summary>
    /// Sets the configuration for the Godot multiplayer client.
    /// </summary>
    public void SetClientConfiguration(ClientConfiguration configuration)
    {
        GD.Print("Applying client configuration:");
        GD.Print(configuration);

        Client.SetConfiguration(configuration);
    }

    /// <summary>
    /// Starts the Godot multiplayer server.
    /// </summary>
    public void StartServer()
    {
        if (Server.Start() == Error.Ok)
        {
            PeerId = 1;
            EmitSignal(SignalName.ServerStarted);
        }
    }

    /// <summary>
    /// Stops the Godot multiplayer server.
    /// </summary>
    public void StopServer()
    {
        Server.Stop();
        EmitSignal(SignalName.ServerStopped);
    }

    /// <summary>
    /// Starts the Godot multiplayer client and connects to the server.
    /// </summary>
    public int JoinServer()
    {
        if (IsServer)
        {
            GD.Print("Cannot join server as a server.");
            return 0;
        }

        PeerId = Client.Connect();
        return PeerId;
    }

    /// <summary>
    /// Disconnects the Godot multiplayer client from to the server.
    /// </summary>
    public void LeaveServer()
    {
        Client.Disconnect();
        PeerId = 0;
        EmitSignal(SignalName.LeftServer);
    }

    /// <summary>
    /// Adds a spawned player to the collection of spawned players.
    /// </summary>
    /// <param name="peerId">The peer id from the Godot Multiplayer API.</param>
    /// <param name="playerNode">The spawned player node.</param>
    public void AddSpawnedPlayer(int peerId, Node playerNode)
    {
        if (!IsServer)
        {
            return;
        }

        if (SpawnedPlayers.ContainsKey(peerId))
        {
            GD.PushWarning($"Player with Peer ID {peerId} is already spawned.");
            return;
        }

        spawnedPlayers.Add(peerId, playerNode.GetPath());

        Rpc(nameof(NotifyPlayerSpawned), peerId, SpawnedPlayers[peerId]);
    }

    /// <summary>
    /// Removes a spawned player to the collection of spawned players.
    /// </summary>
    /// <param name="peerId">The peer id from the Godot Multiplayer API.</param>
    public void RemoveSpawnedPlayer(int peerId)
    {
        if (!IsServer)
        {
            return;
        }

        if (!SpawnedPlayers.ContainsKey(peerId))
        {
            GD.PushWarning($"Player with Peer ID {peerId} does not exist in spawned players.");
            return;
        }

        Rpc(nameof(NotifyPlayerDespawned), peerId, SpawnedPlayers[peerId]);

        spawnedPlayers.Remove(peerId);
    }

    /// <summary>
    /// Notifies all clients that a player has spawned.
    /// </summary>
    [Rpc(RpcMode.Authority, CallLocal = true)]
    private void NotifyPlayerSpawned(int peerId, NodePath playerNodePath)
    {
        EmitSignal(SignalName.PlayerSpawned, peerId, playerNodePath);
    }

    /// <summary>
    /// Notifies all clients that a player has despawned.
    /// </summary>
    [Rpc(RpcMode.Authority, CallLocal = true)]
    private void NotifyPlayerDespawned(int peerId, NodePath playerNodePath)
    {
        EmitSignal(SignalName.PlayerDespawned, peerId, playerNodePath);
    }

    /// <summary>
    /// Broadcasts the server time to all clients.
    /// </summary>
    /// <param name="time">The time passed since game start in ms.</param>
    [Rpc(RpcMode.Authority, TransferMode = TransferModeEnum.Reliable)]
    private void UpdateServerTime(float time)
    {
        if (!IsServer)
            ServerTime = time;
    }

    private void OnConnectionFailed()
    {
        GD.Print("Connection to server failed.");
        EmitSignal(SignalName.ConnectionToServerFailed);
    }

    private void OnConnectedToServer()
    {
        var peerId = Multiplayer.MultiplayerPeer.GetUniqueId();
        GD.Print($"Joined server successfully with Peer ID = {peerId}");
        EmitSignal(SignalName.JoinedServer, peerId);
    }

    private void OnPeerDiconnected(long peerId)
    {
        if (peerId != 1)
        {
            connectedPlayers.Remove((int)peerId);

            GD.Print($"Peer with ID = {peerId} disconnected.");
            EmitSignal(SignalName.ClientDisconnected, peerId);
        }
        else
            GD.Print("Server disconnected.");
    }

    private void OnPeerConnected(long peerId)
    {
        if (peerId != 1)
        {
            connectedPlayers.Add((int)peerId);

            GD.Print($"Peer with ID = {peerId} connected.");
            EmitSignal(SignalName.ClientConnected, peerId);
        }
        else
            GD.Print("Server peer connected.");
    }

    private void OnServerStartFailed(int error)
    {
        EmitSignal(SignalName.StartServerFailed, error);
    }

    private void OnServerJoinFailed(int error)
    {
        EmitSignal(SignalName.JoinServerFailed, error);
    }
}
