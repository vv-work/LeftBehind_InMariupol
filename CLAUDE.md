# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

"Left Behind In Mariupol" is a Unity DOTS (Data-Oriented Technology Stack) RTS game about Ukrainian defenders during the battle for Mariupol. The project uses Unity ECS (Entity Component System) for high-performance simulation of hundreds of units.

## Core Architecture

### Unity DOTS/ECS Implementation
- **Unity Version**: 6000.2.0f1 (Unity 6 LTS)
- **ECS Version**: Unity Entities 1.3.14
- **Physics**: Unity Physics 1.3.14 for DOTS-based physics simulation
- **Graphics**: Unity Entities Graphics 1.4.12 for efficient rendering
- **Render Pipeline**: Universal Render Pipeline (URP) 17.2.0

### ECS Architecture Pattern
The project follows strict ECS architecture:

**Components (Data)**:
- `Unit`: Holds faction data for units
- `FindTargetData`: Range-based target acquisition
- `UnitMoverData`: Movement parameters and target positions
- `Selected`: Unit selection state with event flags

**Systems (Logic)**:
- `FindTargetSystem`: Handles target acquisition using Unity Physics CollisionWorld
- `UnitMoverSystem`: Manages unit movement with Burst-compiled parallel jobs
- `SelectVisualSystem`: Updates visual feedback for selected units
- `ResetEventsSystem`: Resets event flags each frame

**Authoring Components**:
- `UnitAuthoring`, `FindTargetAuthoring`, `UnitMoverAuthoring`: Convert GameObject data to ECS components during baking process

### Hybrid Architecture
The project uses a hybrid approach combining ECS and traditional MonoBehaviours:
- **ECS**: Core gameplay systems, unit simulation, movement, and combat
- **MonoBehaviours**: UI management, input handling, and Unity Editor integration
- **Baking System**: Converts authored GameObjects to ECS entities at build time

## Development Workflow

### Building and Running
- **Open Project**: Use Unity Hub to open with Unity 6000.2.0f1
- **Play Mode**: Press Play in Unity Editor (F5) - ECS systems will automatically start
- **Build**: File → Build Settings → Build (uses MainScene.unity)

### Code Organization
```
Assets/Scripts/
├── Authoring/          # ECS authoring components for GameObjects
├── Systems/            # ECS systems (game logic)
├── MonoBehaviours/     # Traditional Unity components
└── UI/                 # UI-specific components
```

### Testing ECS Systems
- Use Unity's ECS debugging tools: Window → Entities → Systems/Hierarchy
- Systems can be toggled on/off in the Editor for testing
- Burst Inspector: Jobs → Burst → Open Inspector for performance analysis

## Key ECS Concepts

### Entity Queries and Jobs
- Systems use `SystemAPI.Query<>()` for component access
- Parallel jobs (`IJobEntity`) are Burst-compiled for performance
- `EntityQueryBuilder` constructs filtered entity sets

### Selection System Implementation
The unit selection system demonstrates complex ECS/MonoBehaviour interaction:
- Mouse input handled in `UnitSelectionManager` (MonoBehaviour)
- Entity queries find and modify `Selected` components
- Physics raycasting uses Unity Physics CollisionWorld
- Supports both single-click and drag-rectangle selection

### Movement System Architecture
- Uses `IJobEntity` for parallel processing of unit movement
- Integrates with Unity Physics for collision-aware movement
- Formation generation for group movement orders
- Burst-compiled for optimal performance

## Physics Integration

### Unity Physics Setup
- CollisionWorld provides spatial queries for target finding
- PhysicsVelocity components drive unit movement
- Layer-based collision filtering (units on layer 7)
- Raycasting for mouse-based unit selection

## Critical Dependencies
- `com.unity.entities`: Core ECS framework
- `com.unity.physics`: DOTS physics simulation
- `com.unity.entities.graphics`: ECS rendering
- `com.unity.burst`: High-performance compilation
- `com.unity.inputsystem`: Modern input handling

## Performance Considerations
- All core systems are Burst-compiled
- Entity queries are cached and reused
- Parallel job scheduling with dependency management
- Physics queries use temporary NativeArrays for memory efficiency