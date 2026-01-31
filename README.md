# Blades of the Fallen

> A stylish, high-octane action game featuring a unique Chinese ink painting aesthetic. Master the blade, parry your foes, and survive the endless waves of the fallen.

![Game Banner](https://github.com/user-attachments/assets/f3ef9698-79a1-4042-8272-bc433e92e0ac) 

## Overview

**Blades of the Fallen** is an endless action fighter built in **Unity**. Players control a lone warrior defending against encroaching enemies from multiple sides. The game emphasizes rhythm, reaction speed, and precision, wrapped in a visually striking "Ink Slash" rendering style.

## Key Features

*   **Dynamic Combat System:**
    *   **Directional Attacks:** Slash enemies approaching from the **Left** or **Right**.
    *   **Parry Mechanic:** Deflect incoming attacks with precise timing to maintain your flow.
    *   **Combo System:** Build up your combo meter with consecutive hits; missing or getting hit resets your momentum.
*   **Distinct Enemy Types:**
    *   **Basic Enemies:** Relentless attackers that swarm the player.
    *   **Medium Enemies:** Agile foes that **teleport** behind the player after taking damage, requiring quick reflexes to finish off.
    *   **Visual Feedback:** Enemies change color (Purple -> Yellow -> Red) to indicate health states using optimized shader properties.
*   **Game Feel:**
    *   **Hit Stop / Time Freeze:** Impact frames slow down time (`HitTimeEffect`) to accentuate powerful strikes.
    *   **Camera Shake & Pushback:** Visually responsive combat feedback.
*   **Stylized Graphics:** A custom "Chinese Ink Painting" rendering pipeline that unifies character models, effects, and the environment.

## Technical Highlights (Architecture)

This project demonstrates clean, scalable Unity architecture suitable for professional development.

### 1. ScriptableObject-Driven Architecture
The game uses `GameManagerSO` as a central, data-driven hub for state management. This decouples logic from the scene hierarchy.
*   **Observer Pattern:** Systems like `EnemyBase` subscribe to events (`OnPlayerLivesChanged`) to react dynamically (e.g., enemies backing off when the player takes damage).
*   **State Management:** Lives, Score, and Game State are managed centrally, allowing for easy testing and persistence.

### 2. Optimized Rendering
Instead of instantiating new Materials for every color change (which breaks batching), the project utilizes **`MaterialPropertyBlock`**.
*   **Implementation:** `EnemyBase` and `PlayerController` modify shader properties (like `_Color` and `_Switch`) on the fly without memory overhead, ensuring high performance even with many active entities.

### 3. Scalable Enemy System
*   **Polymorphism:** An abstract `EnemyBase` class handles common logic (movement, taking damage, event subscription), while derived classes like `MediumEnemyController` implement specific behaviors (teleportation, animation overrides).

## Controls

The game is designed for mobile and is played using **gestures**:

| Action | Gesture | Description |
| :--- | :--- | :--- |
| **Attack Right** | `Swipe Right` | Slash enemies on the right. |
| **Attack Left** | `Swipe Left` | Slash enemies on the left. |
| **Parry** | `Swipe Up` | Block incoming attacks. |

## Getting Started

1.  **Prerequisites:**
    *   Unity 6 (6000.0.66f1) or later.
    *   URP (Universal Render Pipeline) support.
2.  **Installation:**
    *   Clone the repository.
    *   Open the project in Unity Hub.
    *   Open the main scene located in `Assets/Project/Scenes/`.
3.  **Play:**
    *   Press the Play button in the Editor to start the endless survival mode.

## Project Structure

*   `Assets/Project/Scripts`: Core gameplay logic (Player, Enemies, Managers).
*   `Assets/Project/Scriptable Objects`: Configuration and State assets.
*   `Assets/Chinese Ink Painting Rendering`: Shaders and assets for the visual style.

## Credits

*   **Development:** EEJANAI Team
*   **Assets:**
    *   *Chinese Ink Painting Rendering*
    *   *FX_Ink slash(URP)*
    *   *FreeSwordAnimations*

---
*This project is part of my portfolio demonstrating proficiency in Unity C#, Game Architecture, and Shader interaction.*
