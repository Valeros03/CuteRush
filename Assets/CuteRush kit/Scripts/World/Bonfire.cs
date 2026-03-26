using UnityEngine;

public class Bonfire : InteractableItem
{
    [Header("Bonfire Connections")]
    [Tooltip("Lo spawner dei nemici collegato a questo falò")]
    public EnemySpawner linkedSpawner;

    [Tooltip("L'oggetto con le particelle del fuoco da spegnere")]
    public GameObject fireParticles;

    private void Start()
    {
        interactText = "Spegnere il falò";
    }

    public override void Interact()
    {
        if (!isInteractable) return;

        if (fireParticles != null)
        {
            fireParticles.SetActive(false);
        }

        if (linkedSpawner != null)
        {
            linkedSpawner.StopSpawn();
        }

        isInteractable = false;
        HideInteraction();


    }
}