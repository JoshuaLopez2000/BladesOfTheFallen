# Clean runtime architecture

## Design goals

The runtime uses an inward dependency direction: engine-independent rules know nothing about Unity, while Unity components adapt scenes, rendering, persistence, and timing to those rules. Third-party packages remain outside the project-owned assemblies.

## Dependency direction

```text
Unity scenes and UI
        |
        v
BladesOfTheFallen.Runtime  (presentation + infrastructure adapters)
        |
        v
BladesOfTheFallen.Core     (entities, policies, use cases)
```

The core assembly has `noEngineReferences` enabled. Code added there must remain deterministic and must not reference `UnityEngine`, scene objects, files, or platform APIs.

## State and events

`GameSession` is the single owner of mutable run state. It implements scoring, lives, game-over transitions, kill counts, spawn cadence, and reset behavior without depending on Unity.

`GameManagerSO` is the Unity-facing facade and configuration asset. It preserves inspector and prefab integration, exposes read-only state, and forwards intent methods such as `IncreaseScore`, `DecreaseLife`, and `RegisterEnemyKilled` to `GameSession`. Consumers cannot directly overwrite runtime state.

The asset publishes four event streams:

- `OnScoreChanged` updates score and high-score systems.
- `OnPlayerLivesChanged` updates life icons and pushes enemies away after damage.
- `OnEnemiesKilledChanged` advances spawn difficulty.
- `OnGameOver` opens the end menu and persists a new high score.

`GameManagerSO` publishes requests for pause, resume, hit stop, and restart. `GameManagerMono` handles those requests because coroutines, `Time.timeScale`, and scene loading are Unity infrastructure concerns. It serializes time transitions so only one coroutine controls `Time.timeScale` at a time.

## Input and combat

`InputReaderSO` is an input event channel. `PlayerController` can receive those events or fall back to public UI entry points used by serialized UnityEvents.

The player performs a directional raycast, resolves an `EnemyBase`, and delegates damage to the enemy. Combo state and animation selection remain player concerns. Shader state uses a cached `MaterialPropertyBlock` and updates only when the special-ability state changes.

## Enemies and spawning

`EnemyBase` owns shared initialization, player targeting, movement, spacing, hit recovery, visual properties, and death registration. `BasicEnemyController` and `MediumEnemyController` provide their animation and damage-specific behavior.

`EnemyDifficultyPolicy` deterministically maps kill count and a random roll to a spawn profile. `EnemySpawner` owns only Unity-specific random sampling, positions, and prefab creation. Spawn cadence comes from the core session and is clamped to a safe minimum.

## UI and persistence

UI listeners subscribe and unsubscribe with the Unity object lifecycle. Button listeners registered in code are removed on destruction.

`HighScoreService` owns the record comparison use case and depends on `IHighScoreRepository`. `JsonHighScoreRepository` is the Unity/file-system adapter stored under `Application.persistentDataPath`; malformed or inaccessible data falls back to a safe default. Audio settings use the same defensive storage behavior and are a candidate for the same repository boundary as that feature grows.

## World streaming

`TerrainManager` maps the player's x-position to a terrain-tile index. It ensures a configurable radius of tiles exists around that index and destroys tiles that leave the window, keeping object count bounded during long sessions.

## Assembly boundaries

- `BladesOfTheFallen.Core`: engine-independent session, difficulty, and high-score rules.
- `BladesOfTheFallen.Runtime`: Unity presentation, orchestration, and infrastructure adapters.
- `BladesOfTheFallen.EditModeTests`: editor-only NUnit coverage for core and runtime behavior.
- Vendor code: compiled by its own vendor assemblies or Unity's default assemblies and left unmodified.

## Extension rules

- Put deterministic gameplay rules and use cases in `Core`; depend on interfaces for external storage or services.
- Put Unity, file-system, scene, rendering, and platform implementations in `Runtime` or `Infrastructure`.
- Keep MonoBehaviours thin: translate Unity callbacks into core intents and render the resulting state.
- Add core tests without scene setup; reserve Play Mode tests for physics, animation, serialization, and prefab integration.
