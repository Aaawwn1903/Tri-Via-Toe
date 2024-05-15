using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class SingleJsonDropDown : MonoBehaviour
{
    // TicTacToe variables
    [SerializeField] private Image[] clueAboveImages;
    [SerializeField] private Image[] clueLeftImages;
    [SerializeField] private Button[] buttons;

    // JSON variables
    public TMP_InputField searchInput;
    public TMP_Dropdown searchResultDropdown;
    public TextAsset jsonData; // JSON file containing the data
    public string fieldName;

    private List<QuizData> quizDataList;

    private void Start()
    {
        // TicTacToe initialization
        InitializeTicTacToe();

        // JSON initialization
        LoadDataFromJSON();
        InitializeJSONSearch();
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

    private void LoadDataFromJSON()
    {
        if (jsonData != null)
        {
            quizDataList = JsonUtility.FromJson<QuizDataList>(jsonData.text).data;
        }
        else
        {
            Debug.LogError("JSON data file is not assigned!");
        }
    }

    private void InitializeJSONSearch()
    {
        searchInput.onValueChanged.AddListener(delegate {
            if (searchInput.text.Length >= 3)
                PerformSearch();
                
        });
    }

    public void PerformSearch()
    {
        string searchTerm = searchInput.text.ToLower();
        List<string> matchingElements = new List<string>();

        foreach (var quizData in quizDataList)
        {
            string fieldValue = quizData.vertical_choice.ToLower();
            if (fieldValue.Contains(searchTerm))
            {
                matchingElements.Add(fieldValue);
            }
        }

        DisplaySearchResults(matchingElements);
    }

    private void DisplaySearchResults(List<string> results)
    {
        searchResultDropdown.ClearOptions();
        searchResultDropdown.AddOptions(results);
    }

    [System.Serializable]
    public class QuizData
    {
        public string vertical_choice;
        public string horizontal_choice;
        public string corr_answer;
    }

    [System.Serializable]
    public class QuizDataList
    {
        public List<QuizData> data;
    }
}
