using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Song;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Song;
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
    public class ShopSongController : MonoBehaviour
    {
/*        public TMP_InputField songIdInput;
        public TMP_InputField songCodeInput;
        public TMP_InputField songNameInput;
        public TMP_InputField songDescriptionInput;
        public TMP_InputField songTypeInput;
        public TMP_InputField songPriceInput;
        public TMP_InputField songStatusInput;
        public Toggle canExpireToggle;
        public Toggle canStackToggle;
        public TMP_Text notificationText;
        public float notificationFadeDuration;*/

        private string songResourceUrl = string.Empty;

        private void Start()
        {
            songResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;
        }

        public void GetSongByIdCoroutine(Guid songId, Action<string> onSuccess, Action<string> onError)
        {
            if (songId == null)
            {
                Debug.Log("Failed to get song by ID. Song ID is null!");
                return;
            }

            // Prepare and send api request
            var url = songResourceUrl + "/" + songId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        public async Task<Song?> GetSongByIdAsync(Guid songId)
        {
            var url = songResourceUrl + "/" + songId.ToString();
            var jsonResult = await ApiHelper.Instance.GetAsync(url);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            ResponseResult<Song> result = JsonConvert.DeserializeObject<ResponseResult<Song>>(jsonResult);

            return result.Value;
        }

        public async Task<Song?> CreateSongAsync(CreateSongRequest createSong)
        {
            var jsonData = JsonConvert.SerializeObject(createSong);
            var url = songResourceUrl;
            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            Debug.Log(jsonResult);

            ResponseResult<Song> result = JsonConvert.DeserializeObject<ResponseResult<Song>>(jsonResult);

            return result.Value;
        }

        public async Task<DynamicResponseResult<Song>?> GetSongsFilterPagingAsync(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var url = BuildUrl(songResourceUrl, queryParams);

            Debug.Log(url);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<Song> result = JsonConvert.DeserializeObject<DynamicResponseResult<Song>>(jsonResult);
            return result;
        }

        public string BuildUrl(string baseUrl, NameValueCollection queryParams)
        {
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }

        public NameValueCollection GenerateSongQueryParams(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.SongCode != null)
            {
                queryParams.Add(nameof(filter.SongCode), filter.SongCode);
            }

            if (filter.SongName != null)
            {
                queryParams.Add(nameof(filter.SongName), filter.SongName);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}
