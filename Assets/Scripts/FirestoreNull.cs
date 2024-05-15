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
    [SerializeField] private Button[] answerButton;
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
    public GameObject surrenderConfirmationPanel;
    public GameObject forceTieConfirmationPanel;
    public Button confirmForceTieButton;
    public Button cancelForceTieButton;
    public Button confirmSurrenderButton;
    public Button cancelSurrenderButton;
    public Image noMatchImage;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float displayTime = 1f;

    [Header("Firestore Configuration")]
    public string collectionName = "Answer";

    private FirebaseFirestore db;
    private bool player1Turn = true;
    private int lastClickedButtonIndex = -1;
    public float timePerPlayer = 15f;
    private Coroutine timerCoroutine;
    private bool awaitingConfirmation = false;
    private Action confirmationAction;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        // submitButton.onClick.AddListener(SubmitData);
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
        forceTieButton.onClick.AddListener(() => ConfirmForceTie("Offer draw? Are you sure?"));
        surrenderButton.onClick.AddListener(() => ConfirmSurrender("Surrender to Player " + (player1Turn ? "2" : "1") + ". Are you sure?"));
        confirmForceTieButton.onClick.AddListener(ConfirmForceTieAction);
        cancelForceTieButton.onClick.AddListener(CancelForceTieAction);
        confirmSurrenderButton.onClick.AddListener(ConfirmSurrenderAction);
        cancelSurrenderButton.onClick.AddListener(CancelSurrenderAction);
        surrenderConfirmationPanel.SetActive(false);
        forceTieConfirmationPanel.SetActive(false);
        SetCurrentPlayerImage();
    }

    private void OnAnswerButtonClick(int buttonIndex)
    {
    string selectedAnswer = answerButton[buttonIndex].GetComponentInChildren<Text>().text;
    SubmitData(selectedAnswer);
    }


    private void OnButtonClick(int buttonIndex)
    {
        // Debug.Log("Button clicked: " + buttonIndex);
        lastClickedButtonIndex = buttonIndex;
        // ResetTimer();
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

    CheckValuesInFirestore(clueLeftImageName, clueAboveImageName, selectedAnswer);
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

        if (!foundMatch)
        {
            Debug.LogWarning("No match found in Firestore.");
            StartCoroutine(AnimateNoMatchImage());
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

            // Debug.Log("Time remaining: " + currentTime);
        }

        if (!CheckTie() && !CheckWinner())
        {
            // Time runs out, change player's turn
            ChangeButtonImage(false);
            mainCanvas.SetActive(false);
            forceTieConfirmationPanel.SetActive(false);
            surrenderConfirmationPanel.SetActive(false);
            ResetTimer();
        }
    }

    private void StartTimer()
    {
        StopTimer();
        timerCoroutine = StartCoroutine(Timer());

        // Set the color of playerTurn text based on the player's turn
        playerTurn.color = player1Turn ? new Color(245f / 255f, 77f / 255f, 98f / 255f) : new Color(135f / 255f, 228f / 255f, 58f / 255f);
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
            forceTieConfirmationPanel.SetActive(true);
            TextMeshProUGUI confirmationText = forceTieConfirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
            confirmationText.text = confirmationMessage;
            confirmationAction = ForceTie;
            awaitingConfirmation = true;
        }
    }

    private void ConfirmSurrender(string confirmationMessage)
    {
        if (!awaitingConfirmation)
        {
            surrenderConfirmationPanel.SetActive(true);
            TextMeshProUGUI confirmationText = surrenderConfirmationPanel.GetComponentInChildren<TextMeshProUGUI>();
            confirmationText.text = confirmationMessage;
            confirmationAction = Surrender;
            awaitingConfirmation = true;
        }
    }

    public void ConfirmForceTieAction()
    {
        if (confirmationAction != null)
        {
            confirmationAction();
            awaitingConfirmation = false;
            forceTieConfirmationPanel.SetActive(false);
        }
    }

    public void CancelForceTieAction()
    {
        awaitingConfirmation = false;
        forceTieConfirmationPanel.SetActive(false);
    }

    public void ConfirmSurrenderAction()
    {
        if (confirmationAction != null)
        {
            confirmationAction();
            awaitingConfirmation = false;
            surrenderConfirmationPanel.SetActive(false);
        }
    }

    public void CancelSurrenderAction()
    {
        awaitingConfirmation = false;
        surrenderConfirmationPanel.SetActive(false);
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

        // Wait for display time
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
