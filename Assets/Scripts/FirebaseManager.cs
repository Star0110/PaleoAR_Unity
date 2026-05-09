using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    FirebaseFirestore db;

    void Awake()
    {
        Instance = this;
        db = FirebaseFirestore.DefaultInstance;
    }

    public string userID = "anonimo"; 

    public void GuardarEvento(string targetID, string tipo, int veces)
    {
        var data = new Dictionary<string, object>()
    {
        { "target", targetID },
        { "tipo", tipo },
        { "veces", veces },
        { "timestamp", System.DateTime.UtcNow },
        { "user", userID } 
    };

        db.Collection("interacciones")
          .Document()
          .SetAsync(data);
    }
}