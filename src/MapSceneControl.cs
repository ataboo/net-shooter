using Godot;

public class MapSceneControl : Node2D
{
    private NetService netService;

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");

        netService.Connect(nameof(NetService.OnDisconnect), this, nameof(HandleNetDisconnect));

        netService.QueueIncoming(false);
    }

    private void HandleNetDisconnect() {
		  GetTree().ChangeScene("res://Scenes/MenuScene.tscn");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
