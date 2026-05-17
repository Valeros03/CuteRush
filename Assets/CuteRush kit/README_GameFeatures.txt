Caratteristiche e Funzionalità in CuteRush kit
==============================================

Impostazioni generali
-Build mobile: Il gioco supporta l'input mobile. Riferimenti: `Assets/CuteRush kit/Scripts/Player/PlayerInput.cs` e la mappatura Input System nei package Unity.
-Splash Screen: Impostato nelle proprietà globali del progetto `ProjectSettings/ProjectSettings.asset` (riga 23: `m_ShowUnitySplashScreen: 1`, `m_SplashScreenBackgroundColor`).
-Icona gioco: Presente nelle impostazioni di Player Settings nel progetto Unity.


Main menu
-Load Game: Gestito in `Assets/CuteRush kit/Scripts/SaveSystem/MainMenuSaveIntegration.cs` (riga 51: `ShowLoadGamePanel()`, riga 94: `HandleLoadGame()`).
-Options - Sound/Music: Slider gestiti nel `Assets/CuteRush kit/UI/Scripts/MainMenu.cs` interfacciati con `Assets/CuteRush kit/Audio/Script/AudioManager.cs` (es. `SetMasterVolume()`, `SetMusicVolume()`).
-Classifica (Ordinata con rimpiazzo dinamico):
  - `Assets/CuteRush kit/Scripts/SaveSystem/GlobalLeaderboardData.cs` (riga 23: `topScores`)
  - `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (riga 76: `SubmitScore`, limita la classifica locale e globale con logica di rimpiazzo dinamico).


GamePlay
-Score: Incrementato in `Assets/CuteRush kit/Scripts/GameManager.cs` (riga 84: `AddKillScore(int points)`) e richiamato da `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 437: `GameManager.Instance.AddKillScore(killPoints)`).
-PowerUp/Shop/Bonus giocatore:
  - Upgrades gestiti in `Assets/CuteRush kit/Scripts/Upgrades/UpgradeManager.cs`.
  - Drop salute: `Assets/CuteRush kit/Scripts/Consumabili/Medkit.cs`.
  - Drop colpi: `Assets/CuteRush kit/Scripts/Consumabili/Mags.cs`.
  - Upgrade arma: `Assets/CuteRush kit/Scripts/Player/PlayerUpgrades.cs`.
-Gioco a tempo: Gestito da `Assets/CuteRush kit/Scripts/GameManager.cs` (Coroutine `TimerRoutine` a riga 119).
-Presenza di nemici/sfida: Logica base nemici in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` con i vari spawner `EnemySpawner.cs`.
-Livelli di difficoltà: Definiti in `Assets/CuteRush kit/Scripts/Difficulty/DifficultyManager.cs` e `DifficultyProfile.cs`. Selezionati tramite `DifficultySetter.cs`.
-Difficoltà crescente: La difficoltà scala danni/salute, come visto in `Assets/CuteRush kit/Scripts/Enemy/RangedEnemy.cs` (riga 44: `scaledRangedDamage = Mathf.RoundToInt(rangedAttackDamage * diffMult)`).
-AI complicata: L'AI analizza i movimenti del player per anticipare la mira.
  - File: `Assets/CuteRush kit/Scripts/Enemy/RangedEnemy.cs` (riga 54: `PerformChaseLogic()` -> riga 61: `Vector3 predictedTarget = GetPredictedPlayerPosition(...)`).
  - Funzione chiave: `GetPredictedPlayerPosition` presente in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 483), che esegue un'analisi `GetVelocityOverWindow` per sparare in anticipo rispetto al movimento del player.

Tutorial
- 3 Schermate statiche visualizzabili nel main menù (MainMenuLoadSave)

Strutture
-PlayerPrefs:
  - `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (riga 120, 134, 168).
  - `Assets/CuteRush kit/Scripts/Difficulty/DifficultySetter.cs` (riga 11).
  - `Assets/CuteRush kit/Scripts/Player/PlayerUpgrades.cs` (riga 71, 72, 84).
  - `Assets/CuteRush kit/Scripts/Player/PlayerCombat.cs` (riga 56).
  - `Assets/CuteRush kit/Scripts/Player/InventoryPlayer.cs` (riga 40, 42).
  - `Assets/CuteRush kit/Scripts/GameManager.cs` (riga 45).
  - `Assets/CuteRush kit/UI/Scripts/UIPlayerUpgradesPanel.cs` (riga 103, 343, 347, 351).
  - `Assets/CuteRush kit/Scripts/SaveSystem/MainMenuSaveIntegration.cs` (riga 81, 82, 104, 105).
