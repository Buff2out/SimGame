using Godot;
using System;

public class Consts : Node
{
	[Export]
	public int MapSize = 1000;
	[Export]
	public int CellSize = 64;
	public int MapSizePix { get { return MapSize * CellSize; } }
	[Export]
	public float MaxZoom = 8.0f;
	[Export]
	public float MinZoom = 1.0f;
	[Export]
	public float StarvationLevel = 20.0f;
	[Export]
	public float LoveLevel = 85.0f;
	[Export]
	public int SexBreak = 60;
	[Export]
	public int VegeGrowthProb = 1000;
	[Export]
	public float TicksPerSecond = 1.0f;

	public override void _Ready()
	{
		self = this;
	}

	private static Consts self = null;
	public static Consts GetSingleton()
	{
		return self;
	}
}

