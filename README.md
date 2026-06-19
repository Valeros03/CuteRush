# CuteRush Kit - Game Features and Architecture

![Gameplay Screenshot Placeholder](path/to/gameplay_screenshot.png)

## General Settings
- **Mobile Build**: The game supports mobile input. References: `Assets/CuteRush kit/Scripts/Player/PlayerInput.cs` and the Input System mapping in the Unity packages.
- **Splash Screen**: Set in the global project properties `ProjectSettings/ProjectSettings.asset` (line 23: `m_ShowUnitySplashScreen: 1`, `m_SplashScreenBackgroundColor`).
- **Game Icon**: Configured in the Player Settings within the Unity project.

## Main Menu

![Main Menu Placeholder](path/to/main_menu_screenshot.png)

- **Load Game**: Managed in `Assets/CuteRush kit/Scripts/SaveSystem/MainMenuSaveIntegration.cs` (line 51: `ShowLoadGamePanel()`, line 94: `HandleLoadGame()`).
- **Options (Sound/Music)**: Sliders managed in `Assets/CuteRush kit/UI/Scripts/MainMenu.cs`, interfacing with `Assets/CuteRush kit/Audio/Script/AudioManager.cs` (e.g., `SetMasterVolume()`, `SetMusicVolume()`).
- **Leaderboard (Ordered with dynamic replacement)**:
  - `Assets/CuteRush kit/Scripts/SaveSystem/GlobalLeaderboardData.cs` (line 23: `topScores`)
  - `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (line 76: `SubmitScore`, limits local and global leaderboards with dynamic replacement logic).

## Gameplay

- **Score**: Incremented in `Assets/CuteRush kit/Scripts/GameManager.cs` (line 84: `AddKillScore(int points)`) and called by `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (line 437: `GameManager.Instance.AddKillScore(killPoints)`).
- **Power-Ups, Shop, and Player Bonuses**:
  - Upgrades managed in `Assets/CuteRush kit/Scripts/Upgrades/UpgradeManager.cs`.
  - Health drops: `Assets/CuteRush kit/Scripts/Consumabili/Medkit.cs`.
  - Ammo drops: `Assets/CuteRush kit/Scripts/Consumabili/Mags.cs`.
  - Weapon upgrades: `Assets/CuteRush kit/Scripts/Player/PlayerUpgrades.cs`.

  ![Shop UI Placeholder](path/to/shop_ui_screenshot.png)

- **Time-Based Gameplay**: Managed by `Assets/CuteRush kit/Scripts/GameManager.cs` (`TimerRoutine` coroutine at line 119).
- **Enemy Presence and Challenge**: Base enemy logic in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` alongside various spawners in `EnemySpawner.cs`.
- **Difficulty Levels**: Defined in `Assets/CuteRush kit/Scripts/Difficulty/DifficultyManager.cs` and `DifficultyProfile.cs`. Selected via `DifficultySetter.cs`.
- **Increasing Difficulty**: The difficulty scales damage and health, as seen in `Assets/CuteRush kit/Scripts/Enemy/RangedEnemy.cs` (line 44: `scaledRangedDamage = Mathf.RoundToInt(rangedAttackDamage * diffMult)`).
- **Complex AI**: The AI analyzes the player's movements to anticipate and aim ahead.
  - File: `Assets/CuteRush kit/Scripts/Enemy/RangedEnemy.cs` (line 54: `PerformChaseLogic()` -> line 61: `Vector3 predictedTarget = GetPredictedPlayerPosition(...)`).
  - Key Function: `GetPredictedPlayerPosition` located in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (line 483), which performs a `GetVelocityOverWindow` analysis to shoot ahead of the player's movement.

## Tutorial
- Features 3 static screens viewable from the main menu (`MainMenuLoadSave`).

![Tutorial Screens Placeholder](path/to/tutorial_screenshot.png)

## Architecture and Structure
- **PlayerPrefs**: Utilized across various scripts for settings and local state persistence (e.g., SaveManager, DifficultySetter, PlayerUpgrades, InventoryPlayer).
- **Singletons**: Implemented for core managers like `SaveManager`, `UIManager`, `GameManager`, `DifficultyManager`, `Bootstrapper`, and `UpgradeManager`.
- **Coroutines**: Widely used for timed events, spawning, and asynchronous tasks across HUD, enemies, spawners, weapons, and managers.
- **Enums**: Define states and types such as `AIState`, `motionstate`, `GameState`, `FireMode`, `Handedness`, and `preset` (Crosshair).
- **Static Classes**: Centralized events and constants in `UIEvents.cs`, `GameConstants.cs`, and `GameExtension.cs`.
- **Generics**: Used for utility functions like `GetRandomWeightedItem<T>` and abstract classes like `PickableItem<T>`.
- **Inheritance**: Extensive use of base classes. For example, `RangedEnemy`, `MeleeEnemy`, and `HybridEnemy` inherit from `Enemy`. Weapon types like `ChargeGun` and `AutoGun` inherit from `GunBase`.
- **Overriding**: Subclasses frequently override base methods (e.g., `Start`, `PerformChaseLogic`, `Shoot`, `Interact`, `ApplyEffect`).
- **Interfaces**: Implements interfaces like `IPickable`, `Interactable`, and input actions (`IPlayerActions`).
- **Extension Methods**: Custom extensions for vectors and navigation (e.g., `WithY`, `HasReachedDestination`).
- **Delegates and Events**: Event-driven architecture utilizing delegates for health changes (`HealthChanger`, `DamageTaker`) and events for input (`OnJump`, `OnReload`) and game states (`OnGameOver`).
- **Raycasts**: Used for ground detection, line-of-sight checks, and hitscan weapons.
- **User Interface**: Comprehensive UI scripts for HUD, menus, damage indicators, and upgrade panels located in `Assets/CuteRush kit/UI/Scripts/`.

## Extra Features
- **Particles**:
  - Enemies (blood/hit effects): `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs`
  - Bonfires (World/Environment): `Assets/CuteRush kit/Scripts/World/Bonfire.cs`
  - Weapons (Muzzle flash/shells): `Assets/CuteRush kit/Scripts/AbstractClass/GunBase.cs`
- **Original Animations**: Included within the project assets.
- **Sound**: Managed by `AudioPlayerController.cs` and `AudioGunController.cs` for shooting, reloading, footsteps, damage, and item pickups.

## Technical Notes
- **New Input System (Unity Package)**:
  - References: `Assets/CuteRush kit/Scripts/Player/PlayerInput.cs`, `Assets/PlayerControl.cs`.
  - Action-Based (Event-Driven): Instead of polling keyboard input in `Update()`, it relies on events (e.g., `OnJump`, `OnMove`). This is cleaner, more responsive, and CPU-efficient.
  - Native Cross-Platform: Abstracts hardware completely. A "Move" action works automatically with WASD keys or a gamepad analog stick without separate code.
- **Addressable Asset System**:
  - Asynchronous Memory Management: Addressables load assets (e.g., enemy prefabs, heavy weapons, UI menus) asynchronously only when needed, releasing them from RAM when no longer used. This avoids the RAM spikes associated with direct Inspector references.
  - Build Optimization: Avoids the bottleneck of the "Resources" folder. Assets in "Resources" are indexed by Unity, increasing load times and base build size. Addressables decouple assets from the build base.
  - Decoupling and Flexibility: The code calls a text address instead of a physical path. Assets can be moved, grouped, or downloaded from a remote server without recompiling C# code.