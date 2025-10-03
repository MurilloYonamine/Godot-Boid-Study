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

    public Vector2 CalculateAvoidanceVector(Vector2 position, Unit self, List<Unit> nearbyUnits)
    {
        if (nearbyUnits.Count == 0) return Vector2.Zero;

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

        if (closestUnit == null) return Vector2.Zero;

        // Direção oposta
        Vector2 avoidanceDirection = (position - closestUnit.GlobalPosition).Normalized();

        // Cálculo de força baseado na diferença de massa
        float massInfluence = CalculateMassInfluence(self.Mass, closestUnit.Mass);

        // Aplicar falloff baseado na distância para movimento mais suave
        float distanceFactor = 1.0f - (closestDistance / _detectionRadius);
        distanceFactor = Mathf.Pow(distanceFactor, 0.5f); // Suavizar a curva

        float finalStrength = _repulsionStrength * massInfluence * distanceFactor;

        return avoidanceDirection * finalStrength;
    }

    private float CalculateMassInfluence(float selfMass, float otherMass)
    {
        float massRatio = otherMass / selfMass;
        float influence = 0;
        if (massRatio > 1.0f)
        {
            influence = Mathf.Pow(massRatio, 1.5f);
            return Mathf.Clamp(influence, 1.0f, 5.0f);
        }
        influence = Mathf.Pow(massRatio, 0.7f);
        return Mathf.Clamp(influence, 0.1f, 1.0f);
    }

}