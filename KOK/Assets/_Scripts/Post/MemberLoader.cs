/*using Newtonsoft.Json;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using KOK.ApiHandler.DTOModels;

namespace KOK.Assets._Scripts.Post
{
    public class MemberLoader : MonoBehaviour
    {
        //[SerializeField] private List<Post> postList = new List<Post>();
        //[SerializeField] private List<PostRate> postRateList = new List<PostRate>();
        [SerializeField] private List<Account> memberList = new List<Account>();
        private string accountBaseUrl = "https://localhost:7017/api/accounts";
        public GameObject displayPanel;
        public GameObject displayPrefab;
        private int currentPostIndex = 0;

        void Start()
        {
            //StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
        }

        public void LoadPostMember(string postId)
        {
            StartCoroutine(GetPostMember(accountBaseUrl, postId));
        }

        IEnumerator GetPostMember(string url, string id)
        {
            string reqUrl = $"{url}/{id}";
            UnityWebRequest request = UnityWebRequest.Get(reqUrl);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<ResponseResult<Account>>(response);
                var members = responseObject.Value;

                memberList.Clear();
                memberList.AddRange(members);

                MemberGenerate();
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        void MemberGenerate()
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Account member in memberList)
            {
                GameObject gameObj = Instantiate(displayPrefab, displayPanel.transform);
                gameObj.transform.GetComponent<TMP_Text>().text = member.UserName;
            }
        }
    }

    public class AccountResponseObject
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public List<Account> Value { get; set; }
    }
}
*/