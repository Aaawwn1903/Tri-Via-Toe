using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class SinglePlayer : MonoBehaviour
{
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Button[] buttons;
    public Sprite[] imagesToReplace;
    public GameObject gameOverPanel;
    public TextMeshProUGUI timerText;
    // public Image currentPlayerImage;
    public GameObject finishGamePanel;
    public TextMeshProUGUI finishGameText;
    public Button finishGameButton;

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    private int lastClickedButtonIndex = -1;
    private int imagesChangedCount = 0;
    private int maxImagesChanged = 5;
    private bool gameFinished = false;
    private readonly Stopwatch stopwatch = new();

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
        StartStopwatch();
    }

    private void OnButtonClick(int buttonIndex)
    {
        // Debug.Log("Button clicked: " + buttonIndex);
        if (!gameFinished)
        {
            lastClickedButtonIndex = buttonIndex;
            ChangeButtonImage();
        }
    }

    private void SubmitData()
    {
        if (lastClickedButtonIndex == -1)
        {
            // Debug.LogWarning("No button has been clicked.");
            return;
        }

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;
        int row = lastClickedButtonIndex / 3;
        int col = lastClickedButtonIndex % 3;
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";

        CheckValuesInFirestore(clueLeftImageName, clueAboveImageName, dropdownValue);
    }

    private async void CheckValuesInFirestore(string clueLeftImageName, string clueAboveImageName, string dropdownValue)
    {
        // Debug.Log("Checking values in Firestore...");

        QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data = document.ToDictionary();
            string corrAnswer = data.ContainsKey("corr_answer") ? data["corr_answer"].ToString() : "";
            string horizontalChoice = data.ContainsKey("horizontal_choice") ? data["horizontal_choice"].ToString() : "";
            string verticalChoice = data.ContainsKey("vertical_choice") ? data["vertical_choice"].ToString() : "";

            if (string.Equals(clueLeftImageName, verticalChoice, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(clueAboveImageName, horizontalChoice, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(dropdownValue, corrAnswer, StringComparison.OrdinalIgnoreCase))
            {
                // Debug.Log("Success! Values matched with document: " + document.Id);
                imagesChangedCount++;
                ChangeButtonImage();
                return;
            }
        }
    }

    private void ChangeButtonImage()
    {
        if (imagesChangedCount >= maxImagesChanged)
        {
            FinishGame();
            return;
        }

        if (lastClickedButtonIndex != -1 && buttons[lastClickedButtonIndex].interactable)
        {
            buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = imagesToReplace[imagesChangedCount];
            buttons[lastClickedButtonIndex].interactable = false;
        }
    }

    private void StartStopwatch()
    {
        stopwatch.Start();
    }

    private void FinishGame()
    {
        gameFinished = true;
        stopwatch.Stop();
        finishGamePanel.SetActive(true);
        double progressPercentage = (double)imagesChangedCount / maxImagesChanged * 100;
        finishGameText.text = "Completed: " + progressPercentage.ToString("F2") + "%";
    }
}
