using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ImageLoader : MonoBehaviour
{
    public GameObject[] imageObjects; // GameObject untuk menempatkan gambar
    public string imageFolderPath = "Assets/ImagePulau"; // Path folder gambar, bisa diubah di Inspector
    private List<Sprite> loadedSprites = new List<Sprite>(); // Daftar gambar yang telah dimuat

    void Start()
    {
        LoadImages();
        DisplayImages();
    }

    void LoadImages()
    {
        // Membaca semua file gambar di folder
        string[] imagePaths = Directory.GetFiles(imageFolderPath, "*.png");

        // Memuat gambar menjadi sprite dan menyimpannya di loadedSprites
        foreach (string path in imagePaths)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // Mengatur nama sprite sebagai nama file
            sprite.name = Path.GetFileNameWithoutExtension(path);

            loadedSprites.Add(sprite);
        }
    }

    void DisplayImages()
    {
        // Memastikan bahwa ada cukup banyak gambar yang dimuat
        if (loadedSprites.Count < imageObjects.Length)
        {
            Debug.LogError("Tidak cukup gambar dimuat untuk menempatkan pada GameObject.");
            return;
        }

        // Menetapkan sprite acak ke setiap GameObject gambar
        List<Sprite> tempSprites = new List<Sprite>(loadedSprites);
        for (int i = 0; i < imageObjects.Length; i++)
        {
            int randomIndex = Random.Range(0, tempSprites.Count);
            imageObjects[i].GetComponent<Image>().sprite = tempSprites[randomIndex];
            tempSprites.RemoveAt(randomIndex);
        }
    }
}
