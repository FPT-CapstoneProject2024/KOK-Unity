//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using Newtonsoft.Json;
//using TMPro;
//using KOK.ApiHandler.DTOModels;
//using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
//using KOK.ApiHandler.Context;

//namespace KOK
//{
//    public class PostController : MonoBehaviour
//    {
//        private string postResourceUrl = string.Empty;

//        public GameObject displayPanel;
//        public GameObject captionDisplayPanel;
//        public GameObject displayPrefab;

//        public PostTransition postTransition;
//        private int currentPostIndex = 0;
//        private List<Post> loadedPosts = new List<Post>(); // Store loaded posts

//        void Start()
//        {
//            postResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Posts_Resource;
//            StartCoroutine(GetPosts(postResourceUrl));
//        }

//        IEnumerator GetPosts(string url)
//        {
//            UnityWebRequest request = UnityWebRequest.Get(url);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                string response = request.downloadHandler.text;
//                var responseObject = JsonConvert.DeserializeObject<DynamicResponseResult<Post>>(response);
//                var posts = responseObject.Results;

//                if (postTransition != null)
//                {
//                    GeneratePosts(posts); // Pass loaded posts to the method
//                }

//                Debug.Log(response);
//            }
//            else
//            {
//                Debug.LogError(request.error);
//            }
//        }

//     /*   IEnumerator GetMemberInfo(Post post)
//        {
//            string url = $"https://localhost:7017/api/accounts/{post.MemberId}";

//            UnityWebRequest request = UnityWebRequest.Get(url);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                string response = request.downloadHandler.text;
//                var member = JsonConvert.DeserializeObject<Account>(response);
//                post.Member = member;
//            }
//            else
//            {
//                Debug.LogError($"Failed to fetch member info for post {post.PostId}: {request.error}");
//            }
//        }*/

//        void GeneratePosts(List<Post> posts)
//        {
//            if (postTransition != null)
//            {
//                postTransition.SetInitialPosts(posts); // Call method in PostTransition to set posts
//            }
//        }

//        // Method to get current post ID based on currentPostIndex
//        public string GetCurrentPostId()
//        {
//            if (currentPostIndex >= 0 && currentPostIndex < loadedPosts.Count)
//            {
//                return loadedPosts[currentPostIndex].PostId.ToString();
//            }
//            return string.Empty; // Return empty string if index is out of bounds
//        }
//    }

//    public class PostResponseObject
//    {
//        public string Code { get; set; }
//        public string Message { get; set; }
//        public Metadata Metadata { get; set; }
//        public List<Post> Results { get; set; }
//    }

//    public class Metadata
//    {
//        public int Page { get; set; }
//        public int Size { get; set; }
//        public int Total { get; set; }
//    }
//}

