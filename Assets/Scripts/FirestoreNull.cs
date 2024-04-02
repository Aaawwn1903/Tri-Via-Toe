using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class TicTacToeGame : MonoBehaviour
{
    public TMP_Dropdown searchResultDropdown;
    public Button submitButton;
    public Image[] clueAboveImages;
    public Image[] clueLeftImages;
    public Button[] buttons;
    public Sprite[] imagesToShowOnMatch;
    public Sprite[] imagesToReplace;
    public GameObject gameOverPanel;
    public GameObject mainCanvas; 
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerTurn;
    public Button forceTieButton;
    public Button surrenderButton;
    public Image currentPlayerImage;
    public GameObject confirmationPanel;
    public Button confirmButton;
    public Button cancelButton;
    

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

  

    private FirebaseFirestore db;
    private bool player1Turn = true;
    private int lastClickedButtonIndex = -1;
    private float timePerPlayer = 15f;
    private Coroutine timerCoroutine;
    private bool awaitingConfirmation = false;
    private Action confirmationAction;
    

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        submitButton.onClick.AddListener(SubmitData);

        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
        StartTimer();
        forceTieButton.onClick.AddListener(() => ConfirmForceTie("Offer draw? Are you sure?"));
        surrenderButton.onClick.AddListener(() => ConfirmSurrender("Surrender to Player " + (player1Turn ? "2" : "1") + ". Are you sure?"));
        confirmButton.onClick.AddListener(ConfirmAction);
        cancelButton.onClick.AddListener(CancelAction);
        confirmationPanel.SetActive(false);
        SetCurrentPlayerImage();
    }

    private void OnButtonClick(int buttonIndex)
    {
        Debug.Log("Button clicked: " + buttonIndex);
        lastClickedButtonIndex = buttonIndex;
        // ResetTimer();
    }

    private void SubmitData()
    {
        if (lastClickedButtonIndex == -1)
        {
            Debug.LogWarning("No button has been clicked.");
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
        Debug.Log("Checking values in Firestore...");

        bool foundMatch = false;

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
                foundMatch = true;
                Debug.Log("Success! Values matched with document: " + document.Id);
                break;
            }
        }

        ChangeButtonImage(foundMatch);
    }
    private void SetCurrentPlayerImage()
    {
        currentPlayerImage.sprite = player1Turn ? imagesToShowOnMatch[0] : imagesToReplace[0];
    }

    private void ChangeButtonImage(bool foundMatch)
{
    StopTimer();

    Sprite[] currentPlayerImages = player1Turn ? imagesToShowOnMatch : imagesToReplace;

    if (foundMatch)
    {
        // Reset the timer when a match is found
        ResetTimer();

        buttons[lastClickedButtonIndex].GetComponent<Image>().sprite = currentPlayerImages[0];
        buttons[lastClickedButtonIndex].interactable = false;

        if (CheckWinner())
        {
            Debug.Log("Player " + (player1Turn ? "1" : "2") + " wins!");
            ShowGameOverPanel("Player " + (player1Turn ? "1" : "2") + " wins!");
        }
        else if (CheckTie())
        {
            Debug.Log("Game Tied!");
            ShowGameOverPanel("Game Tied!");
        }

        player1Turn = !player1Turn;
        SetCurrentPlayerImage();
    }
    else
    {
        player1Turn = !player1Turn;
        SetCurrentPlayerImage();
    }

    StartTimer();
}

    private bool CheckWinner()
    {
        string currentPlayerImageName = player1Turn ? imagesToShowOnMatch[0].name : imagesToReplace[0].name;

        for (int row = 0; row < 3; row++)
        {
            if (buttons[row * 3].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[row * 3 + 1].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[row * 3 + 2].GetComponent<Image>().sprite?.name == currentPlayerImageName)
            {
                return true;
            }
        }

        for (int col = 0; col < 3; col++)
        {
            if (buttons[col].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[col + 3].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
                buttons[col + 6].GetComponent<Image>().sprite?.name == currentPlayerImageName)
            {
                return true;
            }
        }

        if ((buttons[0].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[4].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[8].GetComponent<Image>().sprite?.name == currentPlayerImageName) ||
            (buttons[2].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[4].GetComponent<Image>().sprite?.name == currentPlayerImageName &&
             buttons[6].GetComponent<Image>().sprite?.name == currentPlayerImageName))
        {
            return true;
        }

        return false;
    }

    private bool CheckTie()
    {
        bool allButtonsClicked = buttons.All(button => button.GetComponent<Image>().sprite != null && button.GetComponent<Image>().sprite.name != "UISprite");

        if (allButtonsClicked && !CheckWinner())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ForceTie()
{
    // Stop the timer
    StopTimer();

    // Check if the game is already tied or if a player has won
    if (!CheckTie() && !CheckWinner())
    {
        // If not, set the game state to tie
        ShowGameOverPanel("Game Tied!");
    }
}

private void Surrender()
{
    // Stop the timer
    StopTimer();

    // Declare the opponent as the winner
    if (player1Turn)
    {
        ShowGameOverPanel("Player 2 wins by surrender!");
    }
    else
    {
        ShowGameOverPanel("Player 1 wins by surrender!");
    }
}

    private void ShowGameOverPanel(string message)
{
    // Stop the timer coroutine
    StopTimer();
    
    // Show the game over panel
    gameOverPanel.SetActive(true);
    
    // Update the game over message
    TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
    gameOverText.text = message;
}


    private IEnumerator Timer()
{
    float currentTime = timePerPlayer;
    playerTurn.text = "PLAYER'S " + (player1Turn ? "1" : "2") + " TURNS: " + (player1Turn ? "X" : "O");
    
    while (currentTime > 0f)
    {
        // Check for game tied or player wins before decrementing time
        if (CheckTie() || CheckWinner())
        {
            StopTimer();
            break;
        }
        
        yield return new WaitForSeconds(1f);
        currentTime--;

        timerText.text = currentTime.ToString();

        Debug.Log("Time remaining: " + currentTime);
    }

    if (!CheckTie() && !CheckWinner())
    {
        // Time runs out, change player's turn
        ChangeButtonImage(false);
        mainCanvas.SetActive(false);
        confirmationPanel.SetActive(false);
        ResetTimer();
    }
}




    private void StartTimer()
    {
        StopTimer();
        timerCoroutine = StartCoroutine(Timer());
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    private void ResetTimer()
    {
        StopTimer();
        StartTimer();
    }
     private void ConfirmForceTie(string confirmationMessage)
{
    if (!awaitingConfirmation)
    {
        confirmationPanel.SetActive(true);
        TextMeshProUGUI confirmationText = confirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
        confirmationText.text = confirmationMessage;
        confirmationAction = ForceTie;
        awaitingConfirmation = true;
    }
}

private void ConfirmSurrender(string confirmationMessage)
{
    if (!awaitingConfirmation)
    {
        confirmationPanel.SetActive(true);
        TextMeshProUGUI confirmationText = confirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
        confirmationText.text = confirmationMessage;
        confirmationAction = Surrender;
        awaitingConfirmation = true;
    }
}


    private void ConfirmAction()
    {
        if (confirmationAction != null)
        {
            confirmationAction();
            awaitingConfirmation = false;
            confirmationPanel.SetActive(false);
        }
    }

    private void CancelAction()
    {
        awaitingConfirmation = false;
        confirmationPanel.SetActive(false);
    }
}
