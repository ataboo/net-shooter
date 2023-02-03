using Godot;
using System;

public class CameraControl : Camera2D
{
    public Node2D FocusTarget {get; set;}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if(FocusTarget != null) {
            this.Position = Position.LinearInterpolate(FocusTarget.Position, 0.8f);
        }   
    }
}
