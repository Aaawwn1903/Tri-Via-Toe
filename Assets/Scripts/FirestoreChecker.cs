using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string clueAboveImageName;
    private string clueLeftImageName;
    private string dropdownText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetClueAboveImageName(string name)
    {
        clueAboveImageName = name;
        TrySendDataToFirestore();
    }

    public void SetClueLeftImageName(string name)
    {
        clueLeftImageName = name;
        TrySendDataToFirestore();
    }

    public void SetDropdownText(string text)
    {
        dropdownText = text;
        TrySendDataToFirestore();
    }

    private void TrySendDataToFirestore()
    {
        if (!string.IsNullOrEmpty(clueAboveImageName) && !string.IsNullOrEmpty(clueLeftImageName) && !string.IsNullOrEmpty(dropdownText))
        {
            FirestoreManager.Instance.SendDataToFirestore(clueAboveImageName, clueLeftImageName, dropdownText);
        }
    }
}

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }

    [Header("Firestore Configuration")]
    public string collectionName;
    public string documentName;

    private FirebaseFirestore db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    public void SendDataToFirestore(string clueAboveImageName, string clueLeftImageName, string dropdownText)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            {"ClueAboveImageName", clueAboveImageName},
            {"ClueLeftImageName", clueLeftImageName},
            {"DropdownText", dropdownText}
        };

        db.Collection(collectionName).Document(documentName).SetAsync(data)
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Data sent to Firestore successfully!");
                }
                else
                {
                    Debug.LogError($"Failed to send data to Firestore: {task.Exception}");
                }
            });
    }
}
