// using UnityEngine;
// using UnityEngine.UI;
// using Firebase;
// using Firebase.Firestore;
// using System.Collections;

// public class DisplayCountryImage : MonoBehaviour
// {
//     public RawImage rawImage;
//     private string firestoreCollection = "countries";

//     void Start()
//     {
//         // Initialize Firebase Firestore
//         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//         {
//             if (task.IsCompleted)
//             {
//                 // Fetch image URL from Firestore
//                 FetchImageURL();
//             }
//             else
//             {
//                 Debug.LogError("Failed to initialize Firebase Firestore: " + task.Exception);
//             }
//         });
//     }

//     void FetchImageURL()
//     {
//         // Access Firestore database
//         FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

//         // Reference to the collection containing country data
//         CollectionReference countriesRef = db.Collection(firestoreCollection);

//         // Query to get a specific document (for example, the first one)
//         Query firstCountryQuery = countriesRef.Limit(1);

//         // Fetch the document
//         firstCountryQuery.GetSnapshotAsync().ContinueWith(task =>
//         {
//             if (task.IsCompleted)
//             {
//                 // Get the first document snapshot
//                 QuerySnapshot snapshot = task.Result;
//                 foreach (DocumentSnapshot document in snapshot.Documents)
//                 {
//                     // Get the data dictionary from the document
//                     IDictionary countryData = document.ToDictionary();

//                     // Check if imageURL exists in the dictionary
//                     if (countryData.Contains("imageURL"))
//                     {
//                         // Get the imageURL value from the dictionary
//                         string imageURL = countryData["imageURL"].ToString();
//                         if (!string.IsNullOrEmpty(imageURL))
//                         {
//                             // If imageURL is valid, load the image
//                             StartCoroutine(LoadImage(imageURL));
//                         }
//                         else
//                         {
//                             Debug.LogWarning("Image URL is empty or invalid.");
//                         }
//                     }
//                 }
//             }
//             else
//             {
//                 Debug.LogError("Failed to fetch document: " + task.Exception);
//             }
//         });
//     }

//     // Coroutine to download and load the image from the provided URL
//     IEnumerator LoadImage(string url)
//     {
//         // Create UnityWebRequest to download the image
//         using (WWW www = new WWW(url))
//         {
//             // Wait for the download to complete
//             yield return www;

//             // Check for errors
//             if (!string.IsNullOrEmpty(www.error))
//             {
//                 Debug.LogError("Error downloading image: " + www.error);
//             }
//             else
//             {
//                 // Assign the downloaded texture to the Raw Image UI component
//                 rawImage.texture = www.texture;
//             }
//         }
//     }
// }
