using Godot;

public partial class UnitSpawner : Node2D
{
	// === SPAWN SETTINGS ===
	[Export] private PackedScene[] _unitScenes;
	[Export] private int _spawnCount = 10;
	[Export] private Area2D _spawnArea;
	[Export] private float _spawnMargin = 80f;

	// === AREA DATA ===
	private Vector2 _areaSize;
	private Vector2 _areaPos;

	public override void _Ready()
	{
		RectangleShape2D shape = _spawnArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
		_areaSize = shape.Size;
		_areaPos = _spawnArea.GlobalPosition;

		// Spawn initial units
		for (int i = 0; i < _spawnCount; i++)
		{
			CallDeferred(nameof(CreateUnit), GetSafeSpawnPosition(), Vector2.Zero);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			switch (keyEvent.Keycode)
			{
				case Key.G: OnSpawnEnemyKeyPressed(); break;
				case Key.Key1: SpawnSpecificUnit(0, GetRandomSpawnPosition()); break;
				case Key.Key2: SpawnSpecificUnit(1, GetRandomSpawnPosition()); break;
				case Key.Key3: SpawnSpecificUnit(2, GetRandomSpawnPosition()); break;
			}
		}
	}

	private Vector2 GetSafeSpawnPosition()
	{
		// Calculate safe area with margin
		Vector2 safeAreaSize = _areaSize - new Vector2(_spawnMargin * 2, _spawnMargin * 2);
		return GetRandomPositionInArea(safeAreaSize);
	}

	private Vector2 GetRandomSpawnPosition() => GetRandomPositionInArea(_areaSize);

	private Vector2 GetRandomPositionInArea(Vector2 areaSize)
	{
		// Generate random position within area
		Vector2 randomOffset = new Vector2(
			(float)GD.RandRange(-areaSize.X / 2, areaSize.X / 2),
			(float)GD.RandRange(-areaSize.Y / 2, areaSize.Y / 2)
		);
		return _areaPos + randomOffset;
	}

	private PackedScene GetRandomUnitScene()
	{
		if (_unitScenes == null || _unitScenes.Length == 0) return null;

		// Select random unit type
		int randomIndex = GD.RandRange(0, _unitScenes.Length - 1);
		return _unitScenes[randomIndex];
	}

	private void CreateUnit(Vector2 position, Vector2 direction = default)
	{
		// Instantiate unit
		PackedScene sceneToSpawn = GetRandomUnitScene();
		if (sceneToSpawn == null) return;

		Unit unit = sceneToSpawn.Instantiate() as Unit;
		unit.GlobalPosition = position;
		GetParent().AddChild(unit);

		// Initialize unit behavior
		CallDeferred(nameof(InitializeUnitMovement), unit, direction);
	}

	private void InitializeUnitMovement(Unit unit, Vector2 direction)
	{
		if (!IsInstanceValid(unit)) return;

		// Setup unit movement parameters
		unit.InitializeMovementArea(_areaSize, _areaPos);
		unit.SetAutoMoving(true);

		if (direction != Vector2.Zero)
		{
			unit.SetAutoDirection(direction);
		}
	}

	public void OnSpawnEnemyKeyPressed()
	{
		// Spawn enemy from outside area
		Vector2 outsidePosition = _areaPos + new Vector2(_areaSize.X / 2 + 100f, 0f);
		Vector2 directionToCenter = (_areaPos - outsidePosition).Normalized();

		CallDeferred(nameof(CreateUnit), outsidePosition, directionToCenter);
	}

	public void SpawnSpecificUnit(int unitIndex, Vector2 position)
	{
		// Validate index and spawn specific unit
		if (unitIndex >= 0 && unitIndex < _unitScenes.Length)
		{
			CallDeferred(nameof(CreateSpecificUnitByIndex), unitIndex, position);
		}
	}

	private void CreateSpecificUnitByIndex(int unitIndex, Vector2 position)
	{
		// Safety check for unit index
		if (unitIndex < 0 || unitIndex >= _unitScenes.Length || _unitScenes[unitIndex] == null) return;
		
		// Instantiate specific unit type
		Unit unit = _unitScenes[unitIndex].Instantiate() as Unit;
		unit.GlobalPosition = position;
		GetParent().AddChild(unit);

		CallDeferred(nameof(InitializeUnitMovement), unit, Vector2.Zero);
	}
}
