using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public enum preset { none, pistolPreset }
    public preset crosshairPreset = preset.none;

    public bool showCrosshair = true;
    public Texture2D verticalTexture;
    public Texture2D horizontalTexture;

    //Size of boxes
    public float cLength = 10.0f;
    public float cWidth = 3.0f;

    [Header("Dynamic Spread Settings")]
    public float minSpread = 45.0f;
    public float maxSpread = 250.0f;
    public float spreadPerSecond = 150.0f;

    // Nuovi parametri per le varie azioni
    public float moveSpread = 90.0f;         // Quanto si allarga camminando
    public float jumpSpread = 150.0f;        // Quanto si allarga saltando
    public float recoverSpeed = 8.0f;        // Quanto velocemente la molla la fa rimpicciolire

    //Rotation
    public float rotAngle = 0.0f;
    public float rotSpeed = 0.0f;

    [HideInInspector] public Texture2D temp;
    [HideInInspector] public float spread;

    // Queste variabili verranno lette per capire cosa fa il player
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isJumping = false;

    // Riferimento al kick specifico dell'arma
    private float currentWeaponShootBloomKick;

    void Start()
    {
        crosshairPreset = preset.none;
        spread = minSpread;
    }

    // NUOVO METODO: Carica le impostazioni specifiche dell'arma
    public void LoadGunSettings(GunStats stats)
    {
        currentWeaponShootBloomKick = stats.crosshairShootBloomKick;
        // Se vuoi, puoi anche cambiare min/maxSpread in base all'arma qui
    }

    void Update()
    {
        // Rotazione
        rotAngle += rotSpeed * Time.deltaTime;

        // 1. Decidiamo qual è la grandezza base in questo momento
        float targetSpread = minSpread;
        if (isJumping) targetSpread = jumpSpread;
        else if (isMoving) targetSpread = moveSpread;

        // 2. Effetto "Molla": la crosshair si allarga o restringe fluidamente verso il target
        spread = Mathf.Lerp(spread, targetSpread, Time.deltaTime * recoverSpeed);
    }

    // 3. Questo metodo verrà chiamato DAL FUCILE ogni volta che spari
    public void ApplyShootKick()
    {
        spread += currentWeaponShootBloomKick;
        // Impediamo che diventi più grande del maxSpread consentito
        spread = Mathf.Clamp(spread, minSpread, maxSpread);
    }

    void OnGUI()
    {
        if (showCrosshair && verticalTexture && horizontalTexture)
        {
            GUIStyle verticalT = new GUIStyle();
            GUIStyle horizontalT = new GUIStyle();
            verticalT.normal.background = verticalTexture;
            horizontalT.normal.background = horizontalTexture;

            Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);

            if (crosshairPreset == preset.pistolPreset)
            {
                GUIUtility.RotateAroundPivot(45, pivot);

                //Horizontal
                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height - spread) / 2 - 3, 14, 3), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height + spread) / 2, 14, 3), temp, horizontalT);
                //Vertical
                GUI.Box(new Rect((Screen.width - spread) / 2 - 3, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
            }

            if (crosshairPreset == preset.none)
            {
                GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);

                //Horizontal
                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height - spread) / 2 - cLength, cWidth, cLength), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height + spread) / 2, cWidth, cLength), temp, horizontalT);
                //Vertical
                GUI.Box(new Rect((Screen.width - spread) / 2 - cLength, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
            }
        }
    }
}