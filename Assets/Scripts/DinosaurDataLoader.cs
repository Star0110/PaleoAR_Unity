using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Collections.Generic;
using System;
using Vuforia;

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
    private bool yaRegistrado = false;

    private const string TEST_UID = "6HZCVrR3XjZPP9Cn6NXvtOj3ly43";

    private string GetUID()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        return (user != null) ? user.UserId : TEST_UID;
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
            _ = CargarDatos();
    }

    async System.Threading.Tasks.Task CargarDatos()
    {
        Query query = db.Collection("dinosaurios")
                        .WhereEqualTo("targetName", targetName);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            Dictionary<string, object> data = document.ToDictionary();

            nombreTXT.text = "Nombre: " + data["nombre"].ToString();
            eraTXT.text = "Era: " + data["era"].ToString();
            dietaTXT.text = "Dieta: " + data["dieta"].ToString();
            curiosoTXT.text = "Dato Curioso: " + data["datoCurioso"].ToString();
        }

        if (!yaRegistrado)
        {
            await RegistrarVisita();
            yaRegistrado = true;
        }
    }

    async System.Threading.Tasks.Task RegistrarVisita()
    {
        string uid = GetUID();

        try
        {
            DocumentReference visitaRef = db
                .Collection("visitas")
                .Document(uid)
                .Collection("registros")
                .Document(targetName);

            DocumentSnapshot existing = await visitaRef.GetSnapshotAsync();

            if (!existing.Exists)
            {
                Dictionary<string, object> visita = new Dictionary<string, object>
                {
                    { "uid", uid },
                    { "targetName", targetName },
                    { "timestamp", FieldValue.ServerTimestamp }
                };

                await visitaRef.SetAsync(visita);
                Debug.Log("[DinosaurDataLoader] Visita registrada | UID: " + uid + " | Target: " + targetName);
            }
            else
            {
                Debug.Log("[DinosaurDataLoader] Visita ya existía | UID: " + uid + " | Target: " + targetName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[DinosaurDataLoader] Error registrando visita: " + e.Message);
        }
    }

    void OnDestroy()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }
}