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
//            StartCoroutine(GetPosts());
//        }

//        IEnumerator GetPosts()
//        {
//            UnityWebRequest request = UnityWebRequest.Get(postResourceUrl);

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

//        /*   IEnumerator GetMemberInfo(Post post)
//           {
//               string url = $"https://localhost:7017/api/accounts/{post.MemberId}";

//               UnityWebRequest request = UnityWebRequest.Get(url);

//               yield return request.SendWebRequest();

//               if (request.result == UnityWebRequest.Result.Success)
//               {
//                   string response = request.downloadHandler.text;
//                   var member = JsonConvert.DeserializeObject<Account>(response);
//                   post.Member = member;
//               }
//               else
//               {
//                   Debug.LogError($"Failed to fetch member info for post {post.PostId}: {request.error}");
//               }
//           }*/

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


using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KOK
{
    public class PostController : MonoBehaviour
    {
        private string postResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Posts_Resource;

        private void Start()
        {
            postResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Posts_Resource;
        }

        public void GetPostByIdCoroutine(Guid postId, Action<List<Post>> onSuccess, Action<string> onError)
        {
            // Validate Post ID
            if (postId == null)
            {
                Debug.Log("Failed to get Post by ID. Post ID is null!");
                return;
            }

            // Prepare and send api request
            var url = postResourceUrl + "?PostId=" + postId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Post>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve an Post by ID [{postId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public async Task<Post?> GetPostByIdAsync(Guid postId)
        {
            var url = postResourceUrl + "/" + postId.ToString();
            var jsonResult = await ApiHelper.Instance.GetAsync(url);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            ResponseResult<Post> result = JsonConvert.DeserializeObject<ResponseResult<Post>>(jsonResult);

            return result.Value;
        }

        public void GetPostsFilterPagingCoroutine(PostFilter filter, PostOrderFilter orderFilter, PagingRequest paging, Action<List<Post>> onSuccess, Action<string> onError)
        {
            var queryParams = GeneratePostQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(postResourceUrl, queryParams);

            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Post>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve Post list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        /*public async Task<Post?> CreatePostAsync(CreatePostRequest createPost)
        {
            var jsonData = JsonConvert.SerializeObject(createPost);
            var url = postResourceUrl;
            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            Debug.Log(jsonResult);

            ResponseResult<Post> result = JsonConvert.DeserializeObject<ResponseResult<Post>>(jsonResult);

            return result.Value;
        }*/

        public async Task<DynamicResponseResult<Post>?> GetPostsFilterPagingAsync(PostFilter filter, PostOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GeneratePostQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(postResourceUrl, queryParams);

            Debug.Log(url);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<Post> result = JsonConvert.DeserializeObject<DynamicResponseResult<Post>>(jsonResult);
            return result;
        }

        public NameValueCollection GeneratePostQueryParams(PostFilter filter, PostOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.Caption != null)
            {
                queryParams.Add(nameof(filter.Caption), filter.Caption.ToString());
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}

