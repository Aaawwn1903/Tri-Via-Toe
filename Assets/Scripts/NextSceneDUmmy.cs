using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneDummy : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("DummyScene");
    }
}
