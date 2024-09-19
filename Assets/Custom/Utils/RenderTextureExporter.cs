using UnityEngine;

public class RenderTextureExporter : MonoBehaviour
{
    [SerializeField] private string _savePath;
    [SerializeField] private SaveTextureToFileUtility.SaveTextureFileFormat _format = SaveTextureToFileUtility.SaveTextureFileFormat.PNG;
    [Range(1, 100)]
    [SerializeField] private int _quality = 75;
    
    [ContextMenu("SaveToTexture")]
    public void SaveRenderTexture()
    {
        Camera cam = GetComponent<Camera>();
        SaveTextureToFileUtility.SaveRenderTextureToFile(cam.targetTexture, _savePath, _format, _quality);
        Debug.Log("Export finished");
    }
}

public class SaveTextureToFileUtility
{
    public enum SaveTextureFileFormat
    {
        EXR,
        JPG,
        PNG,
        TGA
    };

    /// <summary>
    /// Saves a Texture2D to disk with the specified filename and image format
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="filePath"></param>
    /// <param name="fileFormat"></param>
    /// <param name="jpgQuality"></param>
    static public void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat,
        int jpgQuality = 95)
    {
        switch (fileFormat)
        {
            case SaveTextureFileFormat.EXR:
                System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
                break;
            case SaveTextureFileFormat.JPG:
                System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
                break;
            case SaveTextureFileFormat.PNG:
                System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
                break;
            case SaveTextureFileFormat.TGA:
                System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
                break;
        }
    }


    /// <summary>
    /// Saves a RenderTexture to disk with the specified filename and image format
    /// </summary>
    /// <param name="renderTexture"></param>
    /// <param name="filePath"></param>
    /// <param name="fileFormat"></param>
    /// <param name="jpgQuality"></param>
    static public void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath,
        SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
    {
        Texture2D tex;
        if (fileFormat != SaveTextureFileFormat.EXR)
            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
        else
            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);
        var oldRt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = oldRt;
        SaveTexture2DToFile(tex, filePath, fileFormat, jpgQuality);
        if (Application.isPlaying)
            Object.Destroy(tex);
        else
            Object.DestroyImmediate(tex);
    }
}