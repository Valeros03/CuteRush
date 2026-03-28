using UnityEngine;
using System.Collections;

public class Bonfire : InteractableItem
{
    [Header("Bonfire Connections")]
    public EnemySpawner linkedSpawner;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem[] allFireParticleSystems;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color shutdownColor = Color.green;

    private void Start()
    {
        interactText = "Spegnere il falò";
        SetAllParticlesColor(activeColor);
    }

    public override void Interact()
    {

        if (!isInteractable || linkedSpawner == null)
        {
            UIManager.Instance.ShowMessage("Acido Borico Già Presente", new Color32(230, 80, 30, 255));
            return;
        }
        linkedSpawner.StopSpawn();

        isInteractable = false;
        HideInteraction();

        StartCoroutine(VisualCooldownRoutine(linkedSpawner.shutdownDuration));
    }

    private IEnumerator VisualCooldownRoutine(float duration)
    {
        float elapsed = 0f;

        SetAllParticlesColor(shutdownColor);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color currentColor = Color.Lerp(shutdownColor, activeColor, t);
            SetAllParticlesColor(currentColor);

            yield return null;
        }

        SetAllParticlesColor(activeColor);
        isInteractable = true;
    }

    private void SetAllParticlesColor(Color color)
    {
        if (allFireParticleSystems == null) return;

        for (int i = 0; i < allFireParticleSystems.Length; i++)
        {
            if (allFireParticleSystems[i] != null)
            {
                ParticleSystem.MainModule main = allFireParticleSystems[i].main;
                main.startColor = color;
            }
        }
    }
}