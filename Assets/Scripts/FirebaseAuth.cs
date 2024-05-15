using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    public static string UserId { get; private set; }

    private async void Start()
    {
        // Initialize Firebase Authentication
        auth = FirebaseAuth.DefaultInstance;

        // Initialize Firestore
        db = FirebaseFirestore.DefaultInstance;

        // Start the anonymous authentication process
        var authTask = auth.SignInAnonymouslyAsync();
        await authTask;

        if (authTask.IsCompleted && !authTask.IsCanceled && !authTask.IsFaulted)
        {
            // Retrieve the user ID
            UserId = authTask.Result.User.UserId;
            Debug.Log("Signed in with user ID: " + UserId);

            // Check if the user document already exists
            DocumentReference userDocRef = db.Collection("Users").Document(UserId);
            var userDocSnapshot = await userDocRef.GetSnapshotAsync();

            if (userDocSnapshot.Exists)
            {
                Debug.Log("User document already exists.");
            }
            else
            {
                // Create a new document for the user
                var data = new Dictionary<string, object>
                {
                    { "userId", UserId },
                    // Add other fields as needed
                };

                await userDocRef.SetAsync(data);
                Debug.Log("Created new user document for ID: " + UserId);
            }
        }
        else
        {
            Debug.LogError("Failed to sign in anonymously: " + authTask.Exception);
        }
    }
}
