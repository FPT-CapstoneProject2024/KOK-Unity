using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class SongController : MonoBehaviour
    {
        private string songsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;

        private void Start()
        {
            songsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;
        }

        

        /// <summary>
        /// OLD METHOD, PLEASE DO NOT USE!
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderFilter"></param>
        /// <param name="paging"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void GetSongsFilterPagingCoroutine(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging, Action<List<SongDetail>> onSuccess, Action<string> onError)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(songsResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve songs list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        private NameValueCollection GenerateSongQueryParams(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            filter.SongStatus = SongStatus.ENABLE;
            if (!string.IsNullOrEmpty(filter.SongName))
            {
                queryParams.Add(nameof(filter.SongName), filter.SongName);
            }

            if (!string.IsNullOrEmpty(filter.SongCode))
            {
                queryParams.Add(nameof(filter.SongCode), filter.SongCode);
            }

            if (!string.IsNullOrEmpty(filter.GenreName))
            {
                queryParams.Add(nameof(filter.GenreName), filter.GenreName);
            }

            if (!string.IsNullOrEmpty(filter.SingerName))
            {
                queryParams.Add(nameof(filter.SingerName), filter.SingerName);
            }

            if (!string.IsNullOrEmpty(filter.ArtistName))
            {
                queryParams.Add(nameof(filter.ArtistName), filter.ArtistName);
            }

            queryParams.Add(nameof(filter.SongStatus), filter.SongStatus.ToString());

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        public void GetSongByIdCoroutine(Guid songId, Action<ResponseResult<SongDetail>> onSuccess, Action<ResponseResult<SongDetail>> onError)
        {
            var url = songsResourceUrl + $"/{songId.ToString()}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<SongDetail>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<SongDetail>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetSongsFilterPagingCoroutine(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<SongDetail>> onSuccess, Action<DynamicResponseResult<SongDetail>> onError)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(songsResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        //bba26a12-ce0b-4572-a4c3-0a77da21e323
        public void GetSongsFilterPagingCoroutine(string accountId, SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<SongDetail>> onSuccess, Action<DynamicResponseResult<SongDetail>> onError)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var endpoint = songsResourceUrl + $"/{accountId}/filter";
            var url = QueryHelper.BuildUrl(endpoint, queryParams);
            Debug.Log(url);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}