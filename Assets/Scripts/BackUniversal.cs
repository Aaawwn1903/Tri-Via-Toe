using UnityEngine;
// using UnityEngine.SceneManagement;

public class BackMenuHandler : MonoBehaviour

{
    public string nextSceneName;
    // Update is called once per frame
    void Update()
    {
        // Check if the user has pressed the back button on their device
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Check if there is a previous scene in the build settings
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex > 0)
            {
                // Load the previous scene
               LoadNextScene();
            }
            else
            {
                Debug.Log("No previous scene to go back to.");
            }
        }
    }


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
