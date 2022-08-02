using AtaRoomNet;
using Godot;

public class RemotePlayer : Node2D
{
	[Export]
    public NodePath playerNamePath;
    private Label playerName;
    [Export]
    public NodePath playerSpritePath;
    private Sprite playerSprite;
    [Export]
    public NodePath netStatePath;
    private NetPulledState netState;

    private Vector2? targetPosition;

    public override void _Ready()
    {
        playerName = GetNode<Label>(playerNamePath);
        playerSprite = GetNode<Sprite>(playerSpritePath);
        netState = GetNode<NetPulledState>(netStatePath);

        netState.Connect(nameof(NetPulledState.OnStateChange), this, nameof(HandleStateChange));
    }

    public void InitPlayer(string name, int id) {
        playerName.Text = name;
    }

    public override void _Process(float delta)
    {
        if(targetPosition.HasValue) {
            var deltaPos = targetPosition.Value - Position;
            if(deltaPos.Length() < 0.001) {
                Position = targetPosition.Value;

            } else {
                Position = Position.LinearInterpolate(targetPosition.Value, 10f * delta);
                playerSprite.Rotation = Mathf.Atan2(deltaPos.y, deltaPos.x) + Mathf.Pi / 2f;
            }
        }
    }

    public void HandleStateChange(WSResponse response) {
        var payload = response.ParsePayload<PlayerUpdatePayload>();
        targetPosition = payload.pos.ToVector();
    }
}
