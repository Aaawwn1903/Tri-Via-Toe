using UnityEngine;
using Firebase;

public class FirebaseInitializer : MonoBehaviour
{
    void Start()
    {
        // Check and fix any missing dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            // If any dependencies were missing and were fixed, log it
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase missing dependencies were fixed.");
            }

            // Initialize Firebase
            FirebaseApp app = FirebaseApp.DefaultInstance;

            // Set up Realtime Database URL if needed
            // FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("YOUR_DATABASE_URL");

            Debug.Log("Firebase initialized successfully!");
        });
    }
}
