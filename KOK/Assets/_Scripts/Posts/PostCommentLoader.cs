using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using KOK.ApiHandler.Context;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.PostComment;
using System.Threading.Tasks;
using System;

namespace KOK
{
    public class PostCommentLoader : MonoBehaviour
    {
        private List<Post> postList = new List<Post>();
        private List<PostComment> postCommentList = new List<PostComment>();
        //private string postRateBaseUrl = "https://localhost:7017/api/postRates";
        private string postCommentsResourceUrl = string.Empty;
        public GameObject displayPanel;
        public GameObject displayPrefab;
        private int currentPostIndex = 0;

        void Start()
        {
            //StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
            postCommentsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PostComments_Resource;
        }

        /*public void StartLoadingPostComments(string postId)
        {
            GetPostCommentsAsync
        }*/

        /*IEnumerator GetPostComments(string url, string id)
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
        }*/

        /*public void GetCommentsFilterPaging()
        {
            postCommentList = new();
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PostCommentController>()
                .GetPostCommentsFilterPagingCoroutine(  new PostCommentFilter(),
                                                    new PostCommentOrderFilter(),
                                                    new PagingRequest(),
                                                    CommentsGenerate,
                                                    OnError
                );
        }*/

        void CommentsGenerate(List<PostComment> postCommentList)
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (PostComment postComment in postCommentList)
            {
                GameObject gameObj = Instantiate(displayPrefab, displayPanel.transform);
                gameObj.transform.GetChild(1).GetComponent<TMP_Text>().text = postComment.MemberId.ToString();
                gameObj.transform.GetChild(2).GetComponent<TMP_Text>().text = postComment.Comment;
            }
        }

        public async Task LoadPostCommentsAsync(string postId)
        {
            PostCommentController postCommentController = FindAnyObjectByType<ApiHelper>().gameObject.GetComponent<PostCommentController>();

            List<PostComment> postComments = await postCommentController.GetPostCommentsByPostIdAsync(Guid.Parse(postId));

            //GetPostMember(postComment.MemberId.Value);
            CommentsGenerate(postComments);
        }

        private void OnError(string error)
        {
            Debug.LogError(error);
        }
    }

    /*public class PostRateResponseObject
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public List<PostRate> Value { get; set; }
    }*/
}


