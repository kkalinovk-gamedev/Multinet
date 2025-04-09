#if TOOLS
using Godot;
using Multinet;
using Multinet.Client;
using Multinet.Server;

[Tool]
public partial class MultinetPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        var serverConfigurationIcon = GD.Load<Texture2D>("res://addons/multinet/icons/MultiplayerNode.svg");
        var serverConfigurationScript = GD.Load<Script>("res://addons/multinet/src/Server/ServerConfiguration.cs");
        AddCustomType(nameof(ServerConfiguration), nameof(Resource), serverConfigurationScript, serverConfigurationIcon);

        var clientConfigurationIcon = GD.Load<Texture2D>("res://addons/multinet/icons/MultiplayerNode.svg");
        var clientConfigurationScript = GD.Load<Script>("res://addons/multinet/src/Client/ClientConfiguration.cs");
        AddCustomType(nameof(ClientConfiguration), nameof(Resource), clientConfigurationScript, clientConfigurationIcon);

        var multiplayerNodeIcon = GD.Load<Texture2D>("res://addons/multinet/icons/MultiplayerNode.svg");
        var multiplayerNodeScript = GD.Load<Script>("res://addons/multinet/src/MultiplayerNode.cs");
        AddCustomType(nameof(MultiplayerNode), nameof(Node), multiplayerNodeScript, multiplayerNodeIcon);

        AddAutoloadSingleton(nameof(MultinetManager), "res://addons/multinet/src/MultinetManager.cs");
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(nameof(MultinetManager));

        RemoveCustomType(nameof(MultiplayerNode));
        RemoveCustomType(nameof(ServerConfiguration));
        RemoveCustomType(nameof(ClientConfiguration));
    }
}
#endif
