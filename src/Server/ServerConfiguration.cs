using Godot;
using System.Text;
using static Godot.ENetConnection;

namespace Multinet.Server;

public partial class ServerConfiguration : Resource
{
    [Export]
    public int ServerPort = 9810;

    [Export]
    public CompressionMode ConnectionCompression = CompressionMode.RangeCoder;

    [Export]
    public float ServerTimeUpdateInterval = 100f;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"{nameof(ServerPort)}: {ServerPort}");
        sb.Append("\n");
        sb.Append($"{nameof(ConnectionCompression)}: {ConnectionCompression}");
        sb.Append("\n");

        return sb.ToString();
    }
}
