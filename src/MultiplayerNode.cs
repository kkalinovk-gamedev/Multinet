using Godot;
using Multinet.Client;
using Multinet.Server;
using System.Collections.Generic;
using System.Linq;

namespace Multinet;

/// <summary>
/// A node that manages: </br>
/// - Configurations for both server and client
/// - Auto-starting the server if running in dedicated server mode
/// </summary>
public partial class MultiplayerNode : Node
{
    [Export]
    private bool UseCmdArgs = false;

    [Export]
    private ServerConfiguration ServerConfiguration = new ServerConfiguration()
    {
        ResourceName = "Configuration",
    };

    [Export]
    private ClientConfiguration ClientConfiguration = new()
    {
        ResourceName = "Configuration",
    };

    /// <summary>
    /// <b>WARNING:</b> Subsequent overriden methods should always call the base method first. </br>
    /// Otherwise automatic server & client configurations, as well as server auto-start will not work.
    /// </summary>
    public override void _Ready()
    {
        if (UseCmdArgs)
        {
            Dictionary<string, string> arguments = GetCmdLineArgs();
            var serverPort = GetServerPortArgument(arguments);

            ServerConfiguration.ServerPort = serverPort ?? ServerConfiguration.ServerPort;
            ClientConfiguration.ServerPort = serverPort ?? ClientConfiguration.ServerPort;
        }

        if (OS.HasFeature("dedicated_server"))
        {
            MultinetManager.Instance.SetServerConfiguration(ServerConfiguration);
            MultinetManager.Instance.StartServer();
        }
        else
        {
            MultinetManager.Instance.SetClientConfiguration(ClientConfiguration);
        }
    }

    private static int? GetServerPortArgument(Dictionary<string, string> arguments)
    {
        int? result = null;

        if (arguments.ContainsKey("port"))
        {
            string portValue = arguments["port"];
            if (int.TryParse(portValue, out int port))
            {
                result = port;
            }
            else
            {
                GD.Print($"Server port value of '{portValue}' is not a valid integer.");
            }
        }

        return result;
    }

    private static Dictionary<string, string> GetCmdLineArgs()
    {
        var arguments = new Dictionary<string, string>();

        foreach (var argument in OS.GetCmdlineArgs())
        {
            if (argument.Find("=") > -1)
            {
                string[] keyValue = argument.Split("=");

                arguments[keyValue[0].Split("--").Last()] = keyValue[1];
            }
        }

        return arguments;
    }
}
