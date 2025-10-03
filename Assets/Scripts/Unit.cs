using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : CharacterBody2D
{
	// === NAVIGATION COMPONENTS ===
	private NavigationAgent2D _agent;
	private Vector2 _targetPosition;

	// === MOVEMENT SETTINGS ===
	[Export] private float _speed = 300f;
	private UnitMovement _unitMovement;

	// === VISUAL COMPONENTS ===
	private Sprite2D _sprite2D;
	// private SpriteManager _spriteManager;

	// === BOID AVOIDANCE ===
	private Area2D _avoidanceArea;
	public List<Unit> NearbyUnits { get; private set; } = new List<Unit>();
	public Vector2 AvoidanceDirection { get; private set; } = Vector2.Zero;
	public float AvoidanceRadius { get; private set; } = 60f;
	public float _repulsionStrength { get; private set; } = 2.0f;
	[Export] public float Mass { get; private set; } = 1.0f;

	private const float INTERVAL_TIMER = 0.2f;
	private float _avoidanceTimer = 0f;
	private BoidBehavior _boidBehavior;

	// === DEBUG VISUALIZATION ===
	private DebugUnitDrawer _debugDrawer;


	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_targetPosition = GlobalPosition;

		_unitMovement = new UnitMovement(this, _agent, _sprite2D, _speed);
		_unitMovement.SetRandomDirection();

		// _spriteManager = new SpriteManager();
		// _spriteManager.LoadSprites();
		// AssignRandomSprite();

		_boidBehavior = new BoidBehavior(_repulsionStrength, AvoidanceRadius);

		SetupAvoidanceArea();

		_debugDrawer = new DebugUnitDrawer();
		AddChild(_debugDrawer);
	}

	public override void _PhysicsProcess(double delta)
	{
		_avoidanceTimer += (float)delta;

		if (_avoidanceTimer >= INTERVAL_TIMER)
		{
			CalculateAvoidance();
			_avoidanceTimer = 0f;
		}

		_unitMovement.HandleMovement();
	}

	private void CalculateAvoidance()
	{
		AvoidanceDirection = _boidBehavior.CalculateAvoidanceVector(GlobalPosition, this, NearbyUnits);
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click for manual navigation
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			_unitMovement.HandleMouseNavigation(GetGlobalMousePosition());
		}
	}
	private void SetupAvoidanceArea()
	{
		_avoidanceArea = new Area2D();
		_avoidanceArea.Name = "AvoidanceArea";
		AddChild(_avoidanceArea);

		CircleShape2D shape = new CircleShape2D();
		shape.Radius = AvoidanceRadius;

		CollisionShape2D collision = new CollisionShape2D();
		collision.Shape = shape;
		_avoidanceArea.AddChild(collision);

		_avoidanceArea.AreaEntered += OnAreaEntered;
		_avoidanceArea.AreaExited += OnAreaExited;
	}

	private void OnAreaEntered(Area2D area)
	{
		Node parent = area.GetParent();
		if (parent is Unit unit && unit != this)
		{
			NearbyUnits.Add(unit);
			_debugDrawer?.QueueRedraw();
		}
	}

	private void OnAreaExited(Area2D area)
	{
		Node parent = area.GetParent();
		if (parent is Unit unit)
		{
			NearbyUnits.Remove(unit);
			_debugDrawer?.QueueRedraw();
		}
	}

	// private void AssignRandomSprite()
	// {
	// 	Texture2D sprite = _spriteManager.GetRandomSprite();
	// 	if (sprite == null) return;
	// 	_sprite2D.Texture = sprite;
	// }

	public void SetAutoDirection(Vector2 direction) => _unitMovement?.SetAutoDirection(direction);
	public void SetAutoMoving(bool value) => _unitMovement?.SetAutoMoving(value);
	public void InitializeMovementArea(Vector2 movementArea, Vector2 spawnAreaCenter)
	{
		_unitMovement?.InitializeMovementArea(movementArea, spawnAreaCenter);
	}
}
