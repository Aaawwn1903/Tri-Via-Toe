using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // This variable will be set in the Unity Inspector
    public string nextSceneName;

    // This method can be called to load the next scene
    public void LoadNextScene()
    {
        // Check if the nextSceneName is not empty
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Load the scene with the specified name
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set in the inspector.");
        }
    }
}