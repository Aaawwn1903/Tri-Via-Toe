using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AndroidInputHandler : MonoBehaviour
{
    public TMP_InputField tmp_inputfield;

    void Update()
    {
        // Check if the device is an Android device
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if the keyboard input is not null or empty
            if (Input.inputString != null && Input.inputString != "")
            {
                // Append the keyboard input to the TMP_InputField text
                tmp_inputfield.text += Input.inputString;
            }
        }
    }
}
