using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace KOK
{
    public class Shop : MonoBehaviour
    {
        private string url = "https://localhost:7017/api/accounts";

        void Start()
        {
            StartCoroutine(GetData(url));
        }
        IEnumerator GetData(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }

        }
    }
}
