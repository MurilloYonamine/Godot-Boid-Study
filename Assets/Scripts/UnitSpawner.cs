using Godot;
using System.Collections.Generic;

public partial class UnitSpawner : Node2D
{
	// === SPAWN SETTINGS ===
	[Export] private PackedScene _unitScene;
	[Export] private int _spawnCount = 10;
	[Export] private Area2D _spawnArea;
	[Export] private float _spawnMargin = 80f; 

	// === AREA DATA ===
	private RectangleShape2D _shape;
	private Vector2 _areaSize;
	private Vector2 _areaPos;

	public override void _Ready()
	{
		_shape = _spawnArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
		_areaSize = _shape.Size;
		_areaPos = _spawnArea.GlobalPosition;

		// Use CallDeferred to create units after _Ready() completes
		for (int i = 0; i < _spawnCount; i++)
		{
			CallDeferred(nameof(CreateUnit), GetSafeSpawnPosition());
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.G)
		{
			OnSpawnEnemyKeyPressed();
		}
	}

	private Vector2 GetSafeSpawnPosition()
	{
		Vector2 safeAreaSize = _areaSize - new Vector2(_spawnMargin * 2, _spawnMargin * 2);
		
		Vector2 randomOffset = new Vector2(
			(float)GD.RandRange(-safeAreaSize.X / 2, safeAreaSize.X / 2),
			(float)GD.RandRange(-safeAreaSize.Y / 2, safeAreaSize.Y / 2)
		);
		Vector2 spawnPos = _areaPos + randomOffset;
		
		return spawnPos;
	}

	private Vector2 GetRandomSpawnPosition()
	{
		Vector2 randomOffset = new Vector2(
			(float)GD.RandRange(-_areaSize.X / 2, _areaSize.X / 2),
			(float)GD.RandRange(-_areaSize.Y / 2, _areaSize.Y / 2)
		);
		Vector2 spawnPos = _areaPos + randomOffset;
		
		return spawnPos;
	}

	private void CreateUnit(Vector2 position)
	{
		Unit unit = _unitScene.Instantiate() as Unit;
		unit.GlobalPosition = position;
		unit.Visible = true;
		
		GetParent().AddChild(unit);
		
		// Initialize movement after Unit is in the scene tree
		CallDeferred(nameof(InitializeUnitMovement), unit, position, Vector2.Zero);
	}

	private void InitializeUnitMovement(Unit unit, Vector2 position, Vector2 direction)
	{
		if (!IsInstanceValid(unit)) return; 
		
		unit.InitializeMovementArea(_areaSize, _areaPos);
		unit.SetAutoMoving(true);
		
		// Set custom direction if provided, otherwise use random
		if (direction != Vector2.Zero)
		{
			unit.SetAutoDirection(direction);
		}
	}

	public void OnSpawnEnemyKeyPressed()
	{
		float outsideOffsetX = 100f;
		float outsideOffsetY = 0f;
		Vector2 outsidePosition = _areaPos + new Vector2(_areaSize.X / 2 + outsideOffsetX, outsideOffsetY);
		
		// Use CallDeferred to avoid potential issues
		CallDeferred(nameof(CreateEnemyUnit), outsidePosition);
	}

	private void CreateEnemyUnit(Vector2 position)
	{
		Unit unit = _unitScene.Instantiate() as Unit;
		unit.GlobalPosition = position;
		unit.Visible = true;
		
		GetParent().AddChild(unit);
		
		Vector2 directionToCenter = (_areaPos - position).Normalized();
		CallDeferred(nameof(InitializeUnitMovement), unit, position, directionToCenter);
	}
}
