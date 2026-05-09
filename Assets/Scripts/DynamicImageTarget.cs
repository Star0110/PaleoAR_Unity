using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Firebase.Firestore;
using UnityEngine.Networking;
using System.Collections;

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
            var data = document.ToDictionary();

            nombreTXT.text = "Nombre: " + data["nombre"].ToString();
            eraTXT.text = "Era: " + data["era"].ToString();
            dietaTXT.text = "Dieta: " + data["dieta"].ToString();
            curiosoTXT.text = "Dato Curioso: " + data["datoCurioso"].ToString();

            string tipo = data["tipo"].ToString();
            string mediaUrl = data["mediaURL"].ToString();

            if (tipo == "imagen")
            {
                videoPlayer.Stop();
                videoPlayer.gameObject.SetActive(false);
                dinoImage.gameObject.SetActive(true);
                StartCoroutine(DescargarImagen(mediaUrl));
            }
            else if (tipo == "video")
            {
                // Usar Coroutine para el video, más estable en móvil
                StartCoroutine(ReproducirVideo(mediaUrl));
            }
        }
    }

    IEnumerator ReproducirVideo(string url)
    {
        // Liberar RenderTexture anterior si existe
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        // Crear nuevo RenderTexture
        renderTexture = new RenderTexture(1280, 720, 16);
        renderTexture.Create();

        // Configurar VideoPlayer ANTES de preparar
        videoPlayer.Stop();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;

        // Mostrar el RenderTexture en la RawImage
        dinoImage.texture = renderTexture;

        // Preparar y esperar con evento (más confiable que isPrepared)
        bool prepared = false;
        videoPlayer.prepareCompleted += (vp) => { prepared = true; };
        videoPlayer.Prepare();

        // Timeout de 10 segundos para no quedar atascado
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
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                dinoImage.texture = texture;
            }
            else
            {
                Debug.LogError("Error cargando imagen: " + request.error);
            }
        }
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}