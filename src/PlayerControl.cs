using Godot;
using AtaRoomNet;

public class PlayerControl : Node2D
{
	[Signal]
    public delegate void OnSteeringChange();

	public SteeringPayload Steering {get; private set;} = new SteeringPayload();

	private NetService netService;

	public override void _Ready()
	{
		netService = GetNode<NetService>("/root/NetService");
	}

	public void SetSteering(SteeringPayload steering) {
		this.Steering = new SteeringPayload {
			config = steering.config,
			steering = steering.steering,
			throttle = steering.throttle
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var movement = Vector2.Zero;
		var changed = false;
		if(Input.IsActionJustPressed("move_left")) {
			this.Steering.steering -= 1;
			changed = true;
		}
		if(Input.IsActionJustPressed("move_right")) {
			this.Steering.steering += 1;
			changed = true;
		}
		if(Input.IsActionJustPressed("move_up")) {
			this.Steering.throttle += 1;
			changed = true;
		}
		if(Input.IsActionJustPressed("move_down")) {
			this.Steering.throttle -= 1;
			changed = true;
		}

		if(changed) {
			this.Steering.throttle = Mathf.Clamp(this.Steering.throttle, -2, 4);
			this.Steering.steering = Mathf.Clamp(this.Steering.steering, -4, 4);

			EmitSignal(nameof(OnSteeringChange));
		}
	}
}
