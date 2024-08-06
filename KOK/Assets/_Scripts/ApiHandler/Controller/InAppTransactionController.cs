using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class InAppTransactionController : MonoBehaviour
    {
        private string inAppTransactionResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.InAppTransaction_Resource;

        private void Start()
        {
            inAppTransactionResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.InAppTransaction_Resource;
        }

        public void GetInAppTransactionsByMemberIdCoroutine(Guid accountId, Action<List<InAppTransaction>> onSuccess, Action<string> onError)
        {
            // Validate Account ID
            if (accountId == null)
            {
                Debug.Log("Failed to get by MemberId. MemberId is null!");
                return;
            }

            // Prepare and send api request
            var url = inAppTransactionResourceUrl + "?MemberId=" + accountId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<InAppTransaction>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    onError?.Invoke(errorValue);
                });
        }
    }
}
