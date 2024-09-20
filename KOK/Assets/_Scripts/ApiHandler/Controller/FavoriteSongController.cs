using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class FavoriteSongController : MonoBehaviour
    {
        private string favoriteSongsResourceUrl = string.Empty;

        private void Awake()
        {
            favoriteSongsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.FavoriteSongs_Resource;
        }

        private void Update()
        {
            #region Testing
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    var request = new AddFavoriteSongRequest()
            //    {
            //        MemberId = Guid.Parse("bba26a12-ce0b-4572-a4c3-0a77da21e323"),
            //        SongId = Guid.Parse("36439d06-c3a9-4880-b40d-0a2827621074"),
            //    };
            //    AddFavoriteSongCoroutine(request,
            //        (value) =>
            //        {
            //            Debug.Log(value);
            //        },
            //        (value) =>
            //        {
            //            Debug.Log(value);
            //        });
            //}
            //else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    var filter = new FavoriteSongFilter()
            //    {
            //        MemberId = Guid.Parse("bba26a12-ce0b-4572-a4c3-0a77da21e323"),
            //        SongName = ""
            //    };
            //    GetMemberFavoriteSongCoroutine(filter, FavoriteSongOrderFilter.SongId, new PagingRequest(),
            //        (value) =>
            //        {
            //        },
            //        (value) =>
            //        {
            //        });
            //}
            #endregion
        }

        public void AddFavoriteSongCoroutine(AddFavoriteSongRequest request, Action<ResponseResult<FavoriteSongRequest>> onSuccess, Action<ResponseResult<FavoriteSongRequest>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(favoriteSongsResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<FavoriteSongRequest>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<FavoriteSongRequest>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void RemoveFavoriteSongCoroutine(RemoveFavoriteSongRequest request, Action<string> onSuccess, Action<string> onError)
        {
            var query = $"?MemberId={request.MemberId}&SongId={request.SongId}";
            var url = favoriteSongsResourceUrl + query;
            ApiHelper.Instance.DeleteCoroutine(url,
                (successValue) =>
                {
                    onSuccess?.Invoke(successValue);
                },
                (errorValue) =>
                {
                    onError?.Invoke(errorValue);
                });
        }

        public void GetMemberFavoriteSongCoroutine(FavoriteSongFilter filter, FavoriteSongOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<FavoriteSong>> onSuccess, Action<DynamicResponseResult<FavoriteSong>> onError)
        {
            var queryParams = GenerateFavoriteSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(favoriteSongsResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    Debug.Log(successValue);
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<FavoriteSong>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    Debug.Log(errorValue);
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<FavoriteSong>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private NameValueCollection GenerateFavoriteSongQueryParams(FavoriteSongFilter filter, FavoriteSongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (!string.IsNullOrEmpty(filter.SongName))
            {
                queryParams.Add(nameof(filter.SongName), filter.SongName);
            }

            if (!string.IsNullOrEmpty(filter.MemberId.ToString()))
            {
                queryParams.Add(nameof(filter.MemberId), filter.MemberId.ToString());
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        public void GetMemberFavoriteSongFilterCoroutine(FavoriteSongFilter filter, FavoriteSongOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<FavoriteSong>> onSuccess, Action<DynamicResponseResult<FavoriteSong>> onError)
        {
            var queryParams = GenerateFavoriteSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(favoriteSongsResourceUrl + "/filter", queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<FavoriteSong>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<FavoriteSong>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
