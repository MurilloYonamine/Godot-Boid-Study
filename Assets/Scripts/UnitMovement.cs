using Godot;

public class UnitMovement
{
	// === VISUAL COMPONENTS ===
	private CharacterBody2D _unit;
	private Sprite2D _sprite2D;

	// === NAVIGATION COMPONENTS ===
	private NavigationAgent2D _agent;
	private Rid _navigationMap;

	// === MOVEMENT SETTINGS ===
	private float _speed;
	private bool _isAutoMoving = true;
	private Vector2 _autoDirection = Vector2.Right;
	private Vector2 _movementArea;
	private Vector2 _spawnAreaCenter;
	private bool _isInitialized = false;

	// === BOID INTEGRATION ===
	private Unit _unitReference;

	public UnitMovement(CharacterBody2D unit, NavigationAgent2D agent, Sprite2D sprite2D, float speed)
	{
		_unit = unit;
		_agent = agent;
		_sprite2D = sprite2D;
		_speed = speed;
		_navigationMap = unit.GetWorld2D().NavigationMap;
		_unitReference = unit as Unit;
	}

	public void SetRandomDirection()
	{
		float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);
		_autoDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	public void HandleMovement()
	{
		// Only handle movement if properly initialized
		if (!_isInitialized) return;

		if (_isAutoMoving)
		{
			HandleAutoMovement();
		}
		else
		{
			HandleNavigationMovement();
		}
	}

	private void HandleAutoMovement()
	{
		// Get avoidance direction from boids
		Vector2 avoidanceForce = Vector2.Zero;
		if (_unitReference != null)
			avoidanceForce = _unitReference.AvoidanceDirection;
			
		// Combine movement direction with avoidance (smooth blending)
		Vector2 combinedDirection = _autoDirection + avoidanceForce * 0.5f; // Reduce avoidance strength
		combinedDirection = combinedDirection.Normalized();

		// Apply smoothing to prevent sudden direction changes
		_autoDirection = _autoDirection.Lerp(combinedDirection, 0.1f);

		_unit.Velocity = _autoDirection * _speed;
		_unit.MoveAndSlide();

		// Handle boundary reflections
		HandleBoundaryReflection();

		_sprite2D.FlipH = _autoDirection.X < 0;
	}

	private void HandleBoundaryReflection()
	{
		// Use the defined movement area boundaries
		Vector2 areaMin = _spawnAreaCenter - _movementArea / 2;
		Vector2 areaMax = _spawnAreaCenter + _movementArea / 2;
		float margin = 50f;

		// Simple boundary reflection with smoothing
		bool shouldFlipX = false;
		bool shouldFlipY = false;

		if (_unit.GlobalPosition.X <= areaMin.X + margin || _unit.GlobalPosition.X >= areaMax.X - margin)
		{
			shouldFlipX = true;
		}
		
		if (_unit.GlobalPosition.Y <= areaMin.Y + margin || _unit.GlobalPosition.Y >= areaMax.Y - margin)
		{
			shouldFlipY = true;
		}

		// Apply boundary reflections smoothly
		if (shouldFlipX) _autoDirection.X = -_autoDirection.X;
		if (shouldFlipY) _autoDirection.Y = -_autoDirection.Y;
	}

	private void HandleNavigationMovement()
	{
		if (_agent.IsNavigationFinished())
		{
			if (_unit.Velocity != Vector2.Zero) 
				_autoDirection = _unit.Velocity.Normalized();

			_unit.Velocity = Vector2.Zero;
			_isAutoMoving = true;
			return;
		}

		Vector2 difference = _agent.GetNextPathPosition() - _unit.GlobalPosition;
		Vector2 direction = difference.Normalized();
		
		// Apply boid avoidance even during navigation
		Vector2 avoidanceForce = Vector2.Zero;
		if (_unitReference != null)
			avoidanceForce = _unitReference.AvoidanceDirection;

		// Blend navigation with avoidance
		Vector2 combinedDirection = (direction + avoidanceForce * 0.3f).Normalized();
		
		_unit.Velocity = combinedDirection * _speed;
		_unit.MoveAndSlide();

		_sprite2D.FlipH = combinedDirection.X < 0;
	}

	public void HandleMouseNavigation(Vector2 mousePosition)
	{
		if (!_isInitialized) return;
		
		Vector2 point = NavigationServer2D.MapGetClosestPoint(_navigationMap, mousePosition);
		_agent.TargetPosition = point;
		_isAutoMoving = false;
	}

	public void SetAutoDirection(Vector2 direction) => _autoDirection = direction.Normalized();
	public void SetAutoMoving(bool value) => _isAutoMoving = value;
	
	public void InitializeMovementArea(Vector2 movementArea, Vector2 spawnAreaCenter)
	{
		_movementArea = movementArea;
		_spawnAreaCenter = spawnAreaCenter;
		_isInitialized = true; 
	}
	
	public bool IsAutoMoving => _isAutoMoving;
	public Vector2 AutoDirection => _autoDirection;
}
