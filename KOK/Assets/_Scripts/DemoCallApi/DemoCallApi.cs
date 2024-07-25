using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace KOK
{
    public class DemoCallApi : MonoBehaviour
    {
        [SerializeField] private TMP_InputField fullNameInputField;
        [SerializeField] private TMP_InputField studentCodeInputField;
        [SerializeField] private TMP_InputField yobInputField;
        void Start()
        {
        }


        public void DemoGet()
        {
            StartCoroutine(GetRequest("https://65414370f0b8287df1fe2417.mockapi.io/Student"));
        }

        public void DemoPost()
        {
            string fullName = fullNameInputField.text;
            string studentCode = studentCodeInputField.text;
            int yob = Int32.Parse(yobInputField.text);
            StartCoroutine(
                PostRequest("https://65414370f0b8287df1fe2417.mockapi.io/Student", 
                            fullName, 
                            studentCode, 
                            yob));
        }

        public void DemoPut()
        {

        }

        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }
        }

        IEnumerator PostRequest(string url, string fullName, string studentCode, int yob)
        {
            string postData = $"{{ \"fullName\": {fullName}, \"studentCode\": {studentCode}, \"yob\": {yob}}}";
            using (UnityWebRequest www = UnityWebRequest.Post(url, postData, "application/json"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    Debug.Log("Post success!");
                }
            }
        }
    }
}
