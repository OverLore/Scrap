using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.UI;

public class GetImage : MonoBehaviour
{
    [SerializeField]
    private string uriImage = "https://makeyourgame.fun/images/games/expeditionWars2.png";

    [SerializeField]
    private Image imageToControl;

     [SerializeField]
    private RawImage rawImageToControl;

     [SerializeField]
    private Texture2D texture;

   void Start()
    {
        // Une image existante
        StartCoroutine(GetImageRequest(uriImage));
    }


    IEnumerator GetImageRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
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

                texture = DownloadHandlerTexture.GetContent(webRequest);

                rawImageToControl.texture = texture;
                imageToControl.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }
}
