using Godot;
using SimGame;
using SimGame.source;
using System;
using System.Collections.Generic;

public class Game : Node
{
	[Export]
	public int NumCharsToSpawn = 5000;
	[Export]
	public int NumVegetationToSpawn = 30000;
	private int currentId = 0;
	public int NextId { get { return currentId++; } }
	public Rect2 SpawnArea = new Rect2(0, 0, 30, 30);
	private RandomNumberGenerator random = new RandomNumberGenerator();
	[Export]
	public NodePath GroundTileMapPath;

	public static Game GetSingleton()
	{
		return self;
	}

	public override void _Ready()
	{
		self = this;
		var consts = Consts.GetSingleton();
		SpawnArea = new Rect2(0, 0, consts.MapSize, consts.MapSize);

		// Setting ground
		var tileMap = GetNode<TileMap>(GroundTileMapPath);
		for (int i = 0; i < consts.MapSize; i++)
			for (int j = 0; j < consts.MapSize; j++)
				tileMap.SetCell(i, j, 0);

		// Spawning characters
		for (int i = 0; i < NumCharsToSpawn; i++)
		{
			Creature creature = null;
			int type_i = random.RandiRange(0, 2);
			if (type_i == 0)
				creature = new Lion();
			else if (type_i == 1)
				creature = new Bear();
			else
				creature = new Goat();

			creature.ChessLocation = GetRandomPosition(SpawnArea);
		}

		// Spawning vegetation
		vegeBeds = new bool[consts.MapSize][];
		for (int i = 0; i < consts.MapSize; ++i)
        {
			vegeBeds[i] = new bool[consts.MapSize];
        }

		for (int i = 0; i < NumVegetationToSpawn; i++)
		{
			var pos = GetRandomPosition(SpawnArea);
			if (!IsVegeAt(pos)) 
			{
				var creature = new Vegetation();
				creature.ChessLocation = pos;
				SetVegeAt(pos, true);
			}
		}
	}

	public override void _Process(float delta)
    {
		var consts = Consts.GetSingleton();

		Creature[] creaturesCopy = new Creature[creatures.Count];
		creatures.CopyTo(creaturesCopy);

		uint currentTimeMS = OS.GetTicksMsec();
		uint timeSinceLastTickMS = currentTimeMS - lastTickTimeMS;
		uint timeForTickMS = (uint)Math.Floor(1000.0f / consts.TicksPerSecond);
		if (timeSinceLastTickMS > timeForTickMS)
		{
			currentTick++;
			lastTickTimeMS = currentTimeMS;
			timeSinceLastTickMS = timeSinceLastTickMS - timeForTickMS;

			foreach (var creature in creaturesCopy)
				if (!creature.MarkedForDelete)
					creature.Tick();
		}
		
		foreach (var creature in creaturesCopy)
			if (!creature.MarkedForDelete)
				creature.Update(Mathf.Clamp((float)timeSinceLastTickMS / timeForTickMS, 0.0f, 1.0f));
    }

	private Vector2 GetRandomPosition(Rect2 rect)
	{
		return new Vector2(random.RandiRange((int)rect.Position.x, (int)rect.Position.x + (int)rect.Size.x - 1),
							random.RandiRange((int)rect.Position.y, (int)rect.Position.y + (int)rect.Size.y - 1));
	}

	public Vector2 WorldClamp(Vector2 v)
    {
		var consts = Consts.GetSingleton();
		return new Vector2(Mathf.Clamp(v.x, 0.0f, consts.MapSize - 1), Mathf.Clamp(v.y, 0.0f, consts.MapSize - 1));
    }

	public bool IsValidPosition(Vector2 v)
    {
		return WorldClamp(v) == v;
    }

	public bool IsVegeAt(Vector2 v)
    {
		DebugTools.Assert(IsValidPosition(v), "Invalid position");
		return VegeBeds[(int)v.y][(int)v.x];
    }

	public void SetVegeAt(Vector2 v, bool value)
    {
		DebugTools.Assert(IsValidPosition(v), "Invalid position");
		VegeBeds[(int)v.y][(int)v.x] = value;
	}

	private static Game self = null;
	private List<Creature> creatures = new List<Creature>();
	public List<Creature> Creatures { get { return creatures; } }
	private bool[][] vegeBeds;
	public bool[][] VegeBeds { get { return vegeBeds; } }
	private uint lastTickTimeMS = 0;
	public uint LastTickTimeMS { get { return lastTickTimeMS; } }
	private int currentTick = 0;
	public int CurrentTick { get { return currentTick; } }
}
