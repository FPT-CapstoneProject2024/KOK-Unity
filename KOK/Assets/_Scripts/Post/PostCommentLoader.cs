using System.Collections;
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
        public GameObject displayButton;
        private int currentPostIndex = 0;

        void Start()
        {
            StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
        }

        IEnumerator GetPostComments(string url, string id)
        {
            string reqUrl = $"{url}/{id}";
            UnityWebRequest request = UnityWebRequest.Get(reqUrl);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);
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
                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
                gameObj.transform.GetChild(1).GetComponent<TMP_Text>().text = postRate.MemberId.ToString();
                gameObj.transform.GetChild(2).GetComponent<TMP_Text>().text = postRate.Comment;
            }
        }
    }

    public class ResponseObject
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public List<PostRate> Value { get; set; }
    }
}




//using Newtonsoft.Json;
//using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//namespace KOK
//{
//    public class PostCommentLoader : MonoBehaviour
//    {
//        //[SerializeField] private List<Post> postList = new List<Post>();
//        [SerializeField] private List<PostRate> postRateList = new List<PostRate>();

//        private string postBaseUrl = "https://localhost:7017/api/posts";
//        private string postRateBaseUrl = "https://localhost:7017/api/postRates";
//        private List<string> playerNames = new List<string>();
//        private List<string> postComments = new List<string>();
//        //private List<string> itemNames = new List<string>();
//        public GameObject displayPanel;
//        public GameObject displayButton;

//        void Start()
//        {
//            StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
//            //RefreshClicked();
//        }

//        void CommentsGenerate()
//        {
//            for (int i = 0; i < postRateList.Count; i++)
//            {
//                //GameObject button = displayPanel.transform.GetChild(i).gameObject; 
//                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
//                Debug.Log(gameObj.transform.GetChild(1));
//                Debug.Log(gameObj.transform.GetChild(1));
//                gameObj.transform.GetChild(1).GetComponent<TMP_Text>().text = playerNames[i];
//                gameObj.transform.GetChild(2).GetComponent<TMP_Text>().text = postComments[i];

//                /*int index = i;
//                gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
//                {
//                    ItemClicked(index);
//                });*/
//            }

//            //Destroy(buttonDisplay);
//        }

//        void ItemClicked(int itemIndex)
//        {
//            Debug.Log("item" + itemIndex);
//        }

//        /*public void RefreshClicked()
//        {
//            for (int i = 0; i < displayPanel.transform.childCount; i++)
//            {
//                Destroy(displayPanel.transform.GetChild(i).gameObject);
//            }

//            StartCoroutine(GetData(baseUrl));
//        }*/

//        IEnumerator GetPostComments(string url, string id)
//        {
//            string reqUrl = $"{url}/{id}";
//            UnityWebRequest request = UnityWebRequest.Get(reqUrl);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                string response = request.downloadHandler.text;

//                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);
//                var postRates = responseObject.Value;

//                if (playerNames == null)
//                {
//                    playerNames = new List<string>();
//                }
//                if (postComments == null)
//                {
//                    postComments = new List<string>();
//                }
//                if (postRateList == null)
//                {
//                    postRateList = new List<PostRate>();
//                }

//                // clear before adding
//                playerNames.Clear();
//                postComments.Clear();
//                postRateList.Clear();

//                foreach (PostRate postRate in postRates)
//                {
//                    playerNames.Add(postRate.MemberId.ToString());
//                    postComments.Add(postRate.Comment);
//                    postRateList.Add(postRate);
//                }

//                CommentsGenerate();

//                Debug.Log(response);
//            }
//            else
//            {
//                Debug.LogError(request.error);
//            }
//        }

//        /*IEnumerator GetData(string url)
//        {
//            UnityWebRequest request = UnityWebRequest.Get(url);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                string response = request.downloadHandler.text;

//                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);
//                var items = responseObject.Results;

//                if (itemNames == null)
//                {
//                    itemNames = new List<string>();
//                }
//                if (itemList == null)
//                {
//                    itemList = new List<Item>();
//                }

//                // clear before adding
//                itemNames.Clear();
//                itemList.Clear();

//                foreach (Item item in items)
//                {
//                    itemNames.Add(item.ItemName);
//                    itemList.Add(item);
//                }

//                LayoutGenerate();

//                Debug.Log(response);
//            }
//            else
//            {
//                Debug.LogError(request.error);
//            }
//        }*/

//        public class ResponseObject
//        {
//            public string Message { get; set; }
//            public bool Result { get; set; }
//            public List<PostRate> Value { get; set; }
//        }
//        /*public class ResponseObject
//        {
//            public string Code { get; set; }
//            public string Message { get; set; }
//            public Metadata Metadata { get; set; }
//            public List<PostRate> Results { get; set; }
//        }*/

//        public class Metadata
//        {
//            public int Page { get; set; }
//            public int Size { get; set; }
//            public int Total { get; set; }
//        }
//    }
//}
