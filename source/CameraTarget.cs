using Godot;
using System;

public class CameraTarget : Node2D
{
	[Export]
	public NodePath CameraPath;
	private Camera2D Camera = null;
	[Export]
	public float Speed = 250.0f;

	public override void _Ready()
	{
		Camera = GetNode<Camera2D>(CameraPath);
	}

	public override void _Process(float delta)
	{
		Vector2 dir = new Vector2();
		if (Input.IsKeyPressed((int)KeyList.W))
			dir = Vector2.Up;

		if (Input.IsKeyPressed((int)KeyList.D))
			dir = dir + Vector2.Right;

		if (Input.IsKeyPressed((int)KeyList.S))
			dir = dir + Vector2.Down;

		if (Input.IsKeyPressed((int)KeyList.A))
			dir = dir + Vector2.Left;

		dir = dir.Normalized();

		var consts = Consts.GetSingleton();
		var cameraHalfSize = Camera.Zoom * (GetViewport().Size / 2);
		Vector2 minPos = new Vector2(0, 0) + cameraHalfSize;
		Vector2 maxPos = new Vector2(consts.MapSizePix, consts.MapSizePix) - cameraHalfSize;
		
		Vector2 newPos = Position + dir * Speed * Camera.Zoom * delta;

		newPos = new Vector2(Math.Max(Math.Min(newPos.x, maxPos.x), minPos.x), Math.Max(Math.Min(newPos.y, maxPos.y), minPos.y));

		Position = newPos;
	}
}
