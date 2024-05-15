using UnityEngine;

public class TestAutoResolution : MonoBehaviour
{
    private Vector2 previousResolution;
    public float maxScaleFactor = 2.0f;  // Maximum scale factor for larger resolutions
    public float minScaleFactor = 0.5f;  // Minimum scale factor for smaller resolutions

    void Start()
    {
        // Initialize previousResolution with the current screen resolution
        previousResolution = new Vector2(Screen.width, Screen.height);
        ScaleGameObject();
    }

    void Update()
    {
        // Check if the screen resolution has changed
        if (Screen.width != previousResolution.x || Screen.height != previousResolution.y)
        {
            previousResolution = new Vector2(Screen.width, Screen.height);
            ScaleGameObject();
        }
    }

    void ScaleGameObject()
    {
        // Get the current screen resolution
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the scale factor based on the current resolution
        float scaleFactor = Mathf.Min(screenWidth / previousResolution.x, screenHeight / previousResolution.y);

        // Clamp the scale factor to min and max values
        scaleFactor = Mathf.Clamp(scaleFactor, minScaleFactor, maxScaleFactor);

        // Apply the scale factor to the GameObject
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Log the new scale for debugging
        Debug.Log("New scale factor: " + scaleFactor);
    }
}
