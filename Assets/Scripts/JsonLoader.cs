using System.Collections.Generic;
using UnityEngine;

public class JsonLoader : MonoBehaviour
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
    public TextAsset jsonFile; // Drag and drop your JSON file into this field in the Unity Inspector

    private QuizDataList quizDataList;

    void Start()
    {
        if (jsonFile != null)
        {
            LoadDataFromJSON();
            DebugLogRandomData();
        }
        else
        {
            Debug.LogError("JSON file is not assigned!");
        }
    }

    void LoadDataFromJSON()
    {
        quizDataList = JsonUtility.FromJson<QuizDataList>(jsonFile.text);
    }

    void DebugLogRandomData()
    {
        if (quizDataList != null && quizDataList.data != null && quizDataList.data.Count > 0)
        {
            int randomIndex = Random.Range(0, quizDataList.data.Count);
            QuizData randomData = quizDataList.data[randomIndex];
            Debug.Log("Random Quiz Data - Vertical Choice: " + randomData.vertical_choice +
                      ", Horizontal Choice: " + randomData.horizontal_choice +
                      ", Correct Answer: " + randomData.corr_answer);
        }
        else
        {
            Debug.LogWarning("No data loaded or empty array!");
        }
    }
}
