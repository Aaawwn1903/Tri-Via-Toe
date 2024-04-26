using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections;

public class DummySIngle : MonoBehaviour
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

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    // private int buttonImagesChanged = 0;
    private bool[] buttonsChecked; // Array to track checked buttons
    public float timePerPlayer = 15f;
    public Image noMatchImage;

    // New variables
    private int totalButtonsChanged = 0;
    private int[] individualButtonChanges; // Track individual button changes

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);
        buttonsChecked = new bool[buttons.Length]; // Initialize the array
        individualButtonChanges = new int[buttons.Length]; // Initialize individualButtonChanges array
    }

    private void SubmitData()
    {
        // Reset buttonImagesChanged count
        // buttonImagesChanged = 0;

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (!buttonsChecked[i]) // Check if button hasn't been checked before
            {
                int buttonIndex = i;
                int row = buttonIndex / 3;
                int col = buttonIndex % 3;
                string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
                string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";

                CheckValuesInFirestore(clueLeftImageName, clueAboveImageName, dropdownValue, buttonIndex, () =>
                {
                    buttonsChecked[buttonIndex] = true; // Mark the button as checked

                    if (individualButtonChanges[buttonIndex] > 0)
                    {
                        totalButtonsChanged++; // Increment total buttons changed
                    }
                });
            }
        }

        LogButtonImageChanges();
    }

    private async void CheckValuesInFirestore(string clueLeftImageName, string clueAboveImageName, string dropdownValue, int buttonIndex, Action onComplete)
    {
        Debug.Log("Checking values in Firestore...");

        int documentsChecked = 0;
        bool imageChanged = false; // Flag to track if the button's image has been changed

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

                imageChanged = true; // Set the flag to true
            }
        }

        // Log the number of documents checked
        Debug.Log("Documents checked: " + documentsChecked);

        if (imageChanged)
        {
            buttonsChecked[buttonIndex] = true; // Set buttonsChecked to true only if image has been changed
            individualButtonChanges[buttonIndex]++; // Increment individual button's change count
        }

        onComplete?.Invoke(); // Call the onComplete action
    }

    private void LogButtonImageChanges()
    {
        for (int i = 0; i < individualButtonChanges.Length; i++)
        {
            Debug.Log("Button " + i + " changes: " + individualButtonChanges[i]);
        }
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
}
