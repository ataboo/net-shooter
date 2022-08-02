using System.Collections.Generic;
using System.Linq;
using Godot;

public class RemotePlayersControl : Node2D
{
    [Export]
    PackedScene remotePlayerPrefab;

    private NetService netService;

    private Dictionary<int, RemotePlayer> remoteNodes;

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");

        remoteNodes = new Dictionary<int, RemotePlayer>();

        netService.Connect(nameof(NetService.OnPlayerChange), this, nameof(HandlePlayerChange));

        HandlePlayerChange();
    }

    public void HandlePlayerChange() {
        foreach(var kvp in netService.PlayerNames) {
            if(kvp.Key == netService.PlayerID) {
                continue;
            }
            if(!remoteNodes.ContainsKey(kvp.Key)) {
                remoteNodes[kvp.Key] = InstantiateRemotePlayer(kvp.Key, kvp.Value);
                continue;
            }
        }

        foreach(var kvp in remoteNodes.Where(kvp => !netService.PlayerNames.ContainsKey(kvp.Key))) {
            kvp.Value.SetProcess(false);
            kvp.Value.QueueFree();
            remoteNodes.Remove(kvp.Key);
        }
    }

    RemotePlayer InstantiateRemotePlayer(int id, string name) {
        var newPlayer = remotePlayerPrefab.Instance<RemotePlayer>();
        AddChild(newPlayer);
        newPlayer.InitPlayer(name, id);

        return newPlayer;
    }
}