-Singleton:
  - `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (riga 7: `Instance`).
  - `Assets/CuteRush kit/UI/Singleton/UIManager.cs` (riga 6: `Instance`).
  - `Assets/CuteRush kit/Scripts/GameManager.cs` (riga 8: `Instance`).
  - `Assets/CuteRush kit/Scripts/Difficulty/DifficultyManager.cs` (riga 7: `Instance`).
  - `Assets/CuteRush kit/Scripts/Bootstrapper.cs` (riga 7: `Instance`).
  - `Assets/CuteRush kit/Scripts/Upgrades/UpgradeManager.cs` (riga 6: `Instance`).
-Coroutines:
  - HUD.cs:160, DamageIndicator.cs:64, Enemy.cs:395, 470, 581, EnemySpawner.cs:77, 98, HybridEnemy.cs:175, Bootstrapper.cs:45, DifficultyManager.cs:26, Bonfire.cs:42, WeaponSpawner.cs:53, VitalsController.cs:91, PlayerControl.cs:276, PlayerCombat.cs:52, InventoryPlayer.cs:71, SingleShotGun.cs:15, ChargeGun.cs:19, GunChargeOverTime.cs:65, AutoGun.cs:13, Granade.cs:53, ItemSpawner.cs:43, GunBase.cs:182, 232, GameManager.cs:119.
-Enums:
  - `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 6: `AIState`).
  - `Assets/CuteRush kit/Scripts/Player/PlayerMovement.cs` (riga 14: `motionstate`).
  - `Assets/CuteRush kit/Scripts/GameManager.cs` (riga 36: `GameState`).
  - `Assets/CuteRush kit/Scripts/Weapons/GunStats.cs` (riga 3: `FireMode`).
  - `Assets/CuteRush kit/Scripts/Weapons/RecoilController.cs` (riga 6: `Handedness`).
  - `Assets/CuteRush kit/Scripts/Camera/Crosshair.cs` (riga 6: `preset`).
-Classi statiche:
  - `Assets/CuteRush kit/UI/Static/UIEvents.cs` (riga 4).
  - `Assets/CuteRush kit/Scripts/Utility/GameConstants.cs` (riga 1).
  - `Assets/CuteRush kit/Scripts/Utility/GameExtension.cs` (riga 5).
-Generics:
  - `Assets/CuteRush kit/Scripts/Utility/GameExtension.cs` (riga 27: `GetRandomWeightedItem<T>`).
  - `Assets/CuteRush kit/Scripts/AbstractClass/PickableItem.cs` (riga 5: `PickableItem<T>`).
-Presenza di ereditarietà: Molti script ereditano da classi base: `RangedEnemy.cs`, `MeleeEnemy.cs`, `HybridEnemy.cs` ereditano da `Enemy.cs`. `ChargeGun.cs`, `SingleShotGun.cs`, `AutoGun.cs` ereditano da `GunBase.cs`.
-Overriding: Presente in svariate classi derivate:
  - `RangedEnemy.cs`: Start, OnEnable, PerformChaseLogic, PerformAttack, InterruptAttack.
  - `MeleeEnemy.cs`: Start, PerformChaseLogic, PerformAttack.
  - `HybridEnemy.cs`: Start, OnEnable, PerformChaseLogic, PerformAttack.
  - `EnemySpawner.cs`: InitSpawner.
  - `Bonfire.cs`: Interact.
  - `MedkitSpawner.cs`, `MagSpawner.cs`: TryGiveItem.
  - `GateController.cs`: Interact, OnTriggerEnter, OnTriggerExit.
  - `WeaponSpawner.cs`: Interact, OnTriggerEnter, OnTriggerExit.
  - `Mags.cs`, `GrenadeDrop.cs`, `Coin.cs`, `Medkit.cs`: ApplyEffect.
  - `SingleShotGun.cs`, `ChargeGun.cs`, `AutoGun.cs`: Shoot.
-Interfacce:
  - `Assets/CuteRush kit/Scripts/Interface/IPickable.cs` (riga 5).
  - `Assets/CuteRush kit/Scripts/Interface/Interactable.cs` (riga 5).
  - `Assets/CuteRush kit/Scripts/Player/PlayerControl.cs` (riga 379: `IPlayerActions`).
-ExtensionMethods:
  - `Assets/CuteRush kit/Scripts/Utility/GameExtension.cs` (riga 7: `WithY`, riga 12: `HasReachedDestination`, riga 27: `GetRandomWeightedItem<T>`).
