using UnityEngine;
using System.Collections;

public class YourScript : MonoBehaviour
{
    public GameObject yourGameObject;

    private void Start()
    {
        StartCoroutine(ActivateGameObjectAsync(yourGameObject, true)); // Activates the GameObject
    }

    IEnumerator ActivateGameObjectAsync(GameObject gameObjectToActivate, bool activate)
    {
        AsyncOperation asyncOperation = null;

        if (activate)
        {
            asyncOperation = Resources.LoadAsync<GameObject>(gameObjectToActivate.name);
            asyncOperation.completed += operation => gameObjectToActivate.SetActive(true);
        }
        else
        {
            gameObjectToActivate.SetActive(false);
        }

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
