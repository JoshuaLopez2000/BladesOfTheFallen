# Runtime architecture

## Design goals

The runtime favors small scene components, explicit inspector dependencies, event-driven UI updates, and a single owner for mutable game state. Third-party packages are kept outside the project-owned runtime assembly.

## State and events

`GameManagerSO` is the shared gameplay state asset. It exposes read-only properties and intent-based methods such as `IncreaseScore`, `DecreaseLife`, and `RegisterEnemyKilled`. Consumers cannot directly overwrite runtime state.

The asset publishes four event streams:

- `OnScoreChanged` updates score and high-score systems.
- `OnPlayerLivesChanged` updates life icons and pushes enemies away after damage.
- `OnEnemiesKilledChanged` advances spawn difficulty.
- `OnGameOver` opens the end menu and persists a new high score.

`GameManagerMono` owns operations that require a scene object or coroutine. In particular, it serializes pause, resume, and hit-stop transitions so only one coroutine controls `Time.timeScale` at a time.

## Input and combat

`InputReaderSO` is an input event channel. `PlayerController` can receive those events or fall back to public UI entry points used by serialized UnityEvents.

The player performs a directional raycast, resolves an `EnemyBase`, and delegates damage to the enemy. Combo state and animation selection remain player concerns. Shader state uses a cached `MaterialPropertyBlock` and updates only when the special-ability state changes.

## Enemies and spawning

`EnemyBase` owns shared initialization, player targeting, movement, spacing, hit recovery, visual properties, and death registration. `BasicEnemyController` and `MediumEnemyController` provide their animation and damage-specific behavior.

`EnemySpawner` chooses side, type, speed, and health from the current kill count. Spawn cadence comes from `GameManagerSO` and is clamped to a safe minimum.

## UI and persistence

UI listeners subscribe and unsubscribe with the Unity object lifecycle. Button listeners registered in code are removed on destruction. High-score and audio settings are stored under `Application.persistentDataPath`; malformed or inaccessible files fall back to safe defaults.

## World streaming

`TerrainManager` maps the player's x-position to a terrain-tile index. It ensures a configurable radius of tiles exists around that index and destroys tiles that leave the window, keeping object count bounded during long sessions.

## Assembly boundaries

- `BladesOfTheFallen.Runtime`: project-owned runtime components and ScriptableObjects.
- `BladesOfTheFallen.EditModeTests`: editor-only NUnit coverage for deterministic state and utility behavior.
- Vendor code: compiled by its own vendor assemblies or Unity's default assemblies and left unmodified.
