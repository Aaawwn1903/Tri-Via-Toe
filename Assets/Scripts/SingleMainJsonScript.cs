using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using static JsonLoader;

public class MainSingleJson : MonoBehaviour
{
    [SerializeField] private Button[] answerButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Sprite[] clueRightImages;
    public Button[] buttons;
    public Sprite[] imagesToShowOnMatch;
    public Sprite[] imagesToReplace;
    public GameObject gameOverPanel;
    // public Button addHealthBar;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI healthLeft;
    public TextMeshProUGUI healthRight;
    public Text progres;
    public Image noMatchImage;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float displayTime = 1f;
    public int healthBar = 3;
    public Image healthBarIMG;
    [Header("JSON Configuration")]
    public TextAsset jsonData;

    private QuizDataList quizDataList;
    private int lastClickedButtonIndex = -1;
    public float timePerPlayer = 15f;
    private Coroutine timerCoroutine;
    private float currentTime;
    private const string HighScoreKey = "HighScore";

    private void Start()
    {
        LoadDataFromJSON();
        // addHealthBar.onClick.AddListener(addHealthBarMethod);
        
        for (int i = 0; i < answerButton.Length; i++)
        {
             int buttonIndex = i;
             answerButton[i].onClick.AddListener(() => OnAnswerButtonClick(buttonIndex));
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
        StartTimer();
        

        // healthBarIMG.GetComponent<Image>().sprite = clueRightImages[0];
        
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

    private void OnAnswerButtonClick(int buttonIndex)
    {
        string selectedAnswer = answerButton[buttonIndex].GetComponentInChildren<Text>().text;
        SubmitData(selectedAnswer);
    }

    private void OnButtonClick(int buttonIndex)
    {
        lastClickedButtonIndex = buttonIndex;
    }

    private void SubmitData(string selectedAnswer)
    {
        if (lastClickedButtonIndex == -1)
        {
            Debug.LogWarning("No button has been clicked.");
            return;
        }

        int row = lastClickedButtonIndex / 3;
        int col = lastClickedButtonIndex % 3;
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";
        
        CheckValuesInJSON(clueLeftImageName, clueAboveImageName, selectedAnswer);
    }

    private void CheckValuesInJSON(string clueLeftImageName, string clueAboveImageName, string selectedAnswer)
    {
        // Debug.Log("Checking values in JSON...");

        bool foundMatch = false;
        
        if (quizDataList != null && quizDataList.data != null)
        {
            foreach (var data in quizDataList.data)
            {
                if (string.Equals(data.vertical_choice, clueLeftImageName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(data.horizontal_choice, clueAboveImageName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(data.corr_answer, selectedAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    foundMatch = true;
                    Debug.Log("Success! Values matched with data.");
                    break;
                }
            }
        }

        if (!foundMatch)
        {
            Debug.LogWarning("No match found in JSON.");
            StartCoroutine(AnimateNoMatchImage());
            healthBarMethod();
        }

        ChangeButtonImage(foundMatch);
    }

    private void ChangeButtonImage(bool foundMatch)
    {

        Sprite[] currentPlayerImages = imagesToShowOnMatch;

        if (foundMatch)
        {
            buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = currentPlayerImages[0];
            buttons[lastClickedButtonIndex].interactable = false;
            CheckGameOver();
            UpdatePercentage();
        }
        
    }

    private void UpdatePercentage()
    {
    int totalButtons = buttons.Length;
    int changedButtonsCount = buttons.Count(button => !button.interactable);
    float percentage = (float)changedButtonsCount / totalButtons * 100f;
    progres.text = $"Progress: {percentage:F2}%"; // Display the percentage with 2 decimal places
    }
    private void CheckGameOver()
    {
        bool allButtonsChanged = buttons.All(button => !button.interactable);
        if (allButtonsChanged)
        {
            ShowGameOverPanel("Game Over!\nYou Won!");
        }
    }

    private void ShowGameOverPanel(string message)
{
    if (message == "Game Over!\nYou lost all health!")
    {
        // addHealthBar.gameObject.SetActive(true);
        Debug.Log("add healthbar will be available soon!!");
        currentTime = 0; // Set the time to zero if the game ends due to health reaching zero
    }

    bool healthZeroGameOver = message == "Game Over!\nYou lost all health!";
    noMatchImage.gameObject.SetActive(false);
    StopTimer();
    int changedButtonsCount = buttons.Count(button => !button.interactable);
    int score;
    if (healthZeroGameOver) {
        score = changedButtonsCount * 12; // Only count the button interactions for score
    } else {
        score = changedButtonsCount * 12 + (int)currentTime * 3; // Include time component for score
    }

    SaveHighScore(score);
    gameOverPanel.SetActive(true);
    TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
    gameOverText.text = $"Your Score: {score}\nHigh Score: {GetHighScore()}";
    healthRight.text = $"{message}";
}



    private IEnumerator Timer()
    {
        currentTime = timePerPlayer;

        while (currentTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            currentTime--;

            timerText.text = currentTime.ToString();
        }

        ShowGameOverPanel("Game Over!\nTime's up!");
    }

    private void StartTimer()
    {
        timerCoroutine = StartCoroutine(Timer());
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    private void SaveHighScore(int newScore)
    {
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (newScore > highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, newScore);
        }
    }

    private int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private IEnumerator AnimateNoMatchImage()
    {
            noMatchImage.gameObject.SetActive(true);

            float timer = 0f;
            Color startColor = noMatchImage.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
            while (timer < fadeInTime)
            {
                timer += Time.deltaTime;
                noMatchImage.color = Color.Lerp(startColor, targetColor, timer / fadeInTime);
                yield return null;
            }
            noMatchImage.color = targetColor;

            yield return new WaitForSeconds(displayTime);

            timer = 0f;
            startColor = noMatchImage.color;
            targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            while (timer < fadeOutTime)
            {
                timer += Time.deltaTime;
                noMatchImage.color = Color.Lerp(startColor, targetColor, timer / fadeOutTime);
                yield return null;
            }
            noMatchImage.color = targetColor;

            noMatchImage.gameObject.SetActive(false);
        }
    private void healthBarMethod()
        {
        healthBar--;
        healthLeft.text = "Health Left : " + healthBar.ToString();
        if (healthBar == 2)
        {
            healthBarIMG.GetComponent<Image>().sprite = clueRightImages[0];
        }
        else if (healthBar == 1)
        {
            healthBarIMG.GetComponent<Image>().sprite = clueRightImages[1];
        }
        else 
        {
            healthBarIMG.GetComponent<Image>().sprite = clueRightImages[2];
            ShowGameOverPanel("Game Over!\nYou lost all health!");
        }

    }
    // private int addHealthBarMethod()
    // {
    //     healthBar ++;
    //     return healthBar;
    // }
        }

    

