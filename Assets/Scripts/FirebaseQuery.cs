using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreTextUpdater : MonoBehaviour
{
    public TMP_Text textField;
    public string collectionName; // Nama koleksi di Firestore
    public string documentName; // Nama dokumen di dalam koleksi
    public string fieldName; // Nama field yang ingin ditampilkan di TextMeshPro Text

    void Start()
    {
        // Memanggil metode untuk mengambil data Firestore saat skrip diaktifkan
        FetchDataFromFirestore();
    }

    // Metode untuk mengambil data dari Firestore
    async void FetchDataFromFirestore()
    {
        // Memulai koneksi ke Firestore
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // Mengambil referensi dokumen yang diinginkan
        DocumentReference docRef = db.Collection(collectionName).Document(documentName);

        try
        {
            // Mengambil data dari Firestore
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Mengambil nilai dari field yang diinginkan sebagai array
                List<string> fieldValue = snapshot.GetValue<List<string>>(fieldName);

                if (fieldValue != null)
                {
                    // Menggabungkan nilai-nilai array menjadi satu string dengan baris baru di antara setiap elemen
                    string concatenatedText = string.Join("\n", fieldValue);
                    textField.text = concatenatedText;
                }
                else
                {
                    Debug.LogError("Field value is null.");
                }
            }
            else
            {
                Debug.LogError("Document does not exist!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error fetching document: " + ex.Message);
        }
    }
}
