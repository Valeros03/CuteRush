1. **Update `SaveData.cs`**:
   - Change `WeaponUpgradesSave` to store an integer level per weapon type (e.g., `pistolLevel`, `smgLevel`, `railgunLevel`) instead of full `WeaponStats` objects per weapon type.

2. **Update `GameBootstrapper.cs` and `WeaponSpawner.cs` to load weapon stats based on level**:
   - Use `Resources.Load<GunStats>($"WeaponPresets/{weaponName} Preset {level}")` to dynamically load weapon stats based on the save data level. Note: Since I moved the presets into `Resources/WeaponPresets`, we can easily load them. I will adjust script to correctly load the resource if `GunStats` is a `ScriptableObject`.

3. **Update `PlayerCombat.cs` or `GunBase.cs`**:
   - Update the current equipped weapon with the specific `GunStats` based on its level during `Init` in `PlayerCombat.cs` or by introducing a helper function to set weapon stats based on level.
   - Specifically, when `GameBootstrapper` calls `PlayerCombat.Init()`, we need to also apply the correct level preset to the starting gun.
   - Inside `WeaponSpawner.cs`, after instantiating the real weapon, apply the correct preset to it before it's picked up, or when it's picked up in `PlayerCombat.EquipWeapon`. It's better to do it in `EquipWeapon` or `Init` for consistency.

4. **Add Presets for testing (optional/if needed)**:
   - Actually, since the issue just says "So there's gonna be something like Railgun base, RailGun 1, Railgun 2... do it inside the creation function or wherever better", I will add a method to get the current weapon level based on the weapon name, then load `Resources.Load<GunStats>($"WeaponPresets/{weaponName} Preset {level}")`. If it doesn't exist, we fallback to level 1.
   - Wait, the issue says "Now the weapon levels up instead of upgrading the single Characteristic". Currently `WeaponStats` in SaveData held levels for damage, mag size, etc. We will replace `WeaponStats` class with just weapon level integers.

5. **Run tests/pre-commits**.
