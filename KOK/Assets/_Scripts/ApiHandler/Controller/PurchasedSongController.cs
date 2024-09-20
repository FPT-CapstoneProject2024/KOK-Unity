using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using UnityEngine;

namespace KOK
{
    public class PurchasedSongController : MonoBehaviour
    {
        private string purchasedSongsResourceUrl = string.Empty;

        private void Awake()
        {
            purchasedSongsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PurchasedSongs_Resource;
        }

        public void GetPurchasedSongByIdCoroutine(Guid purchasedSongId, Action<List<PurchasedSong>> onSuccess, Action<string> onError)
        {
            // Validate song ID
            if (purchasedSongId == null)
            {
                Debug.Log("Failed to get song by ID. Song ID is null!");
                return;
            }

            // Prepare and send API request
            var url = purchasedSongsResourceUrl + "?PurchasedSongId=" + purchasedSongId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PurchasedSong>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve a song by ID [{purchasedSongId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetMemberPurchasedSongFilterCoroutine(PurchasedSongFilter filter, PurchasedSongOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<PurchasedSong>> onSuccess, Action<DynamicResponseResult<PurchasedSong>> onError)
        {
            var queryParams = GeneratePurchasedSongQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(purchasedSongsResourceUrl + "/filter", queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PurchasedSong>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PurchasedSong>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private NameValueCollection GeneratePurchasedSongQueryParams(PurchasedSongFilter filter, PurchasedSongOrderFilter orderFilter, PagingRequest paging)
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

            if (!string.IsNullOrEmpty(filter.SongId.ToString()) && filter.SongId != Guid.Empty)
            {
                queryParams.Add(nameof(filter.SongId), filter.SongId.ToString());
            }
            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
