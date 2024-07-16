using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class SongController : MonoBehaviour
    {
        private string songsResourceUrl = string.Empty;

        private void Start()
        {
            songsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;
        }

        void Update()
        {
            #region Testing

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SongFilter filter = new SongFilter();
                PagingRequest paging = new PagingRequest();
                GetSongsFilterPagingCoroutine(filter, SongOrderFilter.SongName, paging, (value) => { Debug.Log(value); }, (value) => { Debug.Log(value); });
            }

            #endregion
        }

        public async Task<DynamicResponseResult<SongDetail>?> GetSongsFilterPagingAsync(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
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

        public void GetSongsFilterPagingCoroutine(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging, Action<string> onSuccess, Action<string> onError)
        {
            var queryParams = GenerateSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(songsResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        private NameValueCollection GenerateSongQueryParams(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.SongName != null)
            {
                queryParams.Add(nameof(filter.SongName), filter.SongName);
            }

            if (filter.SongCode != null)
            {
                queryParams.Add(nameof(filter.SongCode), filter.SongCode);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}
