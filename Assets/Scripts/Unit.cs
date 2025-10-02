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
	private SpriteManager _spriteManager;
	
	// === BOID AVOIDANCE ===
	private Area2D _avoidanceArea;
	private List<Unit> _nearbyUnits = new List<Unit>();
	private const float INTERVAL_TIMER = 0.2f;
	private float _avoidanceTimer = 0f;
	private Vector2 _avoidanceDirection = Vector2.Zero;
	[Export] private float _avoidanceRadius = 60f;
	[Export] private float _repulsionStrength = 1.0f;
	private BoidBehavior _boidBehavior;

	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_targetPosition = GlobalPosition;

		// Initialize movement
		_unitMovement = new UnitMovement(this, _agent, _sprite2D, _speed);
		_unitMovement.SetRandomDirection();

		// Initialize sprite manager
		_spriteManager = new SpriteManager();
		_spriteManager.LoadSprites();
		AssignRandomSprite();

		// Initialize boid behavior
		_boidBehavior = new BoidBehavior(_repulsionStrength, _avoidanceRadius);
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
		_avoidanceDirection = _boidBehavior.CalculateAvoidanceVector(GlobalPosition, _nearbyUnits);
	}

	public Vector2 GetAvoidanceDirection()
	{
		return _avoidanceDirection;
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click for manual navigation
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			_unitMovement.HandleMouseNavigation(GetGlobalMousePosition());
		}
	}
	private void OnUnitEntered(Node2D body)
	{
		if (body is Unit unit && unit != this)
		{
			_nearbyUnits.Add(unit);
		}
	}

	private void OnUnitExited(Node2D body)
	{
		if (body is Unit unit)
		{
			_nearbyUnits.Remove(unit);
		}
	}

	private void AssignRandomSprite()
	{
		Texture2D sprite = _spriteManager.GetRandomSprite();
		if (sprite == null) return;
		_sprite2D.Texture = sprite;
	}

	// Public methods that delegate to MovementManager
	public void SetAutoDirection(Vector2 direction) => _unitMovement?.SetAutoDirection(direction);
	public void SetAutoMoving(bool value) => _unitMovement?.SetAutoMoving(value);
	public void InitializeMovementArea(Vector2 movementArea, Vector2 spawnAreaCenter)
	{
		_unitMovement?.InitializeMovementArea(movementArea, spawnAreaCenter);
	}
}
