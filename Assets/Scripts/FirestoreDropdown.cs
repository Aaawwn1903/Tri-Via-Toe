using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;

public class FirestoreSearch : MonoBehaviour
{
    public TMP_InputField searchInput;
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;

    [Header("Firestore Configuration")]
    public string collectionName;
    public string documentName;
    public string fieldName;

    private FirebaseFirestore db;

    private void Start()
    {
        // Initialize Firestore
        db = FirebaseFirestore.DefaultInstance;

        // Add listener to the submit button
        submitButton.onClick.AddListener(PerformSearch);

        // Add listener to the dropdown
        searchResultDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(searchResultDropdown);
        });
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        Debug.Log("Dropdown selection: " + dropdown.options[dropdown.value].text);
    }

    public void PerformSearch()
    {
        string searchTerm = searchInput.text.ToLower(); // Convert search term to lowercase for case-insensitive search
        
        // Clear previous search results
        searchResultDropdown.ClearOptions();

        Debug.Log("Starting search...");

        // Query Firestore database
        db.Collection(collectionName).Document(documentName).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Search completed successfully.");

                DocumentSnapshot document = task.Result;
                if (document.Exists)
                {
                    Debug.Log("Document exists.");

                    Dictionary<string, object> data = document.ToDictionary();
                    if (data.ContainsKey(fieldName))
                    {
                        List<object> fieldArray = (List<object>)data[fieldName];
                        List<string> matchingElements = new List<string>();
                        foreach (object element in fieldArray)
                        {
                            string fieldValue = element.ToString().ToLower(); // Convert array element to lowercase
                            if (fieldValue.Contains(searchTerm))
                            {
                                Debug.Log("Search term found in array element: " + fieldValue);
                                matchingElements.Add(fieldValue);
                            }
                        }
                        DisplaySearchResults(matchingElements);
                    }
                    else
                    {
                        Debug.LogWarning($"Field '{fieldName}' not found in document.");
                    }
                }
                else
                {
                    Debug.LogError($"Document '{documentName}' does not exist in collection '{collectionName}'");
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch document '{documentName}' with: {task.Exception}");
            }
        });
    }

    private void DisplaySearchResults(List<string> results)
    {
        searchResultDropdown.ClearOptions();
        searchResultDropdown.AddOptions(results);
    }
}
