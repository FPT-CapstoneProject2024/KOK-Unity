using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.PostComment;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public NameValueCollection GeneratePostCommentQueryParams(PostCommentFilter filter, PostCommentOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            /* if (filter.MemberId != null)
             {
                 queryParams.Add(nameof(filter.MemberId), filter.MemberId.ToString());
             }*/

            /*if (filter.PostId != null)
            {
                queryParams.Add(nameof(filter.PostId), filter.PostId.ToString());
            }*/

            if (filter.Comment!= null)
            {
                queryParams.Add(nameof(filter.Comment), filter.Comment.ToString());
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}
