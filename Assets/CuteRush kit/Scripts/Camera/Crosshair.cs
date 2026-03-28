using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public enum preset { none, pistolPreset }
    public preset crosshairPreset = preset.none;

    public bool showCrosshair = true;
    public Texture2D verticalTexture;
    public Texture2D horizontalTexture;

    public float cLength = 10.0f;
    public float cWidth = 3.0f;

    [Header("Dynamic Spread Settings")]
    public float minSpread = 45.0f;
    public float maxSpread = 250.0f;
    public float spreadPerSecond = 150.0f;


    public float moveSpread = 90.0f;
    public float jumpSpread = 150.0f;
    public float recoverSpeed = 8.0f;

    public float rotAngle = 0.0f;
    public float rotSpeed = 0.0f;

    [HideInInspector] public Texture2D temp;
    [HideInInspector] public float spread;

    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isJumping = false;

    private float currentWeaponShootBloomKick;

    void Start()
    {
        crosshairPreset = preset.none;
        spread = minSpread;
    }

    public void LoadGunSettings(GunStats stats)
    {
        currentWeaponShootBloomKick = stats.crosshairShootBloomKick;
    }

    void Update()
    {
        rotAngle += rotSpeed * Time.deltaTime;

        float targetSpread = minSpread;
        if (isJumping) targetSpread = jumpSpread;
        else if (isMoving) targetSpread = moveSpread;

        spread = Mathf.Lerp(spread, targetSpread, Time.deltaTime * recoverSpeed);
    }

    public void ApplyShootKick()
    {
        spread += currentWeaponShootBloomKick;
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

                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height - spread) / 2 - 3, 14, 3), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height + spread) / 2, 14, 3), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - spread) / 2 - 3, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
            }

            if (crosshairPreset == preset.none)
            {
                GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);

                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height - spread) / 2 - cLength, cWidth, cLength), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height + spread) / 2, cWidth, cLength), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - spread) / 2 - cLength, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
            }
        }
    }
}