using Godot;
using System;

public class Camera2D : Godot.Camera2D
{
	public override void _Ready()
	{
		var consts = Consts.GetSingleton();
		LimitLeft = 0;
		LimitTop = 0;
		LimitRight = consts.MapSize * consts.CellSize;
		LimitBottom = consts.MapSize * consts.CellSize;
	}

	public override void _Process(float delta)
	{
		var consts = Consts.GetSingleton();
		if (Input.IsKeyPressed((int)KeyList.Q) && Zoom.x <= consts.MaxZoom)
			Zoom = Zoom + new Vector2(0.05f, 0.05f);

		if (Input.IsKeyPressed((int)KeyList.E) && Zoom.x >= consts.MinZoom)
			Zoom = Zoom - new Vector2(0.05f, 0.05f);
	}
}
