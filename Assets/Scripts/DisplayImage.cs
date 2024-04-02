using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageRandomizer : MonoBehaviour
{
    public Image[] imageObjects; // Array of Image objects to display images
    public Sprite[] images; // Array of sprites to be randomly assigned

    void Start()
    {
        RandomizeImages();
    }

    void RandomizeImages()
    {
        List<Sprite> shuffledImages = new List<Sprite>(images); // Copy the images array to shuffle
        Shuffle(shuffledImages); // Shuffle the list of images

        // Assign shuffled images to the Image objects
        for (int i = 0; i < Mathf.Min(imageObjects.Length, shuffledImages.Count); i++)
        {
            imageObjects[i].sprite = shuffledImages[i];
        }
    }

    // Shuffle the list
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
