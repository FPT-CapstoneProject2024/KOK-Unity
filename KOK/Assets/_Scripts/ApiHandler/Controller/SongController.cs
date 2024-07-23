using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Song;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

        void Update()
        {
        }

        private async Task<DynamicResponseResult<SongDetail>?> GetSongsFilterPagingAsync(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(songsResourceUrl, queryParams);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<SongDetail> result = JsonConvert.DeserializeObject<DynamicResponseResult<SongDetail>>(jsonResult);
            return result;
        }

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
            if (!string.IsNullOrEmpty(filter.SongName))
            {
                queryParams.Add(nameof(filter.SongName), filter.SongName);
            }

            if (!string.IsNullOrEmpty(filter.SongCode))
            {
                queryParams.Add(nameof(filter.SongCode), filter.SongCode);
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
    }
}