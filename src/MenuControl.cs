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
	public LineEdit nameInput;

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
		nameInput = GetNode<LineEdit>(nameInputPath);

		joinButton.Connect("pressed", this, nameof(HandleJoinClick));
		nameInput.Connect("gui_input", this, nameof(HandleNameInput));

		netService.Connect(nameof(NetService.OnJoined), this, nameof(HandleJoined));
		netService.Connect(nameof(NetService.OnDisconnect), this, nameof(HandleDisconnect));
		netService.Connect(nameof(NetService.OnResponse), this, nameof(HandleResponse));
	}

	public void HandleJoinClick() {
		netService.url = serverInput.Text;
		netService.playerName = nameInput.Text;
		netService.gameID = "Net Shooter";
		
		netService.ConnectWS();

		joinButton.Disabled = true;
	}

	void HandleJoined() {
		netService.QueueIncoming(true);
		GetTree().ChangeScene("res://Scenes/MapScene.tscn");
	}

	void HandleDisconnect() {
		joinButton.Disabled = false;
	}

	void HandleNameInput(InputEvent evt) {
		if(evt is InputEventKey keyEvt) {
			GD.Print(keyEvt.Scancode, keyEvt.Pressed);
			if(keyEvt.Pressed && keyEvt.Scancode == (int)KeyList.Enter || keyEvt.Scancode == (int)KeyList.KpEnter) {
				this.HandleJoinClick();
			}
		}
	}

	void HandleResponse(WSResponse response) {
		if(response.type == EngineEvtType.OutPlayerReject) {
			joinButton.Disabled = false;
			// var payload = response.ParsePayload<dynamic>();
			// string[] messages = (string[])payload.messages;

			// foreach(var message in messages) {
			//     GD.Print("Join Err: " + message);
			// }
		}
	}
}
