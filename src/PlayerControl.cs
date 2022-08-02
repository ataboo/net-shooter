using Godot;

public class PlayerControl : Node2D
{
	[Export]
	public float speed = 10f;

	[Export]
	public NodePath netStatePath;
	private NetPushedState netState;
	[Export]
    public NodePath playerNamePath;
    private Label playerName;
    [Export]
    public NodePath playerSpritePath;
    private Sprite playerSprite;

	private NetService netService;

	public override void _Ready()
	{
		netState = GetNode<NetPushedState>(netStatePath);
		playerName = GetNode<Label>(playerNamePath);
		playerSprite = GetNode<Sprite>(playerSpritePath);
		netService = GetNode<NetService>("/root/NetService");

		playerName.Text = netService.playerName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var movement = Vector2.Zero;
		if(Input.IsActionPressed("move_down")) {
			movement.y += 1;
		}
		if(Input.IsActionPressed("move_up")) {
			movement.y -= 1;
		}
		if(Input.IsActionPressed("move_left")) {
			movement.x -= 1;
		}
		if(Input.IsActionPressed("move_right")) {
			movement.x += 1;
		}

		movement = movement.Normalized();
		var lastPos = Position;
		Position += movement * speed * delta;

		if(movement.LengthSquared() > 0) {
			playerSprite.Rotation = Mathf.Atan2(movement.y, movement.x) + Mathf.Pi / 2f;
		}

		if(lastPos != Position) {
			var payload = new PlayerUpdatePayload {
				pos = PayloadVector.FromVector(Position)
			};
			netState.SetPayload(payload);

			GD.Print($"x: {payload.pos.x}, y: {payload.pos.y}");
		}
	}
}
