using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneVersus : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("VersusGame");
    }
}
