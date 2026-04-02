using UnityEngine;

[ExecuteInEditMode]
public class ExplosionVisualScaler : MonoBehaviour
{
    [Header("Raggio Logico dell'Esplosione (Target)")]
    public float logicExplosionRadius = 5f;

    [Header("Dimensione Originale Asset")]
    [Tooltip("Quanto è grande l'esplosione di default a Scala 1? (Di solito è 2 o 3)")]
    public float originalAssetRadius = 2f;

    [Header("Moltiplicatori Visivi Specifici")]
    [Range(0.1f, 3f)] public float fireMultiplier = 1.0f;
    [Range(0.1f, 3f)] public float flashMultiplier = 1.0f;
    [Range(0.1f, 3f)] public float groundMultiplier = 1.0f;
    [Range(0.1f, 3f)] public float sparkMultiplier = 1.0f;

    [Header("Riferimenti Transform (Trascina gli oggetti qui)")]
    [SerializeField] private Transform fireUp;
    [SerializeField] private Transform impact;
    [SerializeField] private Transform sphere;
    [SerializeField] private Transform[] groundEffects;
    [SerializeField] private Transform spark;

    private void OnEnable()
    {
        UpdateVisualScale();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        UpdateVisualScale();
    }
#endif

    public void UpdateVisualScale()
    {
        if (logicExplosionRadius <= 0 || originalAssetRadius <= 0) return;

        float baseScale = logicExplosionRadius / originalAssetRadius;

        ScaleTransform(fireUp, baseScale * fireMultiplier);
        ScaleTransform(impact, baseScale * fireMultiplier);
        ScaleTransform(sphere, baseScale * flashMultiplier);
        ScaleTransform(spark, baseScale * sparkMultiplier);

        if (groundEffects != null)
        {
            foreach (var ground in groundEffects)
            {
                ScaleTransform(ground, baseScale * groundMultiplier);
            }
        }
    }

    private void ScaleTransform(Transform t, float finalScale)
    {
        if (t != null)
        {
            t.localScale = new Vector3(finalScale, finalScale, finalScale);
        }
    }
}