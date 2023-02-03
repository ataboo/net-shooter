
using System.Collections.Generic;
using System.Linq;
using AtaRoomNet;
using Godot;
using Newtonsoft.Json;

public class NetService: Node {
    private WebSocketClient ws;

    public string url;
    public string playerName;
    public string roomCode;
    public string gameID;

    public string PlayerID {get; private set;} = null;

    public bool Joined { get; private set; } = false;

    private bool _queueIncoming = false;
    private Queue<WSResponse> _msgQueue = new Queue<WSResponse>();

    [Signal]
    public delegate void OnJoined();
    [Signal]
    public delegate void OnResponse(WSResponse response);
    [Signal]
    public delegate void OnDisconnect();

    public override void _Ready()
    {
        SetProcess(false);
    }

    public Error ConnectWS()
    {
        if(Joined) {
            return Error.AlreadyInUse;
        }

        var maxBufferSize = 1000;
        var maxPackets = 1000;

        ws = new WebSocketClient();
        ws.VerifySsl = false;
        ws.SetBuffers(maxBufferSize, maxPackets, maxBufferSize, maxPackets);
        ws.Connect("connection_established", this, nameof(HandleConnected));
        ws.Connect("connection_error", this, nameof(HandleConnectionError));
        ws.Connect("connection_closed", this, nameof(HandleConnectionEnded));
        ws.Connect("data_received", this, nameof(HandleDataReceived));

        var err = ws.ConnectToUrl(url, new []{"atanet_v1"});
        if(err != Error.Ok) {
            GD.Print(err);
        }

        SetProcess(true);

        return Error.Ok;
    }

    public override void _Process(float delta)
    {
        ws.Poll();
    }

    public Error SendRequest(WSRequest request) {
        if(!Joined) {
            return Error.Unavailable;
        }

        return SendMessage(request);
    }

    public Error SendGameEvent<TPayload>(string type, TPayload payload) where TPayload : class {
        if(!Joined) {
            return Error.Unavailable;
        }

        var req = new WSRequest {
            type = type,
            id = new Godot.Object().GetInstanceId().ToString(),
            send = System.DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        };

        req.MarshalPayload(payload);

        return SendRequest(req);
    }

    public void WSDisconnect(bool sendDisconnect = true) {
        if(sendDisconnect) {
            ws.DisconnectFromHost();
        }
        SetProcess(false);
        EmitSignal(nameof(OnDisconnect));
        Joined = false;
    }

    private void HandleConnected(string protocol) {
        GD.Print("Connected");

        SendMessage(new WSJoinRequest{
            username = playerName,
            token = "abc-123",
        });
        // SendMessage(new WSJoinRequest{
        //     create = true,
        //     game_id = gameID,
        //     player_name = playerName,
        //     room_code = roomCode,
        //     room_size = 12
        // });
    }

    private void HandleConnectionEnded(bool wasCleanClose) {
        GD.Print("Connection ended");
        WSDisconnect(false);
    }

    private void HandleConnectionError() {
        GD.Print("Connection error");
        WSDisconnect(false);
    }

    public void QueueIncoming(bool active) {
        _queueIncoming = active;

        if(!_queueIncoming) {
            while(_msgQueue.Count > 0) {
                EmitSignal(nameof(OnResponse), _msgQueue.Dequeue());
            }
        }
    }

    private void HandleDataReceived() {
        var res = ReadMessage();
        if(res == null) {
            return;
        }

        if(_queueIncoming) {
            _msgQueue.Enqueue(res);
            return;
        }

        if(!Joined) {
            if(res.type == EngineEvtType.OutPlayerJoined) {
                EmitSignal(nameof(OnJoined));
                Joined = true;
                PlayerID = res.recipient;

                return;
            }
        }

        EmitSignal(nameof(OnResponse), res);
    }

    private Error SendMessage(object req) {
        try {
            var msgBytes = JsonConvert.SerializeObject(req).ToUTF8();
            return ws.GetPeer(1).PutPacket(msgBytes);
        } catch(System.Exception e) {
            GD.PrintErr(e);
            return Error.Failed;
        }
    }

    private WSResponse ReadMessage() {
        try {
            var msgJSON = ws.GetPeer(1).GetPacket().GetStringFromUTF8();
            return JsonConvert.DeserializeObject<WSResponse>(msgJSON);
        } catch(System.Exception e) {
            GD.PrintErr(e);
            return null;
        }
    }

    // private (WSResponse response, TPayload payload) ReadMessage<TPayload>() where TPayload : class {
    //     var response = ReadMessage();
    //     var payload = response.ParsePayload<TPayload>();

    //     return (response, payload);
    // }
}