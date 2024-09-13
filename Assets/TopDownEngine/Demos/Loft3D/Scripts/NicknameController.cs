using System.IO;
using TMPro;
using UnityEngine;

public class NicknameController : MonoBehaviour
{
    [SerializeField] private TextMeshPro Text;
    public int layerToAlwaysOnTop = 31; // Assign a unique layer
    public string fileName = "MyNickname.txt";

    void Start()
    {
        gameObject.layer = layerToAlwaysOnTop;
        if (Camera.main != null)
        {
            Camera.main.cullingMask &= ~(1 << layerToAlwaysOnTop); // Exclude the layer from the camera's culling mask
        }
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    public void SetNickname(string nick)
    {
        Text.SetText(nick);
    }

    public string LoadCachedNicknameFromStorage()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string textContent = File.ReadAllText(filePath);
            Debug.Log("Text content: " + textContent);
            return textContent;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

        return string.Empty;
    }
}