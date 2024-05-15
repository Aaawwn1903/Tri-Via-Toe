using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MultipleChoice : MonoBehaviour
{
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

    [SerializeField] private Image[] clueAboveImages;
    [SerializeField] private Image[] clueLeftImages;
    [SerializeField] private Button[] buttons;
    public Text textHint;
    [SerializeField] private Text[] Option;
    [SerializeField] private TextAsset jsonData; // Assign your JSON file here in the Inspector

    private QuizDataList quizDataList;

    private void Start()
    {
        InitializeTicTacToe();
        LoadDataFromJSON();
        ShuffleOptions(Option);
    }

    private void InitializeTicTacToe()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }

    private void LoadDataFromJSON()
    {
        if (jsonData != null)
        {
            quizDataList = JsonUtility.FromJson<QuizDataList>(jsonData.text);
        }
        else
        {
            Debug.LogError("JSON data is not assigned!");
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        Image clickedButtonImage = buttons[buttonIndex].GetComponent<Image>();
        int row = buttonIndex / 3;
        int col = buttonIndex % 3;
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";
        textHint.text = "Pilih jawaban yang benar untuk " + clueLeftImageName +  " dan " + clueAboveImageName;
        PerformSearch(clueLeftImageName, clueAboveImageName);
        DebugLogRandomAnswer(clueLeftImageName, clueAboveImageName);
    }

    public void PerformSearch(string clueLeftImageName, string clueAboveImageName)
    {
        if (quizDataList == null || quizDataList.data == null || quizDataList.data.Count == 0)
        {
            Debug.LogWarning("No quiz data loaded!");
            return;
        }

        List<QuizData> filteredData = quizDataList.data.Where(data => data.vertical_choice == clueLeftImageName && data.horizontal_choice == clueAboveImageName).ToList();

        if (filteredData.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredData.Count);
            Option[0].text = filteredData[randomIndex].corr_answer;
            Debug.Log("Correct Answer: " + filteredData[randomIndex].corr_answer);
        }
        else
        {
            Debug.LogWarning("No data found for the given clues!");
        }
    }

    public void DebugLogRandomAnswer(string clueLeftImageName, string clueAboveImageName)
    {
        if (quizDataList == null || quizDataList.data == null || quizDataList.data.Count == 0)
        {
            Debug.LogWarning("No quiz data loaded!");
            return;
        }

        List<QuizData> filteredData = quizDataList.data.Where(data => data.horizontal_choice == clueAboveImageName && data.vertical_choice != clueLeftImageName).ToList();

        if (filteredData.Count > 0)
        {
            Shuffle(filteredData);

            int countToShow = Mathf.Min(3, filteredData.Count);
            for (int i = 0; i < countToShow; i++)
            {
                Option[i + 1].text = filteredData[i].corr_answer;
                Debug.LogWarning("Incorrect Answer: " + filteredData[i].corr_answer);
            }
            ShuffleOptions(Option);
        }
        else
        {
            Debug.LogWarning("No data found for the given clues or all data is filtered out!");
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void ShuffleOptions(Text[] options)
    {
        int n = options.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Text value = options[k];
            options[k] = options[n];
            options[n] = value;
        }
    }
}
