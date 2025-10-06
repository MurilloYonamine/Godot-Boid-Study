using Godot;
using System.Collections.Generic;

public partial class DebugUnitDrawer : Node2D
{
    // === DEBUG VISUALIZATION CONTROL ===
    private static bool _globalShowDebugGizmos = false;

    // === COMPONENT REFERENCES ===
    private Unit _parentUnit;

    // === DEBUG DRAWING CONSTANTS ===
    private const int ARC_SEGMENTS = 32;
    private const float ARC_LINE_WIDTH = 2.0f;
    private const float CONNECTION_LINE_WIDTH = 3.0f;

    public override void _Ready()
    {
        _parentUnit = GetParent<Unit>();
        AddToGroup("debug_drawers");
    }

    public void queue_redraw() => QueueRedraw();

    public override void _Process(double delta)
    {
        if (_globalShowDebugGizmos)
        {
            QueueRedraw();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.D)
        {
            _globalShowDebugGizmos = !_globalShowDebugGizmos;
            GetTree().CallGroup("debug_drawers", "queue_redraw");
        }
    }

    public override void _Draw()
    {
        if (!_globalShowDebugGizmos || _parentUnit == null) return;

        DrawAvoidanceRadius();
        DrawNearbyUnitLines();
    }

    private void DrawAvoidanceRadius()
    {
        float radius = _parentUnit.AvoidanceRadius;
        DrawArc(Vector2.Zero, radius, 0, Mathf.Tau, ARC_SEGMENTS, Colors.Blue, ARC_LINE_WIDTH);
    }

    private void DrawNearbyUnitLines()
    {
        // Draw connection lines to detected nearby units
        List<Unit> nearbyUnits = _parentUnit.NearbyUnits;
        foreach (Unit unit in nearbyUnits)
        {
            if (IsInstanceValid(unit))
            {
                Vector2 startPos = Vector2.Zero;
                Vector2 endPos = unit.GlobalPosition - GlobalPosition;
                DrawLine(startPos, endPos, Colors.Red, CONNECTION_LINE_WIDTH);
            }
        }
    }
}