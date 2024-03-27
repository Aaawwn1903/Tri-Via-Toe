using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class CombinedNull : MonoBehaviour
{
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Button[] buttons;
    public Sprite[] imagesToShowOnMatch; // Array of images to show when match is found
    public Sprite[] imagesToReplace; // Array of images to replace when match is not found

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    private int lastClickedButtonIndex = -1;

    private void Start()
    {
        // Initialize Firestore
        db = FirebaseFirestore.DefaultInstance;

        // Add listener to the submit button
        submitButton.onClick.AddListener(SubmitData);

        // Add listener to each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        Debug.Log("Button clicked: " + buttonIndex);
        lastClickedButtonIndex = buttonIndex; // Update last clicked button index
    }

    private void SubmitData()
    {
        if (lastClickedButtonIndex == -1)
        {
            Debug.LogWarning("No button has been clicked.");
            return;
        }

        string dropdownValue = searchResultDropdown.options[searchResultDropdown.value].text;

        // Get row and column based on the last clicked button index
        int row = lastClickedButtonIndex / 3;
        int col = lastClickedButtonIndex % 3;

        // Get name of the image from "Clue Left Image"
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";

        // Get name of the image from "Clue Above Image"
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";

        // Check the values against Firestore documents and change the button image accordingly
        CheckValuesInFirestore(clueLeftImageName, clueAboveImageName, dropdownValue);
    }

    private async void CheckValuesInFirestore(string clueLeftImageName, string clueAboveImageName, string dropdownValue)
    {
        Debug.Log("Checking values in Firestore...");

        bool foundMatch = false;

        // Query Firestore collection
        QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();

        // Iterate through each document
        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data = document.ToDictionary();
            string corrAnswer = data.ContainsKey("corr_answer") ? data["corr_answer"].ToString() : "";
            string horizontalChoice = data.ContainsKey("horizontal_choice") ? data["horizontal_choice"].ToString() : "";
            string verticalChoice = data.ContainsKey("vertical_choice") ? data["vertical_choice"].ToString() : "";

            // Compare values ignoring case sensitivity
            if (string.Equals(clueLeftImageName, verticalChoice, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(clueAboveImageName, horizontalChoice, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(dropdownValue, corrAnswer, StringComparison.OrdinalIgnoreCase))
            {
                foundMatch = true;
                Debug.Log("Success! Values matched with document: " + document.Id);
                break;
            }
        }

        // Change button image based on match
        ChangeButtonImage(foundMatch);
    }

    private void ChangeButtonImage(bool foundMatch)
{
    if (foundMatch)
    {
        // Get the current image index on the button
        int currentImageIndex = buttons[lastClickedButtonIndex].GetComponent<Image>().sprite != null ?
            Array.IndexOf(imagesToShowOnMatch, buttons[lastClickedButtonIndex].GetComponent<Image>().sprite) : -1;

        // Set the next image index
        int nextImageIndex = (currentImageIndex + 1) % imagesToShowOnMatch.Length;

        // Set the next image on the button
        buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = imagesToShowOnMatch[nextImageIndex];
    }
    else
    {
        // No need to change the button image, only switch the array
        // (Assuming you want to keep track of the current image index for the next attempt)
        // You can omit this part if you don't need to track the current image index for false match
        int currentImageIndex = buttons[lastClickedButtonIndex].GetComponent<Image>().sprite != null ?
            Array.IndexOf(imagesToReplace, buttons[lastClickedButtonIndex].GetComponent<Image>().sprite) : -1;

        // Update the current image index to the next image index
        int nextImageIndex = (currentImageIndex + 1) % imagesToReplace.Length;
    }
}

}
