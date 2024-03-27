using UnityEngine;
using TMPro;

public class TextUISizeChanger : MonoBehaviour
{
    public TMP_Text textUI;
    public float changeInterval = 1f;
    private bool increaseSize = true;

    private void Start()
    {
        if (textUI == null)
        {
            Debug.LogError("Text UI reference is not set!");
            enabled = false; // Disable the script if the reference is not set
            return;
        }

        InvokeRepeating("ChangeFontSize", 0f, changeInterval);
    }

    private void ChangeFontSize()
    {
        if (increaseSize)
        {
            textUI.fontSize++;
            if (textUI.fontSize >= 75)
                increaseSize = false;
        }
        else
        {
            textUI.fontSize--;
            if (textUI.fontSize <= 74)
                increaseSize = true;
        }
    }
}
