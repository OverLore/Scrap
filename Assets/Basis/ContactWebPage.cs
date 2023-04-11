using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class ContactWebPage : MonoBehaviour
{
    [SerializeField]
    private string uri = "https://makeyourgame.fun";

    [SerializeField]
    private string uriImage = "https://makeyourgame.fun/images/games/expeditionWars2.png";

    void Start()
    {
        // Une page existante
        StartCoroutine(GetRequest(uri));

        // Une image existante
        StartCoroutine(GetRequest(uriImage));

        // Une page non existante.
        StartCoroutine(GetRequest("https://error.html"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // On envoie la requête et on attend la réponse
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}
