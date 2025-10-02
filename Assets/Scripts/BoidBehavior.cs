using Godot;
using System.Collections.Generic;

public class BoidBehavior
{
    private float _repulsionStrength;
    private float _detectionRadius;

    public BoidBehavior(float repulsionStrength = 1.0f, float detectionRadius = 60f)
    {
        _repulsionStrength = repulsionStrength;
        _detectionRadius = detectionRadius;
    }

    public Vector2 CalculateAvoidanceVector(Vector2 position, List<Unit> nearbyUnits)
    {
        if (nearbyUnits.Count == 0) return Vector2.Zero;

        // Find the closest unit
        Unit closestUnit = null;
        float closestDistance = float.MaxValue;

        foreach (Unit unit in nearbyUnits)
        {
            float distance = position.DistanceTo(unit.GlobalPosition);

            if (distance < closestDistance && distance < _detectionRadius)
            {
                closestDistance = distance;
                closestUnit = unit;
            }
        }

        // If there is no unit close enough, there is no repulsion
        if (closestUnit == null) return Vector2.Zero;

        // Calculate vector in the opposite direction to the closest unit
        Vector2 avoidanceDirection = (position - closestUnit.GlobalPosition).Normalized();

        // Multiply by the repulsion strength
        return avoidanceDirection * _repulsionStrength;
    }

    public void SetRepulsionStrength(float strength)
    {
        _repulsionStrength = strength;
    }

    public void SetDetectionRadius(float radius)
    {
        _detectionRadius = radius;
    }
}