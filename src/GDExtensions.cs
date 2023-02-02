using AtaRoomNet;

public static class GDExtensions {
    public static PayloadVector ToPayload(this Godot.Vector2 vector, int precision = 3) {
        var formatter = $"F{precision}";
        
        return new PayloadVector {
            x = Godot.Mathf.Stepify(vector.x, precision),
            y = Godot.Mathf.Stepify(vector.y, precision),
        };
    }

    public static Godot.Vector2 ToVector(this PayloadVector payloadVector) {
        return new Godot.Vector2 {
            x = (float)payloadVector.x,
            y = (float)payloadVector.y,
        };
    }
}