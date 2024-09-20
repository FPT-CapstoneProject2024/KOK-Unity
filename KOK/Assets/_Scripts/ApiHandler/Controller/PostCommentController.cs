using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.PostComment;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

namespace KOK.Assets._Scripts.ApiHandler.Controller
{
    public class PostCommentController : MonoBehaviour
    {
        private string postCommentResourceUrl = string.Empty;

        private void Start()
        {
            postCommentResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PostComments_Resource;
        }

        /*public void GetPostCommentByIdCoroutine(Guid postCommentId, Action<string> onSuccess, Action<string> onError)
        {
            if (postCommentId == null)
            {
                Debug.Log("Failed to get post comment by ID. Post Comment ID is null!");
                return;
            }

            // Prepare and send api request
            var url = postCommentResourceUrl + "/" + postCommentId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }*/


        public void GetPostCommentsByPostIdCoroutine(Guid postId, Action<List<PostComment>> onSuccess, Action<string> onError)
        {
            // Validate PostComment ID
            if (postId == null)
            {
                Debug.Log("Failed to get PostComment by ID. PostComment ID is null!");
                return;
            }

            // Prepare and send api request
            var url = postCommentResourceUrl + "?PostId=" + postId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    //Debug.LogError($"Error when trying to retrieve an PostComment by ID [{postId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetPostRepliesByCommentIdCoroutine(Guid postId, Action<List<PostComment>> onSuccess, Action<string> onError)
        {
            // Validate PostComment ID
            if (postId == null)
            {
                Debug.Log("Failed to get PostComment by ID. PostComment ID is null!");
                return;
            }

            // Prepare and send api request
            var url = postCommentResourceUrl + "?ParentCommentId=" + postId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    //Debug.LogError($"Error when trying to retrieve an PostComment by ID [{postId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }


        public void GetPostCommentsFilterPagingCoroutine(PostCommentFilter filter, PostCommentOrderFilter orderFilter, PagingRequest paging, Action<List<PostComment>> onSuccess, Action<string> onError)
        {
            var queryParams = GeneratePostCommentQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(postCommentResourceUrl, queryParams);

            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve PostComment list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetAllCommentsOfAPost(Guid postId, Action<List<PostComment>> onSuccess, Action<string> onError)
        {
            PostCommentFilter filter = new() { PostId = postId, CommentType = 0, Status = 1 };
            var queryParams = GeneratePostCommentQueryParams(filter, new(), new());
            var url = QueryHelper.BuildUrl(postCommentResourceUrl, queryParams);

            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PostComment>>(successValue);
                    foreach (var comment in result.Results)
                    {
                        comment.InverseParentComment = comment.InverseParentComment.Where(reply => (int)reply.Status == 1).ToList();
                    }
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve PostComment list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void CreateComment(Guid postId, CreatePostCommentRequest request, Action<ResponseResult<PostComment>> onSuccess, Action<string> onError)
        {
            request.PostId = postId;
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(postCommentResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<string>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void DeleteCommentByIdCoroutine(Guid postCommentId, Action onSuccess, Action onError)
        {
            if (postCommentId == null)
            {
                Debug.Log("Failed to get Post by ID. Post ID is null!");
                return;
            }
            var url = postCommentResourceUrl + $"/{postCommentId.ToString()}";
            ApiHelper.Instance.DeleteCoroutine(url,
                (successValue) =>
                {
                    onSuccess?.Invoke();
                },
                (errorValue) =>
                {
                    onError?.Invoke();
                });

        }

        public void CreateReply(Guid postId, Guid parentCommentId, CreatePostCommentRequest request, Action<ResponseResult<PostComment>> onSuccess, Action<string> onError)
        {
            request.PostId = postId;
            request.ParentCommentId = parentCommentId;
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(postCommentResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<string>(errorValue);
                    onError?.Invoke(result);
                });
        }
        public NameValueCollection GeneratePostCommentQueryParams(PostCommentFilter filter, PostCommentOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.MemberId != null && filter.MemberId != Guid.Empty)
            {
                queryParams.Add(nameof(filter.MemberId), filter.MemberId.ToString());
            }

            if (filter.PostId != null && filter.PostId != Guid.Empty)
            {
                queryParams.Add(nameof(filter.PostId), filter.PostId.ToString());
            }

            queryParams.Add(nameof(filter.Status), filter.Status.ToString());

            if (!filter.Comment.IsNullOrEmpty())
            {
                queryParams.Add(nameof(filter.Comment), filter.Comment.ToString());
            }


            queryParams.Add(nameof(filter.CommentType), filter.CommentType.ToString());


            //queryParams.Add(nameof(paging.page), paging.page.ToString());
            //queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            //queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            //queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        public void UpdateCommentCoroutine(Guid postCommentId, EditPostCommentRequest request, Action<ResponseResult<PostComment>> onSuccess, Action<ResponseResult<PostComment>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            //jsonData = jsonData.Replace("\"Caption\":", "");
            var url = postCommentResourceUrl + "/" + postCommentId;
            Debug.Log(url + "  |  " + jsonData);
            ApiHelper.Instance.PutCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PostComment>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PostComment>>(errorValue);
                    Debug.LogError(result.Value + "\n" + result.Message) ;
                    onError?.Invoke(result);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
