using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;

public class CombinedScript : MonoBehaviour
{
    // TicTacToe variables
    [SerializeField] private Image[] clueAboveImages;
    [SerializeField] private Image[] clueLeftImages;
    [SerializeField] private Button[] buttons;

    // FirestoreSearch variables
    public TMP_InputField searchInput;
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public string collectionName;
    public string documentName;
    public string fieldName;
    private FirebaseFirestore db;

    private void Start()
    {
        // TicTacToe initialization
        InitializeTicTacToe();

        // FirestoreSearch initialization
        InitializeFirestoreSearch();

    }

    private void InitializeTicTacToe()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        // Debug.Log("Button clicked: " + buttonIndex);
        Image clickedButtonImage = buttons[buttonIndex].GetComponent<Image>();
        int row = buttonIndex / 3;
        int col = buttonIndex % 3;
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
        Debug.Log("Clue kiri untuk baris " + row + ": " + clueLeftImageName);
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";
        Debug.Log("Clue atas untuk kolom " + col + ": " + clueAboveImageName);
    }

    private void InitializeFirestoreSearch()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(PerformSearch);
        searchResultDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(searchResultDropdown);
        });
    }

private void DropdownValueChanged(TMP_Dropdown dropdown)
{
    Debug.Log("Dropdown selection: " + dropdown.options[dropdown.value].text);
    CombineAndLogValues();
}

private void CombineAndLogValues()
{
    int buttonIndex = 0; // You need to define the button index here based on your logic
    int row = buttonIndex / 3;
    int col = buttonIndex % 3;
    string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
    string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";
    string dropdownText = searchResultDropdown.options[searchResultDropdown.value].text;

    // Combining the three values
    string combinedValues = $"Combined values: ClueLeft={clueLeftImageName}, ClueAbove={clueAboveImageName}, DropdownText={dropdownText}";
    Debug.Log(combinedValues);
}


    public void PerformSearch()
    {
        string searchTerm = searchInput.text.ToLower();
        searchResultDropdown.ClearOptions();
        // Debug.Log("Starting search...");
        db.Collection(collectionName).Document(documentName).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // Debug.Log("Search completed successfully.");
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
                            string fieldValue = element.ToString().ToLower();
                            if (fieldValue.Contains(searchTerm))
                            {
                                // Debug.Log("Search term found in array element: " + fieldValue);
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
