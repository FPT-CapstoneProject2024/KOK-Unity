/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;

namespace KOK
{
    public class PostCommentLoader : MonoBehaviour
    {
        [SerializeField] private List<Post> postList = new List<Post>();
        [SerializeField] private List<PostRate> postRateList = new List<PostRate>();
        private string postRateBaseUrl = "https://localhost:7017/api/postRates";
        public GameObject displayPanel;
        public GameObject displayPrefab;
        private int currentPostIndex = 0;

        void Start()
        {
            //StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
        }

        public void StartLoadingPostComments(string postId)
        {
            StartCoroutine(GetPostComments(postRateBaseUrl, postId));
        }

        IEnumerator GetPostComments(string url, string id)
        {
            string reqUrl = $"{url}/{id}";
            UnityWebRequest request = UnityWebRequest.Get(reqUrl);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<PostRateResponseObject>(response);
                var postRates = responseObject.Value;

                postRateList.Clear();
                postRateList.AddRange(postRates);

                CommentsGenerate();
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        void CommentsGenerate()
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (PostRate postRate in postRateList)
            {
                GameObject gameObj = Instantiate(displayPrefab, displayPanel.transform);
                gameObj.transform.GetChild(1).GetComponent<TMP_Text>().text = postRate.MemberId.ToString();
                gameObj.transform.GetChild(2).GetComponent<TMP_Text>().text = postRate.Comment;
            }
        }
    }

    public class PostRateResponseObject
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public List<PostRate> Value { get; set; }
    }
}


*/