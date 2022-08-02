using Godot;

public class PayloadVector {
    public string x;
    public string y;

    public static PayloadVector FromVector(Godot.Vector2 vector, int precision = 3) {
        var formatter = $"F{precision}";
        
        return new PayloadVector {
            x = vector.x.ToString(formatter),
            y = vector.y.ToString(formatter),
        };
    }

    public Vector2 ToVector() {
        return new Vector2 {
            x = float.Parse(x),
            y = float.Parse(y),
        };
    }
}