-Delegates:
  - `Assets/CuteRush kit/Scripts/Player/VitalsController.cs` (riga 12: `HealthChanger`, riga 13: `DamageTaker`).
  - `Assets/CuteRush kit/Scripts/Player/InventoryPlayer.cs` (riga 23: `ResourceChangedHandler`).
-Eventi:
  -`OnJump`, `OnReload`, `OnInteract`, `OnHeal` in `PlayerInput.cs`; `OnTakeDamage`, `OnHealthChange` in `VitalsController.cs`; `OnScoreChange`, `OnGameOver` in `GameManager.cs`.
-Animazioni Originali:
	-
-Sound: Tutti presenti in `Assets/CuteRush kit/Audio/Script/AudioPlayerController.cs` e `AudioGunController.cs`:
  - `PlayShoot()`, `PlayRecharge()`, `PlayCharge()`, `PlayNoAmmo()`, `PlayFootstep()`, `PlayHealSound()`, `PlayGoldSound()`, `PlayPickupSound()`, `PlayDamageSound()`.
-Raycast:
  - `Assets/CuteRush kit/Scripts/Player/PlayerMovement.cs` (riga 123: `Physics.Raycast(...)` per controllo a terra).
  - `Assets/CuteRush kit/Scripts/Player/VitalsController.cs` (riga 105: `Physics.Raycast(...)`).
  - `Assets/CuteRush kit/Scripts/AbstractClass/GunBase.cs` (riga 127: `Physics.Raycast(ray, out RaycastHit hit, stats.range, hitLayers)`).
-User Interface:
  - AcidBarItem.cs, SaveSlotUI.cs, PlayerInfoUpdater.cs, UIPlayerUpgradesPanel.cs, HUD.cs, LatterboxRequester.cs, GameOverScreen.cs, UiAcidoBorico.cs, DamageIndicator.cs, MainMenu.cs, LoadingScreenController.cs. Tutte in `Assets/CuteRush kit/UI/Scripts/`.

EXTRA
-Particelle:
  - Nemici (sangue/hit): `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 35: `ParticleSystem hitParticlePrefab`).
  - Falò (World/Ambiente): `Assets/CuteRush kit/Scripts/World/Bonfire.cs` (riga 10: `ParticleSystem[] allFireParticleSystems`).
  - Armi (Spari/Bossoli): `Assets/CuteRush kit/Scripts/AbstractClass/GunBase.cs` (riga 11: `ParticleSystem[] gunParticleSystems`).


NOTE
-Nuovo Input System (Unity Package):

Riferimenti: "Assets/CuteRush kit/Scripts/Player/PlayerInput.cs", "Assets/PlayerControl.cs" (classe C# auto-generata dall'Input Action Asset).

Perché usare il nuovo sistema invece del vecchio "InputManager" ("Input.GetAxis", "Input.GetKeyDown")?

Action-Based (Event-Driven): Invece di interrogare la tastiera 60 volte al secondo nell'"Update()" (Polling), il nuovo sistema si basa sugli eventi (es. "OnJump", "OnMove"). Questo approccio è molto più pulito, reattivo e meno esoso per la CPU.

Cross-Platform Nativo: Astrae completamente l'hardware. L'azione "Move" funziona in automatico sia con i tasti WASD che con la levetta analogica di un Gamepad, senza dover scrivere righe di codice separate per periferiche diverse.

-Addressable Asset System:

Perché usare gli Addressables invece dei classici riferimenti diretti (Direct References nell'Inspector) o della cartella "Resources"?

Gestione Asincrona della Memoria: Gli Addressables caricano gli asset (es. prefab di nemici, armi pesanti, menu UI) in modo asincrono solo nel momento esatto in cui servono, e permettono di "scaricarli" (Release) dalla RAM quando non sono più utilizzati. I riferimenti diretti, al contrario, caricano tutto il peso dell'oggetto in memoria non appena la scena viene aperta, causando picchi di consumo RAM.

Ottimizzazione della Build: Evita il noto "collo di bottiglia" della cartella "Resources". Tutto ciò che si trova in "Resources" viene indicizzato da Unity, aumentando i tempi di caricamento e gonfiando le dimensioni della build base.

Disaccoppiamento e Flessibilità: Il codice chiama un "indirizzo testuale" (Address) anziché un percorso fisico. Questo significa che è possibile spostare i file nel progetto, raggrupparli o (in futuro) persino scaricarli da un server remoto (DLC/Updates) senza dover modificare e ricompilare una singola riga di codice C#.

