// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using Firebase.Storage;
// using Firebase.Extensions;
// public class RandomImagePicker : MonoBehaviour // Renamed the class
// {
//     // Firebase storage link
//     private const string firebaseStorageLink = "https://firebasestorage.googleapis.com/v0/b/tri-via-toe-6e32e.appspot.com/o/";

//     // Firebase storage path
//     private const string firebaseStoragePath = "/";

//     private Firebase.Storage.FirebaseStorage storage;
//     private UnityEngine.UI.RawImage imageUI;  // Reference to the RawImage UI element (assign in Inspector)

//     private async void Start()
//     {
//         storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
//         imageUI = GetComponent<UnityEngine.UI.RawImage>();  // Assuming the script is on the same GameObject

//         await DownloadAndDisplayRandomImage();
//     }

//     private async Task DownloadAndDisplayRandomImage()
//     {
//         StorageReference imagesRef = storage.GetReferenceFromUrl(firebaseStorageLink + firebaseStoragePath);

//         try
//         {
//             // List all items (images) in the given path
//             List<string> imageNames = new List<string>();
//             StorageListResult result = await imagesRef.ListAllAsync();
//             foreach (StorageReference item in result.Items)
//             {
//                 imageNames.Add(item.Name);
//             }

//             // Randomly pick an image from the list
//             int randomIndex = Random.Range(0, imageNames.Count);
//             string randomImageName = imageNames[randomIndex];
//             StorageReference imageRef = imagesRef.Child(randomImageName);

//             // Download and display the randomly selected image
//             byte[] imageBytes = await imageRef.GetBytesAsync(long.MaxValue);

//             Texture2D texture = new Texture2D(1, 1);
//             texture.LoadImage(imageBytes);
//             imageUI.texture = texture;
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Failed to download image: {e.Message}");
//         }
//     }
// }
