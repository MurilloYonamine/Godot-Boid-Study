using Godot;
using System.Collections.Generic;

public partial class DebugUnitDrawer : Node2D
{
    // === DEBUG SETTINGS ===
    private static bool _globalShowDebugGizmos = false;

    // === REFERENCES ===
    private Unit _parentUnit;

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
        DrawArc(Vector2.Zero, radius, 0, Mathf.Tau, 32, Colors.Blue, 2.0f);
    }

    private void DrawNearbyUnitLines()
    {
        List<Unit> nearbyUnits = _parentUnit.NearbyUnits;
        foreach (Unit unit in nearbyUnits)
        {
            if (IsInstanceValid(unit))
            {
                Vector2 startPos = Vector2.Zero;
                Vector2 endPos = unit.GlobalPosition - GlobalPosition;
                DrawLine(startPos, endPos, Colors.Red, 3.0f);
            }
        }
    }
}