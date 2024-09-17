using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class MoMoController : MonoBehaviour
    {
        private string moMoResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.MoMo_Resource;

        private void Start()
        {
            moMoResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.MoMo_Resource;
        }

        public void CreatePackagePurchaseMoMoRequestCoroutine(MoMoPaymentRequest request, Action<ResponseResult<MoMoCreatePaymentResponse>> onSuccess, Action<ResponseResult<MoMoCreatePaymentResponse>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(moMoResourceUrl + "/create-payment/up-package-purchase", jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<MoMoCreatePaymentResponse>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<MoMoCreatePaymentResponse>>(errorValue);
                    onError?.Invoke(result);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
