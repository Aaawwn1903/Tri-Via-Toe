using UnityEngine;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private FirebaseFirestore db;
    private string scoreText;
    public TextMeshProUGUI highScore; 

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        // Example: Retrieve the highest score when the game starts
        RetrieveHighestScore();
    }

    private async void RetrieveHighestScore()
    {
        int highestScore = int.MinValue; // Initialize with the lowest possible score

        try
        {
            // Query the Firestore collection "Scores"
            QuerySnapshot snapshot = await db.Collection("Scores").GetSnapshotAsync();

            // Process each document in the snapshot
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // Convert the document data to a dictionary
                Dictionary<string, object> data = document.ToDictionary();

                // Extract score from the document
                if (data.ContainsKey("score"))
                {
                    int score = Convert.ToInt32(data["score"]);

                    // Update the highest score if the current score is higher
                    if (score > highestScore)
                    {
                        highestScore = score;
                    }
                }
            }

            // Once all scores are processed, you have the highest score
            if (highestScore != int.MinValue)
            {
                // Process the highest score as needed, e.g., display it in the UI
                Debug.Log("Highest Score: " + highestScore);
                scoreText = "High Score : " + highestScore.ToString();
                highScore.text = scoreText;
            }
            else
            {
                // Handle case where no scores are found
                Debug.Log("No scores found in Firestore.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error retrieving highest score from Firestore: " + e);
        }
    }
}
