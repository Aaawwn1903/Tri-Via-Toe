using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    private string tempScore;
    public TextMeshProUGUI highScoreText;
    void Start()
    {
        if (PlayerPrefs.HasKey("Score"))
{
    int savedScore = PlayerPrefs.GetInt("Score");
    Debug.Log("Saved Score: " + savedScore);
    tempScore = "High Score : " + savedScore;
    highScoreText.text = tempScore;
}
else
{
    Debug.Log("Score has not been set yet.");
}

    }
}
