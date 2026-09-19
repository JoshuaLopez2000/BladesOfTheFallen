# Blades of the Fallen

> A mobile action game built around directional attacks, parries, escalating enemy waves, and a Chinese ink-painting visual style.

![Game banner](https://github.com/user-attachments/assets/f3ef9698-79a1-4042-8272-bc433e92e0ac)

## Overview

**Blades of the Fallen** is an endless action fighter made with Unity 6 and the Universal Render Pipeline. The player defends a lone warrior from enemies approaching on both sides. Short gesture inputs drive attacks and parries, while hit stop, camera feedback, combo text, and material effects reinforce combat timing.

## Gameplay

- Swipe left or right to attack in that direction.
- Swipe up to parry nearby enemies.
- Maintain a combo by landing consecutive attacks.
- Adapt to medium enemies that survive multiple hits and reposition behind the player.
- Survive increasingly frequent enemy spawns and pursue a persistent high score.

## Technical highlights

- **Event-driven state:** `GameManagerSO` owns the current score, lives, difficulty state, and gameplay events. UI and enemies subscribe only to the events they need.
- **Separated runtime assembly:** project-owned gameplay code is isolated in `BladesOfTheFallen.Runtime`; Edit Mode tests compile in a separate test assembly.
- **Allocation-conscious rendering:** enemy and player shader overrides reuse `MaterialPropertyBlock` instances instead of cloning materials or allocating every frame.
- **Shared enemy behavior:** `EnemyBase` centralizes targeting, movement, spacing, hit recovery, score registration, and shader updates. Enemy subclasses contain only type-specific behavior.
- **Bounded world streaming:** `TerrainManager` keeps a small window of terrain tiles around the player and destroys tiles outside that window.
- **Defensive persistence:** high-score and audio-setting reads and writes fail gracefully when storage is unavailable or data is invalid.
- **Time-scale ownership:** pause, resume, and hit-stop transitions are coordinated by one runtime component so competing coroutines cannot fight over `Time.timeScale`.

See [Docs/ARCHITECTURE.md](Docs/ARCHITECTURE.md) for the runtime responsibilities and event flow.

## Project structure

```text
Assets/Project/
├── Prefabs/                 Game-owned prefabs and UI
├── Scenes/                  MainScreen, Level1, and development scenes
├── Scriptable Objects/      Runtime configuration/state assets
├── Scripts/                 Production C# assembly
└── Tests/EditMode/          Fast state and utility tests
```

Third-party art, shaders, effects, and packages remain in their vendor folders under `Assets/`; project-authored gameplay code is kept under `Assets/Project`.

## Requirements

- Unity `6000.3.12f1` (the version recorded in `ProjectSettings/ProjectVersion.txt`)
- Universal Render Pipeline 17
- Unity Input System
- TextMesh Pro / Unity UI

Using the recorded editor version is recommended because opening the project in another Unity release can rewrite project and rendering settings.

## Run the project

1. Clone the repository.
2. Add the repository folder in Unity Hub.
3. Open it with Unity `6000.3.12f1`.
4. Open `Assets/Project/Scenes/MainScreen.unity`.
5. Enter Play Mode.

`MainScreen` and `Level1` are enabled in Build Settings. `Mechanics` is retained as a disabled development scene.

## Validation

The project includes Edit Mode coverage for score events, game-over transitions, spawn-interval clamping, and list shuffling. Run it from **Window > General > Test Runner > EditMode**.

The main menu and gameplay scenes are also suitable for a short smoke test: launch from `MainScreen`, enter `Level1`, attack in both directions, parry, pause/resume, restart, and return to the menu.

## Credits

- Development: EEJANAI Team
- Chinese Ink Painting Rendering assets
- FX Ink Slash (URP)
- Free Sword Animations
- Lean Touch / CW Common

Vendor documentation and notices are retained beside the imported assets where provided.
