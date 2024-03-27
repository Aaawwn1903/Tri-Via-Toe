using UnityEngine;
// using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
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
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
            }
            else
            {
                Debug.Log("No previous scene to go back to.");
            }
        }
    }
}