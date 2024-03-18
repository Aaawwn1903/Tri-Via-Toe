using System.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;

public class FirebaseImageDownloader : MonoBehaviour
{
    public string imagePath;  // Inspector field to set image path (relative to storage bucket)

    private Firebase.Storage.FirebaseStorage storage;
    private UnityEngine.UI.RawImage imageUI;  // Reference to the RawImage UI element (assign in Inspector)

    private async void Start()
    {
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        imageUI = GetComponent<UnityEngine.UI.RawImage>();  // Assuming the script is on the same GameObject

        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Please set the image path in the inspector");
            return;
        }

        await DownloadAndDisplayImage();
    }

    private async Task DownloadAndDisplayImage()
    {
        StorageReference imageRef = storage.GetReferenceFromUrl("https://firebasestorage.googleapis.com/v0/b/tri-via-toe-6e32e.appspot.com/o/Person.png");

        try
        {
            // Option 1: Explicitly specify a large max size
            byte[] imageBytes = await imageRef.GetBytesAsync(long.MaxValue);

            // Option 2: Call without argument (if confident about no size restriction)
            // byte[] imageBytes = await imageRef.GetBytesAsync();

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageBytes);
            imageUI.texture = texture;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to download image: {e.Message}");
        }
    }
}