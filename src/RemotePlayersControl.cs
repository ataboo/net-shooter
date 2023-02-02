using System.Collections.Generic;
using System.Linq;
using AtaRoomNet;
using Godot;

public class RemotePlayersControl : Node2D
{
    [Export]
    PackedScene vesselPrefab;

    [Export]
    NodePath playerControlPath;
    PlayerControl playerControl;


    private NetService netService;

    private Dictionary<string, RemotePlayer> vessels;

    private Dictionary<string, CharacterPayload> characters;

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");
        playerControl = GetNode<PlayerControl>(playerControlPath);

        vessels = new Dictionary<string, RemotePlayer>();

        netService.Connect(nameof(NetService.OnResponse), this, nameof(HandleWsResponse));
        playerControl.Connect(nameof(PlayerControl.OnSteeringChange), this, nameof(HandleSteeringChange));
    }

    void HandleWsResponse(WSResponse res) {
        switch(res.type) {
            case EngineEvtType.OutVesselUpdate:
                var payload = res.ParsePayload<VesselUpdatePayload>();
                if(!vessels.ContainsKey(payload.id)) {
                    var newVessel = this.InstantiateVessel(payload);
                    vessels[payload.id] = newVessel;

                    MatchCharactersToVessels();
                }

                var vessel = vessels[payload.id];
                vessel.VesselUpdate(payload);
                break;
            case EngineEvtType.OutVesselRemove:
                break;
            case EngineEvtType.OutCharUpdate:
                var charArray = res.ParsePayload<CharacterPayload[]>();
                characters = charArray.ToDictionary(c => c.id, c => c);
                GD.Print(characters);
                MatchCharactersToVessels();
                
                break;
            default:
                break;
        }
        
        // foreach(var kvp in netService.PlayerNames) {
        //     if(kvp.Key == netService.PlayerID) {
        //         continue;
        //     }
        //     if(!remoteNodes.ContainsKey(kvp.Key)) {
        //         remoteNodes[kvp.Key] = InstantiateRemotePlayer(kvp.Key, kvp.Value);
        //         continue;
        //     }
        // }

        // foreach(var kvp in remoteNodes.Where(kvp => !netService.PlayerNames.ContainsKey(kvp.Key))) {
        //     kvp.Value.SetProcess(false);
        //     kvp.Value.QueueFree();
        //     remoteNodes.Remove(kvp.Key);
        // }
    }


    void HandleSteeringChange() {
        netService.SendGameEvent(EngineEvtType.InSteerUpdate, playerControl.Steering);
    }


    RemotePlayer InstantiateVessel(VesselUpdatePayload vesselData) {
        var newPlayer = vesselPrefab.Instance<RemotePlayer>();
        AddChild(newPlayer);
        newPlayer.InitPlayer(vesselData.id);

        return newPlayer;
    }

    void MatchCharactersToVessels() {
        if(characters == null || vessels == null) {
            return;
        }

        foreach(var c in characters.Values) {
            if(!string.IsNullOrEmpty(c.vessel) && vessels.ContainsKey(c.vessel)) {
                GD.Print($"Matched {c.id} to {c.vessel}");
                vessels[c.vessel].CharacterUpdate(c);
            } else {
                GD.Print($"Failed to match {c.id} to {c.vessel}");
            }
        }
    }
}
