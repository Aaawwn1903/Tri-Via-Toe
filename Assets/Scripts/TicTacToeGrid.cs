using UnityEngine;
using UnityEngine.UI;

public class TicTacToeGrid : MonoBehaviour
{
    public Button buttonPrefab; // Prefab for the button
    public Transform gridParent; // Parent transform for the grid

    private void Start()
    {
        // Instantiate buttons and add them to the grid
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // Instantiate button prefab
                Button button = Instantiate(buttonPrefab, gridParent);

                // Set button position and size in the grid
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(col / 3f, (2 - row) / 3f);
                rectTransform.anchorMax = new Vector2((col + 1) / 3f, (3 - row) / 3f);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                // Add click event handler to the button
                int buttonRow = row;
                int buttonCol = col;
                button.onClick.AddListener(() => OnButtonClick(buttonRow, buttonCol));
            }
        }
    }

    private void OnButtonClick(int row, int col)
    {
        // Handle button click event here
        Debug.Log("Button clicked at row " + row + ", column " + col);
    }
}
