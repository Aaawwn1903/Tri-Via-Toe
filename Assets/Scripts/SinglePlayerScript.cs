using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

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
    public Image currentPlayerImage;
    public GameObject confirmationPanel;
    public Button confirmButton;
    public Button cancelButton;
    public Button finishButton;

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    private int lastClickedButtonIndex = -1;
    private int imageChangeCount = 0;
    private List<Tuple<Image, Image>> imagePairs = new List<Tuple<Image, Image>>();
    private Coroutine stopwatchCoroutine;
    private bool isGameFinished = false;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);
        finishButton.onClick.AddListener(FinishGame);

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }

        CreateImagePairs();
        StartStopwatch();
        SetCurrentPlayerImage();
    }

    private void OnButtonClick(int buttonIndex)
    {
        Debug.Log("Button clicked: " + buttonIndex);
        lastClickedButtonIndex = buttonIndex;
    }

    private void SubmitData()
    {
        if (lastClickedButtonIndex == -1)
        {
            Debug.LogWarning("No button has been clicked.");
            return;
        }

        if (isGameFinished)
        {
            Debug.LogWarning("Game has finished.");
            return;
        }

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;

        // Create a set of image pairs containing current clue images and the button images
        List<Tuple<Image, Image>> currentImagePairs = new List<Tuple<Image, Image>>();
        for (int i = 0; i < clueLeftImages.Length; i++)
        {
            currentImagePairs.Add(new Tuple<Image, Image>(clueLeftImages[i], clueAboveImages[i]));
        }
        currentImagePairs.Add(new Tuple<Image, Image>(buttons[lastClickedButtonIndex].GetComponent<Image>(), null));

        // Check if current image pairs match with Firestore data
        CheckValuesInFirestore(currentImagePairs, dropdownValue);
    }

    private async void CheckValuesInFirestore(List<Tuple<Image, Image>> imagePairs, string dropdownValue)
    {
        Debug.Log("Checking values in Firestore...");

        bool foundMatch = false;

        QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data = document.ToDictionary();
            string corrAnswer = data.ContainsKey("corr_answer") ? data["corr_answer"].ToString() : "";
            string horizontalChoice = data.ContainsKey("horizontal_choice") ? data["horizontal_choice"].ToString() : "";
            string verticalChoice = data.ContainsKey("vertical_choice") ? data["vertical_choice"].ToString() : "";

            if (imagePairs.Any(pair => string.Equals(pair.Item1.sprite?.name, verticalChoice, StringComparison.OrdinalIgnoreCase)) &&
                imagePairs.Any(pair => string.Equals(pair.Item2?.sprite?.name, horizontalChoice, StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(dropdownValue, corrAnswer, StringComparison.OrdinalIgnoreCase))
            {
                foundMatch = true;
                Debug.Log("Success! Values matched with document: " + document.Id);
                break;
            }
        }

        ChangeButtonImage(foundMatch);
    }

    private void CreateImagePairs()
    {
        for (int i = 0; i < clueLeftImages.Length; i++)
        {
            imagePairs.Add(new Tuple<Image, Image>(clueLeftImages[i], clueAboveImages[i]));
        }
    }

    private void ChangeButtonImage(bool foundMatch)
    {
        if (foundMatch)
        {
            // Increment the count of image changes
            imageChangeCount++;

            // Change the image of the last clicked button
            buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = imagesToReplace[imageChangeCount - 1];
            buttons[lastClickedButtonIndex].interactable = false;

            // Check if the finish condition is met
            if (imageChangeCount >= 5)
            {
                FinishGame();
            }
        }
    }

    private void SetCurrentPlayerImage()
    {
        currentPlayerImage.sprite = imagesToReplace[0]; // Set initial image
    }

    private void StartStopwatch()
    {
        stopwatchCoroutine = StartCoroutine(Stopwatch());
    }

    private void StopStopwatch()
    {
        if (stopwatchCoroutine != null)
        {
            StopCoroutine(stopwatchCoroutine);
        }
    }

    private IEnumerator Stopwatch()
    {
        while (!isGameFinished)
        {
            yield return null;
        }
    }

    private void FinishGame()
    {
        StopStopwatch();
        isGameFinished = true;
        gameOverPanel.SetActive(true);
        TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
        float completionPercentage = (float)imageChangeCount / 45f * 100f;
        gameOverText.text = "Completed: " + completionPercentage.ToString("F2") + "%";
    }
}
