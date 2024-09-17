using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
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

        public void GetPostRatingOfAMember(Guid postId, Guid memberId, Action<PostRating> onSuccess, Action<string> onError) 
        {
            var url = postRatingResourceUrl + $"?MemberId={memberId}&PostId={postId}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PostRating>>(successValue);
                    onSuccess?.Invoke(result.Results[0]);
                },
                (errorValue) =>
                {
                    onError?.Invoke(errorValue);
                });
        }

        public void CreatePostRatingCoroutine(CreatePostRatingRequest newPostRating, Action<PostRating> onSuccess, Action<string> onError)
        {
            var jsonData = JsonConvert.SerializeObject(newPostRating);
            var url = postRatingResourceUrl;
            Debug.Log(url + "\n" + jsonData);
            ApiHelper.Instance.PostCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PostRating>>(successValue);
                    onSuccess?.Invoke(result.Value);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to create new post rating: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }

}
