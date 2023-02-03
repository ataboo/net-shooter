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
    private PlayerControl _playerControl;

    [Export]
    NodePath mapSpritePath;
    private Sprite _mapSprite;

    [Export]
    NodePath cameraControlPath;
    private CameraControl _cameraControl;


    private NetService netService;

    private Dictionary<string, RemotePlayer> vessels;

    private Dictionary<string, CharacterPayload> characters;

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");
        _playerControl = GetNode<PlayerControl>(playerControlPath);
        _mapSprite = GetNode<Sprite>(mapSpritePath);

        vessels = new Dictionary<string, RemotePlayer>();

        netService.Connect(nameof(NetService.OnResponse), this, nameof(HandleWsResponse));
        _playerControl.Connect(nameof(PlayerControl.OnSteeringChange), this, nameof(HandleSteeringChange));
		_cameraControl = GetNode<CameraControl>(cameraControlPath);
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
            case EngineEvtType.OutWorldUpdate:
                var world = res.ParsePayload<WorldPayload>();
                this.SetWorldTexture(world);
                break;
            default:
                GD.Print("Event not supported: " + res.type);
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

    private void SetWorldTexture(WorldPayload worldPayload) {
        var imgBytes =  Marshalls.Base64ToRaw(worldPayload.image);
        var img = new Image();
        img.LoadPngFromBuffer(imgBytes);
        img.Convert(Image.Format.Rgb8);
        img.Lock();
        for(var x=0; x<img.GetWidth(); x++) {
            for(var y=0; y<img.GetHeight(); y++) {
                img.SetPixel(x, y, ColourWorldPixel(img.GetPixel(x, y)));
            }
        }
        img.Unlock();

        var imgText = new ImageTexture();
        imgText.CreateFromImage(img, 3);
        _mapSprite.Scale = new Vector2(2, 2);
        _mapSprite.Texture = imgText;
    }

    private Color ColourWorldPixel(Color color) {
        var deep = 0.55f;
        var surface = 0.75f;
        
        if(color.r < deep) {
            color.r = 0f;
            color.g = 0.15f;
            color.b = 0.2f;
        } else if(color.r < surface) {
            var t = 1f - (color.r - deep) / (surface - deep);
            color.r = Mathf.Lerp(0.05f, 0f, t);
            color.g = Mathf.Lerp(0.4f, 0.15f, t);
            color.b = Mathf.Lerp(0.4f, 0.2f, t);
        } else {
            color.r -= .2f;
            color.g -= .2f;
            color.b -= .4f;
        }
        
        return color;
    }


    void HandleSteeringChange() {
        netService.SendGameEvent(EngineEvtType.InSteerUpdate, _playerControl.Steering);
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
                if(c.id == netService.PlayerID) {
                    _cameraControl.FocusTarget = vessels[c.vessel];
                }
            } else {
                GD.Print($"Failed to match {c.id} to {c.vessel}");
            }
        }
    }
}
