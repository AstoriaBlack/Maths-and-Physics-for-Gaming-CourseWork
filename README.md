# 5CCGD010W – Maths and Physics for Games: Coursework

![Task 1 - Red Ball Bounce](Assets/images/Task-1.png)

![Task 1 - Red Ball Bounce](Assets/images/Task-2.png)

![Task 1 - Red Ball Bounce](Assets/images/Task-3.png)


A Unity project built for the module **Maths and Physics for Gaming (5CCGD010W)**, demonstrating projectile motion, waypoint-based movement with easing, and a multi-system space game combining physics, tethering, and resource management.

- **Engine:** Unity 2022.3.45f1
- **Language:** C#
- **Modelling:** Blender (rocket and boulder assets, exported as FBX)

---

## Overview

The coursework is split into three tasks, each building on core physics and maths concepts covered in the module.

### Task 1 — Projectile Motion & Bounce (Red Ball)
`RedBallBehaviour.cs`

A ball is launched with an initial velocity and simulated under gravity using **semi-implicit (symplectic) Euler integration** — velocity is updated before position each frame. On hitting the ground, the vertical velocity is reversed and scaled by a **coefficient of restitution (e)**, while horizontal velocity is preserved (frictionless bounce). The ball comes to rest once the bounce speed falls below a threshold relative to the original launch speed.

### Task 2 — Waypoint Navigation & Easing (Pink Ball)
`PinkBallBehaviourScript.cs`

A ball moves between four waypoints in a loop, using **vector normalisation** to get direction and a **linear ease-in/ease-out** to slow down within a defined `slowRadius` of each target, giving smoother arrivals than constant-speed movement.

### Task 3 — Rocket, Boulders & Drop Zones (Space Game)
The most complex task, built from six interacting scripts:

| Script | Responsibility |
|---|---|
| `RocketController.cs` | Directional thrust via `Rigidbody.AddForce`, fuel drain on thrust, scene-specific gravity |
| `TetherController.cs` | Boulder pickup/detection within radius, attaching via `FixedJoint`, delivery logic |
| `TetherAnchorFollow.cs` | Keeps the tether anchor locked below the rocket in world space |
| `DropZoneController.cs` | Detects boulder delivery via trigger, prevents double-counting with a tracked list |
| `ObstacleController.cs` | Rotating + sine-wave oscillating obstacles that drain fuel on contact |
| `GameController.cs` | Fuel, scoring, win condition, and full game reset logic |

**Goal:** Pick up boulders (light/medium/heavy — each with a different fuel drain rate while carried) using the rocket's tether, avoid moving obstacles, and deliver all boulders to the drop zone before fuel runs out.

---

## Project Structure

```
Assets/
  Scripts/
    RedBallBehaviour.cs
    PinkBallBehaviourScript.cs
    RocketController.cs
    TetherController.cs
    TetherAnchorFollow.cs
    DropZoneController.cs
    ObstacleController.cs
    GameController.cs
Packages/
ProjectSettings/
UserSettings/
```

---

## Controls (Task 3)

| Key | Action |
|---|---|
| ↑ / ↓ / ← / → | Apply directional thrust to the rocket |
| E | Pick up nearest boulder within range |

---

## Key Physics & Maths Concepts

- **Semi-implicit Euler integration** for projectile motion (Task 1)
- **Coefficient of restitution** applied to vertical velocity only, for a frictionless bounce
- **Vector normalisation** for direction vectors (Task 2, Task 3)
- **Linear interpolation / easing** for smooth deceleration near waypoints
- **Newton's Second Law (F = ma)** via `Rigidbody.AddForce` for rocket thrust
- **Sine wave oscillation** for obstacle motion (`Mathf.Sin(Time.time * speed)`)

---

## Notable Implementation Decisions

- **Per-scene gravity** is set via script in each task's `Start()` to isolate Task 3's low "space" gravity from the standard gravity used in Tasks 1 and 2.
- **`FixedJoint` is placed on the boulder**, not the tether anchor, so that destroying the joint only affects the boulder and leaves the rocket unaffected.
- **Auto-delivery on trigger entry** (rather than a second keypress) simplifies the delivery flow; a `deliveredBoulders` list prevents the same boulder being counted twice if it re-enters the drop zone trigger.
- **`gameWon` boolean flag** is used instead of `Time.timeScale = 0` to pause game logic, avoiding a freeze-on-reset bug.

---

## Setup

1. Clone or download the repository.
2. Open the project folder in **Unity Hub** using **Unity 2022.3.45f1** (or later 2022.3 LTS).
3. Unity will regenerate the `Library` folder on first open — this may take a few minutes.
4. Open the relevant scene for each task and press Play.

---

## Author

Kithmini — coursework submission for 5CCGD010W, Maths and Physics for Gaming.
