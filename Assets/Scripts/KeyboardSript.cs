using UnityEngine;

public class MoveUpWhenKeyboard : MonoBehaviour
{
    private Vector3 originalPosition;
    private TouchScreenKeyboard keyboard;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Check if the mobile keyboard is active
        if (TouchScreenKeyboard.visible && keyboard == null)
        {
            // Keyboard just became active
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            // Move the GameObject up (adjust as necessary)
            transform.position += Vector3.up * 2f;
        }
        else if (!TouchScreenKeyboard.visible && keyboard != null)
        {
            // Keyboard just became inactive
            keyboard = null;
            // Reset the GameObject position to original
            transform.position = originalPosition;
        }
    }
}
