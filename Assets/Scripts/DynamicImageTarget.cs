using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using Vuforia;

public class DynamicImageTarget : MonoBehaviour
{
    [Header("Firestore")]
    public string targetName;

    [Header("UI")]
    public TMP_Text nombreTXT;
    public TMP_Text eraTXT;
    public TMP_Text dietaTXT;
    public TMP_Text curiosoTXT;
    public RawImage dinoImage;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

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
            var data = document.ToDictionary();

            nombreTXT.text = "Nombre: " + data["nombre"].ToString();
            eraTXT.text = "Era: " + data["era"].ToString();
            dietaTXT.text = "Dieta: " + data["dieta"].ToString();
            curiosoTXT.text = "Dato Curioso: " + data["datoCurioso"].ToString();

            string tipo = data["tipo"].ToString();
            string mediaUrl = data["mediaURL"].ToString();

            Debug.Log("Tipo: " + tipo + " | URL: " + mediaUrl);

            if (tipo == "imagen")
            {
                // ✅ Solo detener el video, NO desactivar su GameObject
                videoPlayer.Stop();
                videoPlayer.targetTexture = null;

                // ✅ Activar dinoImage y limpiar textura residual
                dinoImage.gameObject.SetActive(true);
                dinoImage.texture = null;

                StartCoroutine(DescargarImagen(mediaUrl));
            }
            else if (tipo == "video")
            {
                // ✅ Activar dinoImage antes de la coroutine
                dinoImage.gameObject.SetActive(true);

                StartCoroutine(ReproducirVideo(mediaUrl));
            }
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
                Debug.Log("[DynamicImageTarget] Visita registrada | UID: " + uid + " | Target: " + targetName);
            }
            else
            {
                Debug.Log("[DynamicImageTarget] Visita ya existía | UID: " + uid + " | Target: " + targetName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[DynamicImageTarget] Error registrando visita: " + e.Message);
        }
    }

    IEnumerator ReproducirVideo(string url)
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        renderTexture = new RenderTexture(1280, 720, 16);
        renderTexture.Create();

        videoPlayer.Stop();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;

        dinoImage.texture = renderTexture;

        bool prepared = false;
        videoPlayer.prepareCompleted += (vp) => { prepared = true; };
        videoPlayer.Prepare();

        float timeout = 10f;
        while (!prepared && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (!prepared)
        {
            Debug.LogError("Video no pudo prepararse: " + url);
            yield break;
        }

        videoPlayer.Play();
        Debug.Log("Video reproduciendo: " + url);
    }

    IEnumerator DescargarImagen(string url)
    {
        string urlEscapada = Uri.EscapeUriString(url);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlEscapada))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                if (dinoImage != null && dinoImage.gameObject.activeInHierarchy)
                {
                    dinoImage.texture = texture;
                    Debug.Log("Imagen cargada correctamente: " + url);
                }
            }
            else
            {
                Debug.LogError("Error cargando imagen: " + request.error + " | URL: " + url);
            }
        }
    }

    void OnDestroy()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}