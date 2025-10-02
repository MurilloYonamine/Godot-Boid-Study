using Godot;
using System.Collections.Generic;

public partial class UnitSpawner : Node2D
{
	// === SPAWN SETTINGS ===
	[Export] private PackedScene _unitScene;
	[Export] private int _spawnCount = 10;
	[Export] private Area2D _spawnArea;

	// === AREA DATA ===
	private RectangleShape2D _shape;
	private Vector2 _areaSize;
	private Vector2 _areaPos;

	public override void _Ready()
	{
		_shape = _spawnArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
		_areaSize = _shape.Size;
		_areaPos = _spawnArea.GlobalPosition;

		for (int i = 0; i < _spawnCount; i++)
		{
			Unit unit = CreateUnit(GetRandomSpawnPosition());
			GetParent().CallDeferred("add_child", unit);
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.G)
		{
			OnSpawnEnemyKeyPressed();
		}
	}
	private Vector2 GetRandomSpawnPosition()
	{
		Vector2 randomOffset = new Vector2(
			(float)GD.RandRange(-_areaSize.X / 2, _areaSize.X / 2),
			(float)GD.RandRange(-_areaSize.Y / 2, _areaSize.Y / 2)
		);
		return _areaPos + randomOffset;
	}

	private Unit CreateUnit(Vector2 position)
	{
		Unit unit = (Unit)_unitScene.Instantiate();
		unit.GlobalPosition = position;
		unit.SpawnArea = _spawnArea;
		unit.Visible = true;
		unit.SetAutoMoving(true);
		return unit;
	}

	public void OnSpawnEnemyKeyPressed()
	{
		Vector2 outsidePosition = _areaPos + new Vector2(_areaSize.X / 2 + 100, 0);
		Unit unit = CreateUnit(outsidePosition);

		Vector2 directionToCenter = (_areaPos - outsidePosition).Normalized();
		unit.SetAutoDirection(directionToCenter);

		GetParent().CallDeferred("add_child", unit);
	}
}
