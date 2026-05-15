Caratteristiche e Funzionalità in CuteRush kit
==============================================

Questo documento elenca la posizione e la descrizione delle funzionalità richieste all'interno del progetto CuteRush kit.

## Impostazioni generali
* **Build mobile:** Il gioco include configurazioni per input mobile visibili in `PlayerInput.cs` e nei package di Unity, confermando la compatibilità con build mobile.
* **Splash Screen:** Impostato in `ProjectSettings/ProjectSettings.asset` (riga 23: `m_ShowUnitySplashScreen: 1`, `m_SplashScreenBackgroundColor`, ecc.). Gestisce l'animazione di avvio del gioco e la visualizzazione del logo.
* **Icona gioco:** Presente nelle configurazioni di build del progetto (visualizzabile nei settings del player in Unity).

## Main menu
* **Load Game (CheckPoint):** Gestito in `Assets/CuteRush kit/Scripts/SaveSystem/MainMenuSaveIntegration.cs` e `Assets/CuteRush kit/UI/Scripts/MainMenu.cs`. Il salvataggio viene gestito tramite la classe `SaveManager` e lo slot di salvataggio viene caricato dal menù.
* **Options:**
  * **Sound/Music:** `Assets/CuteRush kit/UI/Scripts/MainMenu.cs` (Gestisce i toggle del menù) e `Assets/CuteRush kit/Audio/Script/AudioManager.cs` per l'effettivo controllo di Volume Master, Musica, SFX e Suoni d'ambiente.
* **Classifica:**
  * **Ordinata con rimpiazzo dinamico (Top 5):** `Assets/CuteRush kit/Scripts/SaveSystem/GlobalLeaderboardData.cs` (Definisce la struttura) e `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (riga ~178 `SubmitScore` e gestione della logica di inserimento, limitata a 5 per mappa con rimpiazzo dei punteggi inferiori).

## GamePlay
* **Score:** `Assets/CuteRush kit/Scripts/GameManager.cs` (riga 14 `OnScoreChange`, gestisce l'incremento dello score ad ogni uccisione, es. riga 437 in `Enemy.cs`).
* **PowerUp/Shop/Bonus giocatore:**
  * Sistema potenziamenti e shop gestito da `Assets/CuteRush kit/Scripts/Upgrades/UpgradeManager.cs`. I consumabili come Medkit e Grenade sono considerati PowerUp temporanei e shop.
* **Gioco a tempo:** `Assets/CuteRush kit/Scripts/GameManager.cs` (Coroutine `TimerRoutine` a riga 119 e `OnTimeUpdated` riga 33).
* **Presenza di nemici/sfida:** Gestiti nelle classi sotto `Assets/CuteRush kit/Scripts/Enemy/` (es. `Enemy.cs`, `MeleeEnemy.cs`, `RangedEnemy.cs`).
* **Livelli di difficoltà & Difficoltà crescente:** `Assets/CuteRush kit/Scripts/Difficulty/DifficultyManager.cs` e `DifficultySetter.cs`. `GameManager.cs` (riga 45) carica il profilo di difficoltà selezionato tramite PlayerPrefs.
* **AI base (NavMesh):** I nemici implementano NavMeshAgent per muoversi verso il player, come visto in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` e `MeleeEnemy.cs`.

## Strutture
* **PlayerPrefs:** Utilizzato massicciamente in `Assets/CuteRush kit/Scripts/SaveSystem/SaveManager.cs` (es. riga 120 per "Weapon"), `DifficultySetter.cs` (riga 11 per "DifficultyProfile") e `InventoryPlayer.cs` (riga 40 per Acido Borico).
* **Singleton:** `SaveManager.cs` (riga 7), `UIManager.cs` (riga 6), `GameManager.cs` (riga 8), `DifficultyManager.cs` (riga 7). Pattern usato per avere un'unica istanza globale di questi manager.
* **Coroutines:** `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 395 `DamageFlinchRoutine`, riga 470 `DisableAfterTime`), `Bootstrapper.cs` (riga 45 `TransitionSceneRoutine`), `GameManager.cs` (riga 119 `TimerRoutine`). Utilizzate per logiche basate sul tempo.
* **Enums:** `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (riga 6 `AIState`), `PlayerMovement.cs` (riga 14 `motionstate`), `GameManager.cs` (riga 36 `GameState`).
* **Classi statiche:** `Assets/CuteRush kit/UI/Static/UIEvents.cs` (riga 4), `GameConstants.cs` (riga 1), `GameExtensions.cs` (riga 5).
* **Generics:** `Assets/CuteRush kit/Scripts/Utility/GameExtension.cs` (riga 27 `GetRandomWeightedItem<T>`), `PickableItem.cs` (riga 5 `PickableItem<T>`).
* **Presenza di ereditarietà & Overriding:** Le classi dei nemici (`RangedEnemy.cs`, `MeleeEnemy.cs`) ereditano da `Enemy.cs` ed eseguono override di metodi come `Start()`, `PerformAttack()`, ecc. Le armi ereditano da `GunBase.cs`. I pannelli UI da `UIPanel.cs` facendo override di `Show()`.
* **Interfacce:** `Assets/CuteRush kit/Scripts/Interface/IPickable.cs` (riga 5), `Interactable.cs` (riga 5).
* **ExtensionMethods:** `Assets/CuteRush kit/Scripts/Utility/GameExtension.cs` (riga 7 `WithY`, riga 12 `HasReachedDestination`, riga 27 `GetRandomWeightedItem<T>`).
* **Delegates ed Eventi:** `Assets/CuteRush kit/Scripts/Player/VitalsController.cs` (riga 12 `HealthChanger`, riga 13 `DamageTaker`), `GameManager.cs` (riga 14 `OnScoreChange`), `PlayerInput.cs` (Eventi Action come `OnJump`).
* **Raycast:** `Assets/CuteRush kit/Scripts/Player/PlayerMovement.cs` (riga 123 per il ground check) e `GunBase.cs` (riga 127 per sparare proiettili hitscan).

## EXTRA
* **Particelle:** Utilizzate in `Assets/CuteRush kit/Scripts/Enemy/Enemy.cs` (es. riga 35 `hitParticlePrefab` usato quando i nemici vengono colpiti).
