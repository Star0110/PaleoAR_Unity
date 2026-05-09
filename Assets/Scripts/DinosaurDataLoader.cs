using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;

public class DinosaurDataLoader : MonoBehaviour
{
    [Header("Target")]
    public string targetName;

    [Header("UI")]
    public TMP_Text nombreTXT;
    public TMP_Text eraTXT;
    public TMP_Text dietaTXT;
    public TMP_Text curiosoTXT;

    FirebaseFirestore db;

    async void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        await CargarDatos();
    }

    async System.Threading.Tasks.Task CargarDatos()
    {
        Query query = db.Collection("dinosaurios")
                        .WhereEqualTo("targetName", targetName);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data = document.ToDictionary();

            nombreTXT.text = "Nombre: "+data["nombre"].ToString();
            eraTXT.text = "Era: " + data["era"].ToString();
            dietaTXT.text = "Dieta: " + data["dieta"].ToString();
            curiosoTXT.text = "Dato Curioso: "+data["datoCurioso"].ToString();
        }
    }
}