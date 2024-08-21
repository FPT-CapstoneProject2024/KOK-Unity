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



        public void GetPostsFilterPagingCoroutine(PostFilter filter, PostOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<Post>> onSuccess /*Action<List<Post>> onSuccess*/, Action<string> onError)
        {
            var queryParams = GeneratePostQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(postResourceUrl, queryParams);
            url = url.Replace("Ascending", "Descending");
            Debug.Log(url);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Post>>(successValue);
                    //onSuccess?.Invoke(result.Results);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve Post list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
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

        public void CreatePostCoroutine(CreatePostRequest newPost, Action<Post> onSuccess, Action<string> onError)
        {
            var jsonData = JsonConvert.SerializeObject(newPost);
            var url = postResourceUrl;

            ApiHelper.Instance.PostCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Post>>(successValue);
                    onSuccess?.Invoke(result.Value);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to create new post: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void AddPostCoroutine(CreatePostRequest request, Action<ResponseResult<Post>> onSuccess, Action<ResponseResult<Post>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            Debug.Log(postResourceUrl + "  |  " + jsonData);
            ApiHelper.Instance.PostCoroutine(postResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Post>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Post>>(errorValue);
                    onError?.Invoke(result);
                });
        }
    }
}

