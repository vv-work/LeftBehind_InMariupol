# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D game project titled "Left Behind In Mariupol" - likely a narrative-driven game set in the Ukrainian city of Mariupol. The project uses Unity 6000.2.0f1 with Universal Render Pipeline (URP) for cross-platform rendering.

## Unity Project Architecture

### Core Technologies
- **Unity Version**: 6000.2.0f1 (Unity 6 LTS)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.2.0
- **Input System**: Unity Input System 1.14.1 with custom InputActions
- **Navigation**: AI Navigation 2.0.8 for pathfinding and AI movement
- **Visual Scripting**: Unity Visual Scripting 1.9.7 enabled

### Project Structure
- **Assets/Scenes/**: Contains game scenes (currently SampleScene.unity)
- **Assets/Settings/**: URP configuration files, rendering profiles for PC and Mobile
- **Assets/InputSystem_Actions.inputactions**: Comprehensive input mapping for Player and UI actions
- **ProjectSettings/**: Unity project configuration files
- **Packages/**: Unity Package Manager dependencies

### Input System Configuration
The project uses a sophisticated input system with two main action maps:
- **Player Actions**: Move, Look, Attack, Interact (with Hold interaction), Crouch, Jump, Sprint, Previous/Next navigation
- **UI Actions**: Standard UI navigation, pointer interactions, scroll wheel support

Input supports multiple control schemes:
- Keyboard & Mouse
- Gamepad (Xbox/PlayStation controllers)
- Touch (mobile devices)
- XR Controllers
- Generic Joystick

### Rendering Setup
The project is configured for both PC and mobile platforms with separate URP assets:
- **PC**: Higher quality rendering settings
- **Mobile**: Optimized for mobile performance
- Volume profiles for post-processing effects

## Development Workflow

### Building the Project
Unity projects are built through the Unity Editor interface:
1. Open the project in Unity Editor 6000.2.0f1
2. Use File → Build Settings to configure target platform
3. Select scenes to include (currently only SampleScene)
4. Build via Build or Build and Run

### Code Development
- The project currently has no custom C# scripts in the Assets folder
- C# scripts would be compiled automatically by Unity
- Use the generated .sln file to work with Visual Studio or Rider

### Common Unity Commands
- **Open Project**: Launch Unity Hub and open the project folder
- **Play Mode**: Use Unity Editor's play button to test gameplay
- **Build**: File → Build Settings → Build (or Ctrl+Shift+B)
- **Package Manager**: Window → Package Manager to manage dependencies

### Testing
Unity projects use Unity Test Framework:
- **Play Mode Tests**: Runtime testing of game logic
- **Edit Mode Tests**: Editor-time testing of utilities and systems
- Access via Window → General → Test Runner

## Key Dependencies
The project includes these essential Unity packages:
- `com.unity.render-pipelines.universal`: URP rendering system
- `com.unity.inputsystem`: Modern input handling
- `com.unity.ai.navigation`: NavMesh and AI pathfinding
- `com.unity.timeline`: Cutscene and animation sequencing
- `com.unity.visualscripting`: Node-based scripting system
- `com.unity.textmeshpro`: Advanced text rendering

## Development Notes
- This appears to be a narrative game project about Mariupol, likely dealing with serious historical/political themes
- The comprehensive input system suggests the game will support multiple platforms and input methods
- URP setup indicates the project is designed for modern graphics capabilities while maintaining mobile compatibility
- The project is in early stages with minimal custom content beyond Unity template setup

## Platform Targeting
The URP configuration suggests multi-platform development:
- PC/Console: Full feature set with higher quality rendering
- Mobile: Optimized rendering pipeline for performance
- Cross-platform input system ready for all supported Unity platforms