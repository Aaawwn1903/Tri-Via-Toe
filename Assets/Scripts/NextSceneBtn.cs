using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneBtn : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
