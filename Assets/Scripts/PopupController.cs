using UnityEngine;
using TMPro;
using Unity.Collections;

public class PopupController : MonoBehaviour
{
    // References to the TMP components
    private TMP_InputField inputField;
    private TMP_Dropdown dropdown;
    private int x;

    private bool isActive = false; // Track whether the popup is active

    private void Start()
    {
        // Get references to the TMP components
        inputField = GetComponentInChildren<TMP_InputField>();
        dropdown = GetComponentInChildren<TMP_Dropdown>();
    }

    // This method will be called when the pop-up is deactivated
    private void OnDisable()
    {
        if (isActive)
        {
            // Reset the pop-up to its initial state
            ResetPopup();
            isActive = false;
        }
    }

    // Method to reset the pop-up to its initial state
    private void ResetPopup()
    {
        // Clear the text content of the input field
        inputField.text = "";

        // Clear the options in the dropdown
        // dropdown.ClearOptions();

        // Reset any other properties or states of the pop-up if necessary
        // For example, you could reset position, scale, or visibility
        // transform.position = new Vector3(0f, 0f, 0f);
    }

    // Method to handle when the pop-up is activated
    public void OnActivate()
    {
        isActive = true;
        // You can add any other logic you need when the pop-up is activated
    }
}
