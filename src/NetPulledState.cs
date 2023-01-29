using AtaRoomNet;
using Godot;

public class NetPulledState: Node {
    [Export]
    public int playerID;

    [Export]
    public string eventName;

    private NetService netService;

    public WSResponse LastResponse {get; private set;}

    [Signal]
    public delegate void OnStateChange(WSResponse res);

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");

        netService.Connect(nameof(NetService.OnResponse), this, nameof(HandleResponse));
    }

    public void HandleResponse(WSResponse response) {
        if(response.type != eventName) {
            return;
        }

        LastResponse = response;

        EmitSignal(nameof(OnStateChange), response);

        GD.Print("emitted");
    }
}