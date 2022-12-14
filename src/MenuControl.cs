using AtaRoomNet;
using Godot;

public class MenuControl : Control
{
    [Export]
    public NodePath joinButtonPath;
    private Button joinButton;
    // [Export]
    // public NodePath pingIconPath;
    // private Image pingIcon;
    [Export]
    public NodePath serverInputPath;
    private TextEdit serverInput;
    [Export]
    public NodePath nameInputPath;
    public TextEdit nameInput;
    [Export]
    public NodePath roomInputPath;
    private TextEdit roomInput;

    private NetService netService;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");
        joinButton = GetNode<Button>(joinButtonPath);
        // pingIcon = GetNode<Image>(pingIconPath);
        serverInput = GetNode<TextEdit>(serverInputPath);
        nameInput = GetNode<TextEdit>(nameInputPath);
        roomInput = GetNode<TextEdit>(roomInputPath);

        joinButton.Connect("pressed", this, nameof(HandleJoinClick));

        netService.Connect(nameof(NetService.OnJoined), this, nameof(HandleJoined));
        netService.Connect(nameof(NetService.OnDisconnect), this, nameof(HandleDisconnect));
        netService.Connect(nameof(NetService.OnResponse), this, nameof(HandleResponse));
    }

    public void HandleJoinClick() {
        netService.url = serverInput.Text;
        netService.playerName = nameInput.Text;
        netService.roomCode = roomInput.Text;
        netService.gameID = "Net Shooter";
        
        netService.ConnectWS();

        joinButton.Disabled = true;
    }

    void HandleJoined() {
        GetTree().ChangeScene("res://Scenes/MapScene.tscn");
    }

    void HandleDisconnect() {
        joinButton.Disabled = false;
    }

    void HandleResponse(WSResponse response) {
        if(response.type == ResponseType.JoinReject) {
            joinButton.Disabled = false;
            // var payload = response.ParsePayload<dynamic>();
            // string[] messages = (string[])payload.messages;

            // foreach(var message in messages) {
            //     GD.Print("Join Err: " + message);
            // }
        }
    }
}
