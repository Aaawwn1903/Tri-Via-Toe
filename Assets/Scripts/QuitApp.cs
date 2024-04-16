using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButtonn : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}