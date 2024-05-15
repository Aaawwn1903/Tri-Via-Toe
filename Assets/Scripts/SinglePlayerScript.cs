using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;

public class SinglePlayer : MonoBehaviour
{
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Button[] buttons;
    public Sprite[] imagesToShowOnMatch;
    public GameObject gameOverPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameScore;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float displayTime = 1f;
    public float gameTime = 60f;
    private int score = 0;
    private bool isAnimating = false;
    public GameObject loadingAnimation;

    [Header("Firestore Configuration")]
    public string collectionName = "Users";

    private FirebaseFirestore db;
    public Image noMatchImage;
    public Image sameAnswerImage;

    private Dictionary<Button, int> buttonMatches = new Dictionary<Button, int>();
    private bool timerRunning = false;
    private string userId;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);
        StartCoroutine(StartGameTimer());
        userId = FirebaseAuthManager.UserId;
        Debug.LogWarning("User ID : " + userId);
        LoadUserScore();
         foreach (var button in buttons)
        {
            buttonMatches.Add(button, 0);
        }
    }

   private async void LoadUserScore()
{
    DocumentReference userDocRef = db.Collection(collectionName).Document(userId);
    DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();
    
    if (snapshot.Exists)
    {
        Dictionary<string, object> data = snapshot.ToDictionary();
        score = Convert.ToInt32(data["score"]);
        UpdateScoreDisplay();
        Debug.LogWarning("Default Score ; " + score);
    }
    else
    {
        // Create a new document with initial score data
        Dictionary<string, object> initialData = new Dictionary<string, object>
        {
            { "score", 0 }
        };
        
        // Set the initial data for the user's document
        await userDocRef.SetAsync(initialData);
        
        // Update the score variable with the initial score
        score = 0;
        Debug.LogWarning("Default Score ; " + score);
        // Update the score display
        UpdateScoreDisplay();
    }
}


    private async void SubmitData()
    {
        submitButton.interactable = false;
        timerRunning = false;
        loadingAnimation.SetActive(true);

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;

        Debug.Log("Submitting data and checking values in Firestore...");

        await CheckButtonMatches(dropdownValue);

        bool allButtonsMatched = buttonMatches.All(pair => pair.Value >= 1);
        if (allButtonsMatched)
        {
            loadingAnimation.SetActive(false);
            EndGame();
            return;
        }

        submitButton.interactable = true;
        loadingAnimation.SetActive(false);
        timerRunning = true;
        StartCoroutine(StartGameTimer());
    }

    private async Task CheckButtonMatches(string dropdownValue)
    {
        int documentsChecked = 0;
        int buttonsProcessed = 0;
        int buttonImagesChanged = 0;
        string buttonName;

        foreach (var button in buttons)
        {
            int buttonIndex = Array.IndexOf(buttons, button);
            int row = buttonIndex / 3;
            int col = buttonIndex % 3;
            string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
            string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";

            QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                documentsChecked++;

                Dictionary<string, object> data = document.ToDictionary();
                string corrAnswer = data.ContainsKey("corr_answer") ? data["corr_answer"].ToString() : "";
                string horizontalChoice = data.ContainsKey("horizontal_choice") ? data["horizontal_choice"].ToString() : "";
                string verticalChoice = data.ContainsKey("vertical_choice") ? data["vertical_choice"].ToString() : "";

                if (string.Equals(clueLeftImageName, verticalChoice, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(clueAboveImageName, horizontalChoice, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(dropdownValue, corrAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Found match with document: " + document.Id);
                    UnityEngine.Sprite sprite = buttons[buttonIndex].GetComponent<Image>().sprite;
                    buttonName = sprite.name;
                    if (buttonName == "CrossBoard")
                    {
                        StartCoroutine(AnimateSameAnswer(buttonIndex));
                    }
                    else if (buttons[buttonIndex].GetComponent<Image>().sprite != null && buttonName != "CrossBoard")
                    {
                        buttons[buttonIndex].GetComponent<Image>().sprite = imagesToShowOnMatch[0];
                        buttons[buttonIndex].interactable = false;
                        buttonImagesChanged++;
                        buttonMatches[button]++;
                    }
                }
            }

            buttonsProcessed++;

            Debug.Log("Documents checked: " + documentsChecked);

            if (buttonsProcessed == buttons.Length)
            {
                Debug.Log("Button images changed: " + buttonImagesChanged);
                if (buttonImagesChanged == 0)
                {
                    StartCoroutine(AnimateNoMatchImage());
                }
            }
        }
    }

    private void UpdateScoreDisplay()
    {
        gameScore.text = "Game score = " + score;
    }

    private async void EndGame()
    {
        foreach (var matchCount in buttonMatches.Values)
        {
            score += matchCount * 10;
        }

        score += (int)(gameTime * 2);

        DocumentReference userDocRef = db.Collection(collectionName).Document(userId);
        DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
            int currentScore = Convert.ToInt32(data["score"]);
            if (score > currentScore)
            {
                Dictionary<string, object> newData = new Dictionary<string, object>
                {
                    { "score", score }
                };
                await userDocRef.SetAsync(newData, SetOptions.MergeAll);
            }
        }

        Debug.LogWarning("After game Score ; " + score);
        UpdateScoreDisplay();

        submitButton.interactable = false;
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
        gameOverPanel.SetActive(true);
    }

    private IEnumerator StartGameTimer()
    {
        timerRunning = true;
        while (gameTime > 0 && timerRunning)
        {
            yield return new WaitForSeconds(1f);
            gameTime--;
            UpdateTimerDisplay();
        }
        if (timerRunning)
            EndGame();
    }

    private void UpdateTimerDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        timerText.text = string.Format("{0}", (int)timeSpan.TotalSeconds);
    }

    private IEnumerator AnimateNoMatchImage()
    {
        if (isAnimating)
        {
            yield break;
        }

        isAnimating = true;
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

        if (noMatchImage.gameObject.activeSelf)
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
        isAnimating = false;
    }

    private IEnumerator AnimateSameAnswer(int buttonIndex)
    {
        Dictionary<int, float> buttonIndexToWaitTime = new Dictionary<int, float>
        {
            { 0, 3.3f },
            { 1, 3.0f },
            { 2, 2.6f },
            { 3, 2.3f },
            { 4, 2f },
            { 5, 1.6f },
            { 6, 1.3f },
            { 7, 1f },
            { 8, 0.6f },
        };

        if (buttonIndex >= 0 && buttonIndexToWaitTime.ContainsKey(buttonIndex))
        {
            yield return new WaitForSeconds(buttonIndexToWaitTime[buttonIndex]);
        }

        if (isAnimating)
        {
            yield break;
        }

        isAnimating = true;
        sameAnswerImage.gameObject.SetActive(true);

        float timer = 0f;
        Color startColor = sameAnswerImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            sameAnswerImage.color = Color.Lerp(startColor, targetColor, timer / fadeInTime);
            yield return null;
        }
        sameAnswerImage.color = targetColor;

        if (sameAnswerImage.gameObject.activeSelf)
            yield return new WaitForSeconds(displayTime);

        timer = 0f;
        startColor = sameAnswerImage.color;
        targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            sameAnswerImage.color = Color.Lerp(startColor, targetColor, timer / fadeOutTime);
            yield return null;
        }
        sameAnswerImage.color = targetColor;

        sameAnswerImage.gameObject.SetActive(false);
        isAnimating = false;
    }
}
