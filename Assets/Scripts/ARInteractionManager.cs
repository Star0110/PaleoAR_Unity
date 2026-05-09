using UnityEngine;
using System.Collections.Generic;

public class ARInteractionManager : MonoBehaviour
{
    public static ARInteractionManager Instance;

    private Dictionary<string, int> contador = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    public void RegistrarInteraccion(string targetID, string tipo)
    {
        string key = targetID + "_" + tipo;

        if (!contador.ContainsKey(key))
            contador[key] = 0;

        contador[key]++;

        Debug.Log($"Interacción con {targetID} ({tipo}): {contador[key]}");

        FirebaseManager.Instance.GuardarEvento(targetID, tipo, contador[key]);
    }
}