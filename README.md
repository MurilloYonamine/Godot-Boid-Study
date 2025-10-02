# Godot Boids Study

A Godot 4.x project implementing efficient boid behavior simulation in C# to replace expensive collision detection for many entities.  

## Problem
Traditional collision detection between many units is expensive. Using collision bodies, the game struggled with 60–80 monsters at 30 FPS.

## Solution
Boids are autonomous agents that follow simple rules to avoid each other without costly collisions.  
This implementation:
- Uses `Area2D` to detect nearby units
- Updates avoidance every 0.2 seconds
- Calculates a repulsion vector from the closest unit
- Applies configurable repulsion strength

## Project Structure
```
Assets/
├── Scripts/
│   ├── Unit.cs
│   ├── UnitMovement.cs
│   ├── UnitSpawner.cs
│   └── BoidBehavior.cs
├── Scenes/
│   └── MainScene.tscn
├── Nodes/
│   └── Unit/
│       └── Unit.tscn
└── Sprites/
    └── Fishes/
```

## Scripts Overview

- **Unit.cs** – Main unit controller; handles movement, detection, and sprite setup.  
- **BoidBehavior.cs** – Calculates repulsion vectors from nearby units.  
- **UnitMovement.cs** – Handles auto-movement, navigation, and boundary reflection.  
- **UnitSpawner.cs** – Creates units at random positions inside the spawn area.  
- **SpriteManager.cs** – Loads and provides random sprites.  

## Controls
- **Left Click:** Move unit to mouse position  
- **G Key:** Spawn enemy unit moving toward center  

## Preview
![Boids Behavior Demo](Media/boids-demo.gif)

## Credits

**Boid Algorithm:**  
Based on the boid optimization technique from [this Reddit post](https://www.reddit.com/r/godot/comments/1l096sw/collision_was_too_expensive_heres_what_i_did/) by u/ShnenyDev.

**Art Assets:**  
Fish sprites from [Kenney's Fish Pack 2.0](https://www.kenney.nl) - Licensed under Creative Commons Zero (CC0).  
Support Kenney at [www.kenney.nl](https://www.kenney.nl) or [patreon.com/kenney](https://patreon.com/kenney).