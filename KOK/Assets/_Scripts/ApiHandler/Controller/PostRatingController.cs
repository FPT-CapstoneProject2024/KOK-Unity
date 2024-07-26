using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostRating;
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
    public class PostRatingController : MonoBehaviour
    {
        private string postRatingResourceUrl = string.Empty;

        private void Start()
        {
            postRatingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PostRatings_Resource;
        }

        public void GetPostRatingByIdCoroutine(Guid postRatingId, Action<string> onSuccess, Action<string> onError)
        {
            if (postRatingId == null)
            {
                Debug.Log("Failed to get post rating by ID. Post Rating ID is null!");
                return;
            }

            // Prepare and send api request
            var url = postRatingResourceUrl + "/" + postRatingId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        public async Task<List<PostRating?>> GetPostRatingsByPostIdAsync(Guid postId)
        {
            var url = postRatingResourceUrl + "?" + postId.ToString();
            var jsonResult = await ApiHelper.Instance.GetAsync(url);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            ResponseResult<List<PostRating>> result = JsonConvert.DeserializeObject<ResponseResult<List<PostRating>>>(jsonResult);

            return result.Value.ToList();
        }

        /*public async Task<PostRating?> CreatePostRatingAsync(CreatePostRatingRequest createPostRating)
        {
            var jsonData = JsonConvert.SerializeObject(createPostRating);
            var url = postRatingResourceUrl;
            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            Debug.Log(jsonResult);

            ResponseResult<PostRating> result = JsonConvert.DeserializeObject<ResponseResult<PostRating>>(jsonResult);

            return result.Value;
        }*/

/*        public async Task<DynamicResponseResult<PostRating>?> GetPostRatingsFilterPagingAsync(PostRatingFilter filter, PostRatingOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GeneratePostRatingQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(postRatingResourceUrl, queryParams);

            Debug.Log(url);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<PostRating> result = JsonConvert.DeserializeObject<DynamicResponseResult<PostRating>>(jsonResult);
            return result;
        }*/

        /*public NameValueCollection GeneratePostRatingQueryParams(PostRatingFilter filter, PostRatingOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.PostRatingCode != null)
            {
                queryParams.Add(nameof(filter.PostRatingCode), filter.PostRatingCode);
            }

            if (filter.PostRatingName != null)
            {
                queryParams.Add(nameof(filter.PostRatingName), filter.PostRatingName);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }*/
    }

}
