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

        // Mendapatkan gambar dari tombol yang diklik
        Image clickedButtonImage = buttons[buttonIndex].GetComponent<Image>();

        // Menghitung baris dan kolom berdasarkan indeks tombol
        int row = buttonIndex / 3;
        int col = buttonIndex % 3;

        // Mendapatkan nama gambar dari "Clue Left Image" yang terpasang
        string clueLeftImageName = clueLeftImages[row].sprite != null ? clueLeftImages[row].sprite.name : "null";
        Debug.Log("Clue kiri untuk baris " + row + ": " + clueLeftImageName);

        // Mendapatkan nama gambar dari "Clue Above Image" yang terpasang
        string clueAboveImageName = clueAboveImages[col].sprite != null ? clueAboveImages[col].sprite.name : "null";
        Debug.Log("Clue atas untuk kolom " + col + ": " + clueAboveImageName);
    }
}
