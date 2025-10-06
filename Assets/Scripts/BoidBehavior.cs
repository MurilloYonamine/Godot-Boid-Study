using Godot;
using System.Collections.Generic;

public class BoidBehavior
{
    // === BEHAVIOR PARAMETERS ===
    private float _repulsionStrength;
    private float _detectionRadius;

    public BoidBehavior(float repulsionStrength = 1.0f, float detectionRadius = 60f)
    {
        _repulsionStrength = repulsionStrength;
        _detectionRadius = detectionRadius;
    }

    public Vector2 CalculateAvoidanceVector(Vector2 position, Unit self, List<Unit> nearbyUnits)
    {
        if (nearbyUnits.Count == 0) return Vector2.Zero;

        Unit closestUnit = FindClosestUnit(position, nearbyUnits);
        if (closestUnit == null) return Vector2.Zero;

        return CalculateAvoidanceForce(position, self, closestUnit);
    }

    private Unit FindClosestUnit(Vector2 position, List<Unit> nearbyUnits)
    {
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

        return closestUnit;
    }

    private Vector2 CalculateAvoidanceForce(Vector2 position, Unit self, Unit closestUnit)
    {
        Vector2 avoidanceDirection = (position - closestUnit.GlobalPosition).Normalized();
        
        float massInfluence = CalculateMassInfluence(self.Mass, closestUnit.Mass);
        float distanceFactor = CalculateDistanceFactor(position, closestUnit.GlobalPosition);
        float finalStrength = _repulsionStrength * massInfluence * distanceFactor;

        return avoidanceDirection * finalStrength;
    }

    private float CalculateDistanceFactor(Vector2 position, Vector2 targetPosition)
    {
        float distance = position.DistanceTo(targetPosition);
        float distanceFactor = 1.0f - (distance / _detectionRadius);
        return Mathf.Pow(distanceFactor, 0.5f);
    }

    private float CalculateMassInfluence(float selfMass, float otherMass)
    {
        float massRatio = otherMass / selfMass;
        float influence = 0;

        if (massRatio > 1.0f)
        {
            // Heavier units cause stronger avoidance response
            influence = Mathf.Pow(massRatio, 1.5f);
            return Mathf.Clamp(influence, 1.0f, 5.0f);
        }

        // Lighter units cause weaker avoidance response
        influence = Mathf.Pow(massRatio, 0.7f);
        return Mathf.Clamp(influence, 0.1f, 1.0f);
    }

}