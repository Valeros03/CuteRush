using UnityEngine;
using System.IO; // Necessario per salvare i file

public static class TextureConverter
{
    /// <summary>
    /// Converte una RenderTexture in una Texture2D con supporto alla trasparenza (Alpha).
    /// </summary>
    public static Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        // 1. Crea una nuova Texture2D con la stessa risoluzione, formato RGBA32 (trasparenza)
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);

        // Memorizza la RT attualmente attiva per ripristinarla dopo
        RenderTexture previousActive = RenderTexture.active;

        // 2. Imposta la nostra RenderTexture come "attiva" per Unity
        RenderTexture.active = rTex;

        // 3. Legge i pixel dalla RT attiva e li copia nella Texture2D
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply(); // Applica i cambiamenti

        // Ripristina la RT precedente
        RenderTexture.active = previousActive;

        return tex;
    }

    /// <summary>
    /// Converte una Texture2D in un Sprite pronto per la UI di Unity.
    /// </summary>
    public static Sprite Texture2DToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// Salva una Texture2D come file PNG trasparente in un percorso specifico.
    /// </summary>
    public static void SaveTexture2DToPNG(Texture2D tex, string filePath)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Icona salvata in: " + filePath);
    }
}