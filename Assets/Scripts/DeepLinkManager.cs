using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    void Start()
    {
        string url = Application.absoluteURL;

        if (!string.IsNullOrEmpty(url) && url.Contains("user="))
        {
            string userID = url.Split("user=")[1];

            FirebaseManager.Instance.userID = userID;

            Debug.Log("Usuario recibido: " + userID);
        }
        else
        {
 
            FirebaseManager.Instance.userID = SystemInfo.deviceUniqueIdentifier;
        }
    }

    public void RegresarAReact()
    {
        string userID = FirebaseManager.Instance.userID;

        Application.OpenURL("examen://back?user=" + userID);
    }
}