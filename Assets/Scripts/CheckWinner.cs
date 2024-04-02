using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

public class TicTacToeWinner : MonoBehaviour
{
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Button[] buttons;
    public Sprite[] imagesToShowOnMatch; // Array of images for Player 1
    public Sprite[] imagesToReplace; // Array of images for Player 2
    public TMP_Text textPanel;

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    public GameObject Panel;
    private string currentPlayer;
    private FirebaseFirestore db;
    private bool player1Turn = true; // Flag to track player's turn
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

        // Change button image and switch player's turn based on match
        ChangeButtonImage(foundMatch);

        // Check for winner or tie
        if (CheckWinner())
        {
            Debug.Log("Player " + (player1Turn ? "1" : "2") + " wins!");
        }
        else if (CheckTie())
        {
            // Debug.Log("It's a tie!");
            // textPanel.text = "Game Tied";
            // Panel.SetActive(true);
        }
    }

    private void ChangeButtonImage(bool foundMatch)
    {
        // Get the current player's images based on turn
        Sprite[] currentPlayerImages = player1Turn ? imagesToShowOnMatch : imagesToReplace;

        if (foundMatch)
        {
            // Set the image for the current player
            buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = currentPlayerImages[0];

            // Disable the button after correct answer
            buttons[lastClickedButtonIndex].interactable = false;
            
            // Switch to the next player's turn
            player1Turn = !player1Turn;
        }
        else
        {
            // Switch to the next player's turn without changing the button image
            player1Turn = !player1Turn;
        }
    }

    private bool CheckWinner()
    {
        string currentPlayerImageName = player1Turn ? imagesToShowOnMatch[0].name : imagesToReplace[0].name;

        // Check horizontal rows
        for (int row = 0; row < 3; row++)
        {
            if (buttons[row * 3].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[row * 3 + 1].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[row * 3 + 2].GetComponent<Image>().sprite?.name == currentPlayerImageName)
            {
                // Three consecutive symbols found in a row, return true
                return true;
            }
        }

        // Check vertical columns
        for (int col = 0; col < 3; col++)
        {
            if (buttons[col].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[col + 3].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[col + 6].GetComponent<Image>().sprite?.name == currentPlayerImageName)
            {
                // Three consecutive symbols found in a column, return true
                return true;
            }
        }

        // Check diagonals
        if ((buttons[0].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[4].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[8].GetComponent<Image>().sprite?.name == currentPlayerImageName) ||
            (buttons[2].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[4].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[6].GetComponent<Image>().sprite?.name == currentPlayerImageName))
        {
            // Three consecutive symbols found in a diagonal, return true
            return true;
        }

        // No winner found
        return false;
    }

    private bool CheckTie()
{
    // Check if there is a winner first
    if (CheckWinner())
    {
        return false; // If there's a winner, the game is not tied
    }

    // If there's no winner, then check if all buttons are filled
    foreach (Button button in buttons)
    {
        if (button.GetComponent<Image>().sprite == null)
        {
            // There is an empty button, game is not tied
            return false;
        }
    }
    // All buttons are filled and there's no winner, game is tied
    return true;
}

}
