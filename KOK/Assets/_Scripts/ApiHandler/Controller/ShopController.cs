using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace KOK
{
    public class ShopController : MonoBehaviour
    {
        private string shopResourceUrl = string.Empty;

        private void Awake()
        {
            shopResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Shop_Resource;
        }

        public void PurchaseSongCoroutine(SongPurchaseRequest request, Action<ResponseResult<SongPurchaseResponse>> onSuccess, Action<ResponseResult<SongPurchaseResponse>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(shopResourceUrl + "/purchase-song", jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<SongPurchaseResponse>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<SongPurchaseResponse>>(errorValue);
                    onError?.Invoke(result);
                });
        }
    }
}
