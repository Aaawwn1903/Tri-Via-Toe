using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using Debug = UnityEngine.Debug;

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
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float displayTime = 1f;
    public float gameTime = 60f;
    public GameObject loadingAnimation; // Loading animation game object

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    public Image noMatchImage;
    public Image sameAnswerImage;

    private Dictionary<Button, int> buttonMatches = new Dictionary<Button, int>();
    private bool timerRunning = false; // Flag to control the timer coroutine

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);
        // endGameButton.onClick.AddListener(EndGame); // Register end game button click listener

        // Initialize buttonMatches dictionary
        foreach (var button in buttons)
        {
            buttonMatches.Add(button, 0);
        }

        StartCoroutine(StartGameTimer()); // Start the game timer
    }

    private IEnumerator StartGameTimer()
    {
        timerRunning = true; // Start the timer
        while (gameTime > 0 && timerRunning)
        {
            yield return new WaitForSeconds(1f);
            gameTime--;
            UpdateTimerDisplay();
        }
        if (timerRunning) // Ensure timerRunning is still true to prevent premature end game
            EndGame(); // End the game when time runs out or timer is stopped
    }

    private void UpdateTimerDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        timerText.text = string.Format("{0}", (int)timeSpan.TotalSeconds);
    }


    private async void SubmitData()
    {
        // Disable submit button
        submitButton.interactable = false;

        // Pause timer and show loading animation
        timerRunning = false;
        loadingAnimation.SetActive(true);

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;

        Debug.Log("Submitting data and checking values in Firestore...");    

        int documentsChecked = 0;
        int buttonsProcessed = 0;
        int buttonImagesChanged = 0; // Reset buttonImagesChanged to 0

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
                documentsChecked++; // Increment count for each document checked

                Dictionary<string, object> data = document.ToDictionary();
                string corrAnswer = data.ContainsKey("corr_answer") ? data["corr_answer"].ToString() : "";
                string horizontalChoice = data.ContainsKey("horizontal_choice") ? data["horizontal_choice"].ToString() : "";
                string verticalChoice = data.ContainsKey("vertical_choice") ? data["vertical_choice"].ToString() : "";

                if (string.Equals(clueLeftImageName, verticalChoice, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(clueAboveImageName, horizontalChoice, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(dropdownValue, corrAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Found match with document: " + document.Id);

                    // Change the image of the corresponding button
                    buttons[buttonIndex].GetComponent<Image>().sprite = imagesToShowOnMatch[0];
                    buttons[buttonIndex].interactable = false;
                    buttonImagesChanged++; // Increment buttonImagesChanged when a match is found
                    buttonMatches[button]++; // Increment the count of correct matches for this button
                }
            }

            buttonsProcessed++; // Increment the counter

            // Log the number of documents checked
            Debug.Log("Documents checked: " + documentsChecked);

            // Log the total number of button images changed after all buttons are checked
            if (buttonsProcessed == buttons.Length) // Check if all buttons are processed
            {
                Debug.Log("Button images changed: " + buttonImagesChanged);
                if (buttonImagesChanged == 0) // Check if only one button image is changed
                {
                    StartCoroutine(AnimateNoMatchImage());
                }
            }
        }

        // Check if any button has multiple matches and display noMatchImage accordingly
        foreach (var match in buttonMatches)
        {
            if (match.Value > 1)
            {
                StartCoroutine(AnimateSameAnswer());
                break;
            }
        }

        // Enable submit button and resume timer
        submitButton.interactable = true;
        loadingAnimation.SetActive(false);
        timerRunning = true; // Resume the timer
        StartCoroutine(StartGameTimer()); // Restart the timer
    }

    private IEnumerator AnimateNoMatchImage()
    {
        noMatchImage.gameObject.SetActive(true);

        // Fade in
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

        // Wait for display time only if the image is visible
        if (noMatchImage.gameObject.activeSelf)
            yield return new WaitForSeconds(displayTime);

        // Fade out
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

    private IEnumerator AnimateSameAnswer()
    {
        sameAnswerImage.gameObject.SetActive(true);

        // Fade in
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

        // Wait for display time only if the image is visible
        if (sameAnswerImage.gameObject.activeSelf)
            yield return new WaitForSeconds(displayTime);

        // Fade out
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
    }

    private void EndGame()
    {
        // Disable button and show game over panel
        submitButton.interactable = false;
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
        gameOverPanel.SetActive(true);
    }
}
