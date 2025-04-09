using Godot;
using System.Text;

namespace Multinet.Client;

public partial class ClientConfiguration : Resource
{
    [Export]
    public string ServerAddress = "127.0.0.1";

    [Export]
    public int ServerPort = 9810;

    [Export]
    public ENetConnection.CompressionMode ConnectionCompression = ENetConnection.CompressionMode.RangeCoder;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"{nameof(ServerAddress)}: {ServerAddress}");
        sb.Append("\n");
        sb.Append($"{nameof(ServerPort)}: {ServerPort}");
        sb.Append("\n");
        sb.Append($"{nameof(ConnectionCompression)}: {ConnectionCompression}");
        sb.Append("\n");

        return sb.ToString();
    }
}
