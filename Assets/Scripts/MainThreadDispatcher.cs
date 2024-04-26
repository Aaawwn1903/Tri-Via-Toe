using UnityEngine;
using System;
using System.Collections.Generic;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> s_Actions = new Queue<Action>();
    private static MainThreadDispatcher s_Instance;

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject dispatcherObj = new GameObject("MainThreadDispatcher");
                s_Instance = dispatcherObj.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(dispatcherObj);
            }
            return s_Instance;
        }
    }

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject); // Ensures only one instance of MainThreadDispatcher exists
            return;
        }

        s_Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        lock (s_Actions)
        {
            while (s_Actions.Count > 0)
            {
                s_Actions.Dequeue()?.Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (s_Actions)
        {
            s_Actions.Enqueue(action);
        }
    }
}
