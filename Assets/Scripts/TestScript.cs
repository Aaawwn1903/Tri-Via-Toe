using UnityEngine;
using UnityEngine.UI;

public class ButtonImageName : MonoBehaviour
{
    private Button[] buttons; // Array to store buttons

    void Start()
    {
        // Get all Button components attached to the GameObject
        buttons = GetComponentsInChildren<Button>();

        // Add listeners to each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Store the current index for use in the listener
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        // Get the Image component attached to the clicked button
        Image image = buttons[buttonIndex].GetComponent<Image>();

        // Get the name of the sprite associated with the clicked button's image
        string imageName = image.sprite.name;

        // Print the name to the console (you can replace this with any desired action)
        Debug.Log("Button " + buttonIndex + " Image Name: " + imageName);
    }
}
