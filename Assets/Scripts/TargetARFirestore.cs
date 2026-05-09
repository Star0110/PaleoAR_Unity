using UnityEngine;
using Vuforia;
using Firebase.Firestore;
using System.Collections.Generic;

public class TargetARFirestore : MonoBehaviour
{
    public AudioSource audioSource;
    public string targetName;
    public Animator animator;

    private ImageTargetBehaviour imageTarget;
    private FirebaseFirestore db;

    private bool yaDetectado = false;

    void Start()
    {
        Debug.Log("🔥 Script iniciado");

        imageTarget = GetComponent<ImageTargetBehaviour>();

        if (imageTarget == null)
        {
            Debug.LogError("❌ No es un ImageTarget válido");
            return;
        }

        db = FirebaseFirestore.DefaultInstance;
    }

    void Update()
    {
        if (imageTarget == null) return;

        // 🔥 DETECCIÓN ESTABLE (como EventoAR)
        if (imageTarget.TargetStatus.Status == Status.TRACKED)
        {
            if (!yaDetectado)
            {
                yaDetectado = true;
                Debug.Log("✅ Target detectado");
                ActivarTarget();
            }
        }
        else
        {
            if (yaDetectado)
            {
                Debug.Log("❌ Target perdido");
            }
            yaDetectado = false;
        }
    }

    void ActivarTarget()
    {
        Debug.Log("🎯 Activando contenido");

        // 🔊 AUDIO
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("🔊 Audio reproducido");
        }

        // 🎬 ANIMACIÓN
        if (animator != null)
        {
            animator.SetTrigger("Play");
            Debug.Log("🎬 Animación activada");
        }
        else
        {
            Debug.LogWarning("⚠️ Animator no asignado");
        }

        GuardarEvento("deteccion", "Target detectado");
    }

    void GuardarEvento(string tipo, string descripcion)
    {
        if (db == null)
        {
            Debug.LogError("❌ Firebase no disponible");
            return;
        }

        Dictionary<string, object> evento = new Dictionary<string, object>
        {
            { "target", targetName },
            { "tipo", tipo },
            { "descripcion", descripcion },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };

        db.Collection("eventosAR").AddAsync(evento);

        Debug.Log("☁️ Evento guardado: " + tipo);
    }
}