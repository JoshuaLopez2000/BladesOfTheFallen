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

- **Engine-independent game core:** session state, difficulty progression, and high-score rules live in `BladesOfTheFallen.Core`, which has no Unity dependency.
- **Adapter-based runtime:** `GameManagerSO`, spawners, UI, scene loading, time scaling, and JSON storage adapt Unity APIs to the core instead of owning business rules.
- **Explicit assembly boundaries:** core rules, Unity runtime code, and Edit Mode tests compile separately, preventing accidental engine coupling in the domain layer.
- **Allocation-conscious rendering:** enemy and player shader overrides reuse `MaterialPropertyBlock` instances instead of cloning materials or allocating every frame.
- **Shared enemy behavior:** `EnemyBase` centralizes targeting, movement, spacing, hit recovery, score registration, and shader updates. Enemy subclasses contain only type-specific behavior.
- **Explicit combat phases:** attacks and parries progress through startup, active, and recovery windows; hit stun and post-hit invulnerability prevent overlapping enemy attacks from draining multiple lives.
- **Cancel-friendly slashes:** new directional slashes restart pending attack animations, successful hits have no cooldown, and failed slashes retain a non-cancellable recovery penalty.
- **Timing-based parries:** a parry counters only an active enemy attack, stunning and pushing the attacker instead of acting like another slash.
- **Readable counter cue:** enemies blink cyan during the parryable portion of their attack animation while preserving their current health color.
- **Bounded world streaming:** `TerrainManager` keeps a small window of terrain tiles around the player and destroys tiles outside that window.
- **Defensive persistence:** high-score and audio-setting reads and writes fail gracefully when storage is unavailable or data is invalid.
- **Time-scale ownership:** pause, resume, and hit-stop transitions are coordinated by one runtime component so competing coroutines cannot fight over `Time.timeScale`.

See [Docs/ARCHITECTURE.md](Docs/ARCHITECTURE.md) for the runtime responsibilities and event flow.

## Project structure

```text
Assets/Project/
├── Prefabs/                 Game-owned prefabs and UI
├── Scenes/                  MainScreen, Level1, and development scenes
├── Scriptable Objects/      Runtime configuration assets
├── Scripts/
│   ├── Core/                Engine-independent rules assembly
│   ├── Infrastructure/      Persistence and other Unity-side adapters
│   └── *.cs                 Scene-facing runtime components
└── Tests/EditMode/          Fast core, state, and utility tests
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

The project includes Edit Mode coverage for session resets, score events, game-over transitions, difficulty tiers, high-score registration, spawn-interval clamping, and list shuffling. Run it from **Window > General > Test Runner > EditMode**.

The main menu and gameplay scenes are also suitable for a short smoke test: launch from `MainScreen`, enter `Level1`, attack in both directions, parry, pause/resume, restart, and return to the menu.

## Credits

- Development: EEJANAI Team
- Chinese Ink Painting Rendering assets
- FX Ink Slash (URP)
- Free Sword Animations
- Lean Touch / CW Common

Vendor documentation and notices are retained beside the imported assets where provided.
