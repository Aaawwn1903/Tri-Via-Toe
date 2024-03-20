using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    [SerializeField] private Image[] clueAboveImages; // Array untuk clue di atas
    [SerializeField] private Image[] clueLeftImages; // Array untuk clue di kiri
    [SerializeField] private Button[] buttons; // Array untuk tombol-tombol permainan

    private void Start()
    {
        // Mengaitkan fungsi OnClick pada setiap tombol
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i; // Mencegah penangkapan nilai i yang salah di dalam fungsi OnClick
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        Debug.Log("Button clicked: " + buttonIndex);

        // Menghitung baris dan kolom berdasarkan indeks tombol
        int row = buttonIndex / 3;
        int col = buttonIndex % 3;

        // Mendapatkan nilai clue atas dan kiri sesuai urutan dari kiri ke kanan
        if (col > 0)
        {
            // Mendapatkan nilai clue kiri
            clueLeftImages[row].sprite = buttons[buttonIndex - 1].image.sprite;
            Debug.Log("Clue kiri untuk baris " + row + ": " + buttons[buttonIndex - 1].image.sprite);
        }
        else
        {
            // Jika tombol berada di kolom pertama, maka nilai clue kiri adalah null
            clueLeftImages[row].sprite = null;
            Debug.Log("Clue kiri untuk baris " + row + ": null");
        }

        if (row > 0)
        {
            // Mendapatkan nilai clue atas
            clueAboveImages[col].sprite = buttons[buttonIndex - 3].image.sprite;
            Debug.Log("Clue atas untuk kolom " + col + ": " + buttons[buttonIndex - 3].image.sprite);
        }
        else
        {
            // Jika tombol berada di baris pertama, maka nilai clue atas adalah null
            clueAboveImages[col].sprite = null;
            Debug.Log("Clue atas untuk kolom " + col + ": null");
        }
    }
}
