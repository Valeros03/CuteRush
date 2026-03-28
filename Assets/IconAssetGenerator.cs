using UnityEngine;

public class IconAssetGenerator : MonoBehaviour
{
    public RenderTexture sourceRenderTexture; // Trascina qui la tua RenderTexture
    public string iconName = "NuovaIconaArma"; // Nome del file file

    [ContextMenu("Salva Icona come PNG trasparente")]
    public void SaveIconToProject()
    {
        if (sourceRenderTexture == null) return;

        // 1. Convertiamo in Texture2D
        Texture2D tex = TextureConverter.RenderTextureToTexture2D(sourceRenderTexture);

        // 2. Creiamo il percorso (cartella Assets)
        string path = Application.dataPath + "/" + iconName + ".png";

        // 3. Salviamo il file
        TextureConverter.SaveTexture2DToPNG(tex, path);

        // Nota: dopo aver salvato, dovrai cliccare sulla cartella Assets 
        // in Unity per fargli vedere il nuovo file.
        // Ricordati di impostare il PNG appena creato come "Sprite (2D and UI)" 
        // nelle sue import settings.
    }
}