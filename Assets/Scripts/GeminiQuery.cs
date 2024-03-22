// using UnityEngine;
// using TMPro;
// using Firebase.Firestore;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine.UI; // Import UI namespace for dropdown

// public class FirestoreDropdownUpdater : MonoBehaviour
// {
//     public TMP_Text textField; // Optional for displaying the currently selected value
//     public Dropdown dropdown; // Reference to the Dropdown component
//     public string collectionName; // Firestore collection name
//     public string documentName; // Firestore document name
//     public string fieldName; // Firestore field name containing options

//     private void OnEnable()
//     {
//         if (dropdown != null)
//         {
//             dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
//         }
//     }

//     private void OnDisable()
//     {
//         if (dropdown != null)
//         {
//             dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
//         }
//     }

//     async void Start()
//     {
//         FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
//         DocumentReference docRef = db.Collection(collectionName).Document(documentName);

//         try
//         {
//             DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

//             if (snapshot.Exists)
//             {
//                 List<string> fieldValue = snapshot.GetValue<List<string>>(fieldName);

//                 if (fieldValue != null)
//                 {
//                     dropdown.ClearOptions(); // Clear any existing options

//                     List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
//                     foreach (string option in fieldValue)
//                     {
//                         options.Add(new Dropdown.OptionData(option));
//                     }

//                     dropdown.AddOptions(options);
//                 }
//                 else
//                 {
//                     Debug.LogError("Field value is null.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("Document does not exist!");
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("Error fetching document: " + ex.Message);
//         }
//     }

//    private void OnDropdownValueChanged(int index)
// {
//     if (textField != null)
//     {
//         textField.text = dropdown.options[index].text; // Use dropdown.options[index].text
//     }
// }

// }
