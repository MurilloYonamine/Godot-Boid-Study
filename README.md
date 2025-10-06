# Godot Boids Study
A Godot 4.x project implementing efficient boid behavior simulation following the approach discussed in [this Reddit post](https://www.reddit.com/r/godot/comments/1l096sw/collision_was_too_expensive_heres_what_i_did/)
about replacing expensive collision systems with boid avoidance algorithms.

## The Problem & Solution
**The Problem:**
Traditional collision detection between many entities becomes extremely expensive as the number
of objects increases. The original approach using collision bodies was barely managing 60-80
monsters at 30 FPS.

**The Solution - Boids:**
Boids (bird-oids) are autonomous agents that follow simple rules to create emergent flocking
behavior without expensive collision calculations.

### What are Boids?
Boids are artificial life programs that simulate the flocking behavior of birds. The classic boid algorithm follows three simple rules:
1. **Separation** - Avoid crowding neighbors (steer to avoid)
2. **Alignment** - Steer towards average heading of neighbors  
3. **Cohesion** - Steer towards average position of neighbors

### How This Project Uses Boids
This project implements a simplified boid system focused primarily on **separation** for collision avoidance:
- Uses Area2D to detect nearby units and add them to an array
- Calculates the closest unit every 0.2 seconds for performance
- Returns a repulsion vector in the opposite direction from the nearest unit
- Applies mass-based influence (heavier units cause stronger avoidance)
- Multiplies the vector by configurable repulsion strength

## Scripts Overview (Quick Summary)

- **Unit.cs** – Main unit controller; handles movement, detection, and sprite setup
- **BoidBehavior.cs** – Calculates repulsion vectors from nearby units with mass influence
- **UnitMovement.cs** – Handles auto-movement, navigation, and boundary reflection
- **UnitSpawner.cs** – Creates units at random positions inside the spawn area
- **DebugUnitDrawer.cs** – Visual debugging system for boid behavior
- **SpriteManager.cs** – Loads and provides random fish sprites

## Detailed Script Documentation

### Unit.cs - Main Unit Controller
The core component that orchestrates all unit behavior:
- **Movement Integration**: Combines boid avoidance with navigation
- **Detection System**: Uses Area2D to find nearby units for avoidance calculations
- **Timing Control**: Updates avoidance calculations every 0.2 seconds for performance
- **Mass System**: Each unit has configurable mass affecting avoidance strength

### BoidBehavior.cs - Avoidance Algorithm
Implements the core boid separation behavior:
- **Distance-Based Calculation**: Stronger repulsion for closer units
- **Mass Influence**: Heavier units cause stronger avoidance response
- **Configurable Parameters**: Adjustable repulsion strength and detection radius
- **Performance Optimized**: Only calculates avoidance for the closest unit

### UnitMovement.cs - Movement System
Handles all unit locomotion and navigation:
- **Auto Movement**: Units move in random directions with boundary reflection
- **Navigation Integration**: Uses Godot's NavigationAgent2D for pathfinding
- **Click-to-Move**: Players can direct units by clicking
- **Boundary Handling**: Units bounce off screen edges naturally

### UnitSpawner.cs - Unit Generation
Manages creation and placement of units:
- **Safe Spawning**: Ensures units spawn within defined areas with margins
- **Multiple Unit Types**: Supports different unit variants (Light, Normal, Heavy)
- **Keyboard Shortcuts**: Quick spawn controls for testing different unit types
- **Dynamic Creation**: Runtime unit generation for interactive testing

### DebugUnitDrawer.cs - Visual Debugging
Provides visual feedback for boid behavior development:
- **Avoidance Radius**: Shows detection area around each unit
- **Connection Lines**: Draws lines to nearby units being avoided
- **Toggle Control**: Press 'D' to enable/disable debug visualization
- **Performance Aware**: Only draws when debug mode is active

### SpriteManager.cs - Asset Management
Handles sprite loading and assignment:
- **Dynamic Loading**: Loads fish sprites from the Assets directory
- **Random Assignment**: Gives each unit a unique random appearance
- **Resource Management**: Efficiently handles sprite textures

## Controls

### Basic Controls
- **Left Click:** Move selected unit to mouse position
- **G Key:** Spawn enemy unit that moves toward center

### Unit Spawning
- **1 Key:** Spawn Light Unit (low mass)
- **2 Key:** Spawn Normal Unit (medium mass)  
- **3 Key:** Spawn Heavy Unit (high mass)

### Debug Controls
- **D Key:** Toggle debug visualization (shows avoidance radius and connections)

## Preview
<div align="center">
    <img src="Media/boids-demo.gif" alt="Boids Behavior Demo" width="100%">
</div>

## Credits

**Boid Algorithm:**  
Based on the boid optimization technique from [this Reddit post](https://www.reddit.com/r/godot/comments/1l096sw/collision_was_too_expensive_heres_what_i_did/) by u/ShnenyDev.

**Art Assets:**  
Fish sprites from [Kenney's Fish Pack 2.0](https://www.kenney.nl) - Licensed under Creative Commons Zero (CC0).  
Support Kenney at [www.kenney.nl](https://www.kenney.nl) or [patreon.com/kenney](https://patreon.com/kenney).