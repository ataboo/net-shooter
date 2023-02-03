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

    private Vector2? targetPosition;

    private float targetRotation;

    private string vesselId;

    private string playerId;

    public override void _Ready()
    {
        playerName = GetNode<Label>(playerNamePath);
        playerSprite = GetNode<Sprite>(playerSpritePath);
    }

    public void InitPlayer(string id) {
        this.vesselId = id;
    }

    public override void _Process(float delta)
    {
        if(targetPosition.HasValue) {
            Position = Position.LinearInterpolate(targetPosition.Value, 10f * delta);
            playerSprite.Rotation = Mathf.LerpAngle(playerSprite.Rotation, targetRotation, 10f * delta);
        }
    }

    public void VesselUpdate(VesselUpdatePayload update) {
        targetPosition = new Vector2((float)update.tran.pos.x + 400, -(float)update.tran.pos.y + 300);
        targetRotation = (float)update.tran.hdg;
    }

    public void CharacterUpdate(CharacterPayload character) {
        playerName.Text = character.name;
        playerId = character.id;
    }
}
