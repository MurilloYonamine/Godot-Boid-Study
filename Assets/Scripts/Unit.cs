using Godot;
using System;

public partial class Unit : CharacterBody2D
{
	// === NAVIGATION COMPONENTS ===
	private NavigationAgent2D _agent;
	private Vector2 _targetPosition;
	private Rid _navigationMap;

	// === MOVEMENT SETTINGS ===
	[Export] private float _speed = 300f;
	private bool _isAutoMoving = true;
	private Vector2 _autoDirection = Vector2.Right;

	// === VISUAL COMPONENTS ===
	private Sprite2D _sprite2D;
	[Export] public Texture2D[] SpriteOptions;

	// === SPAWN AREA DATA ===
	public Area2D SpawnArea { get; set; }
	private RectangleShape2D _spawnShape;
	private Vector2 _spawnAreaCenter;
	private Vector2 _spawnAreaSize;

	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_targetPosition = GlobalPosition;

		AssignRandomSprite();

		_spawnShape = SpawnArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
		_spawnAreaSize = _spawnShape.Size;
		_spawnAreaCenter = SpawnArea.GlobalPosition;

		_navigationMap = GetWorld2D().NavigationMap;
	}

	private void AssignRandomSprite()
	{
		if (SpriteOptions.Length > 0)
		{
			int index = (int)(GD.Randi() % SpriteOptions.Length);
			_sprite2D.Texture = SpriteOptions[index];
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click for manual navigation
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			Vector2 point = NavigationServer2D.MapGetClosestPoint(_navigationMap, GetGlobalMousePosition());
			_agent.TargetPosition = point;
			_isAutoMoving = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isAutoMoving)
		{
			Rect2 areaRect = new Rect2(_spawnAreaCenter - _spawnAreaSize / 2, _spawnAreaSize);

			Velocity = _autoDirection * _speed;
			MoveAndSlide();

			if (!areaRect.HasPoint(GlobalPosition))
			{
				_autoDirection = -_autoDirection;
			}

			_sprite2D.FlipH = _autoDirection == Vector2.Left;
		}
		else
		{
			if (_agent.IsNavigationFinished())
			{
				Velocity = Vector2.Zero;
				Vector2 newTarget = GetRandomPositionInSpawnArea();
				Vector2 point = NavigationServer2D.MapGetClosestPoint(_navigationMap, newTarget);
				_agent.TargetPosition = point;
				return;
			}

			LookAt(_agent.GetNextPathPosition());
			Vector2 difference = _agent.GetNextPathPosition() - GlobalPosition;
			Vector2 direction = difference.Normalized();
			Velocity = direction * _speed;
			MoveAndSlide();
		}
	}

	private Vector2 GetRandomPositionInSpawnArea()
	{
		return _spawnAreaCenter + new Vector2(
			(float)GD.RandRange(-_spawnAreaSize.X / 2, _spawnAreaSize.X / 2),
			(float)GD.RandRange(-_spawnAreaSize.Y / 2, _spawnAreaSize.Y / 2)
		);
	}
	public void SetAutoDirection(Vector2 direction) => _autoDirection = direction.Normalized();
	public void SetAutoMoving(bool value) => _isAutoMoving = value;
}
