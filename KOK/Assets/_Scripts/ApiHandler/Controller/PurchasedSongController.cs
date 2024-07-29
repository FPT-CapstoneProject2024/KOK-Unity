using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PurchasedSong;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.Assets._Scripts.ApiHandler.Controller
{
    public class PurchasedSongController : MonoBehaviour
    {
        private string purchasedSongResourceUrl = string.Empty;

        private void Start()
        {
            purchasedSongResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PurchasedSongs_Resource;
        }

        public void GetPurchasedSongByIdCoroutine(Guid songId, Action<List<PurchasedSong>> onSuccess, Action<string> onError)
        {
            // Validate song ID
            if (songId == null)
            {
                Debug.Log("Failed to get song by ID. Song ID is null!");
                return;
            }

            // Prepare and send API request
            var url = purchasedSongResourceUrl + "?PurchasedSongId=" + songId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<PurchasedSong>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve a song by ID [{songId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }
    }
}
