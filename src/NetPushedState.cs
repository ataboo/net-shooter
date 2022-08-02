using Godot;

public class NetPushedState : Node2D
{
    [Export]
    public float pollDelay = 0.1f;

    [Export]
    public bool sendNullPayload = false;

    [Export]
    public string stateName;

    private NetService netService;

    private object _payload;
    private bool dirty = false;

    private float pollCooldown;

    public void MarkDirty() {
        dirty = true;
    }

    public void SetPayload(object payload) {
        dirty = true;
        _payload = payload;
    }

    public override void _Ready()
    {
        netService = GetNode<NetService>("/root/NetService");
    }

    public override void _Process(float delta)
    {
        if(!netService.Joined) {
            return;
        }

        pollCooldown -= delta;
        if(pollCooldown < 0) {
            pollCooldown = pollDelay;

            if(dirty && (_payload != null || sendNullPayload)) {
                netService.SendGameEvent(stateName, _payload);
            }
            dirty = false;
        }
    }
}
