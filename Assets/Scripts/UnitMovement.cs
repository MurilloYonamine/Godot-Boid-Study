using Godot;

public class UnitMovement
{
	// === COMPONENT REFERENCES ===
	private CharacterBody2D _unit;
	private Sprite2D _sprite2D;

	// === NAVIGATION SYSTEM ===
	private NavigationAgent2D _agent;
	private Rid _navigationMap;

	// === MOVEMENT CONFIGURATION ===
	private float _speed;
	private bool _isAutoMoving = true;
	private Vector2 _autoDirection = Vector2.Right;
	private Vector2 _movementArea;
	private Vector2 _spawnAreaCenter;
	private bool _isInitialized = false;

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
		// Validate initialization before processing
		if (!_isInitialized) return;

		// Choose movement mode based on current state
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
		const float DIRECTION_SMOOTHING = 0.1f;
		const float AVOIDANCE_STRENGTH = 0.5f;

		// BOID INTEGRATION
		Vector2 avoidanceForce = Vector2.Zero;
		if (_unitReference != null)
		{
			avoidanceForce = _unitReference.AvoidanceDirection;
		}

		// Blend autonomous movement with avoidance behavior
		Vector2 combinedDirection = _autoDirection + avoidanceForce * AVOIDANCE_STRENGTH;
		combinedDirection = combinedDirection.Normalized();

		// Smooth direction changes to prevent jittery movement
		_autoDirection = _autoDirection.Lerp(combinedDirection, DIRECTION_SMOOTHING);

		// === MOVEMENT EXECUTION ===
		_unit.Velocity = _autoDirection * _speed;
		_unit.MoveAndSlide();

		HandleBoundaryReflection();

		// Update sprite orientation
		_sprite2D.FlipH = _autoDirection.X < 0;
	}

	private void HandleBoundaryReflection()
	{
		// Calculate movement area boundaries
		Vector2 areaMin = _spawnAreaCenter - _movementArea / 2;
		Vector2 areaMax = _spawnAreaCenter + _movementArea / 2;
		float margin = 50f;

		// Detect boundary collisions
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

		// Apply direction reflection for boundary bouncing
		if (shouldFlipX) _autoDirection.X = -_autoDirection.X;
		if (shouldFlipY) _autoDirection.Y = -_autoDirection.Y;
	}

	private void HandleNavigationMovement()
	{
		// Check if navigation target reached
		if (_agent.IsNavigationFinished())
		{
			// Preserve current direction for smooth transition
			if (_unit.Velocity != Vector2.Zero)
				_autoDirection = _unit.Velocity.Normalized();

			// Switch back to autonomous movement
			_unit.Velocity = Vector2.Zero;
			_isAutoMoving = true;
			return;
		}

		// Path Following with avoidence
		Vector2 difference = _agent.GetNextPathPosition() - _unit.GlobalPosition;
		Vector2 direction = difference.Normalized();

		// Integrate boid avoidance during navigation
		Vector2 avoidanceForce = Vector2.Zero;
		if (_unitReference != null)
			avoidanceForce = _unitReference.AvoidanceDirection;

		// Combine navigation direction with avoidance
		Vector2 combinedDirection = (direction + avoidanceForce * 0.3f).Normalized();

		// Execute navigation movement
		_unit.Velocity = combinedDirection * _speed;
		_unit.MoveAndSlide();

		_sprite2D.FlipH = combinedDirection.X < 0;
	}

	public void HandleMouseNavigation(Vector2 mousePosition)
	{
		if (!_isInitialized) return;

		// Set navigation target to clicked position
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

}
