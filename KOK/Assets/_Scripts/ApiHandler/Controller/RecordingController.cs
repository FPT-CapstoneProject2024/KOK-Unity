using KOK.ApiHandler.Context;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using System.Collections.Generic;
using System;
using UnityEngine;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Recording;
using Newtonsoft.Json;
using KOK.ApiHandler.DTOModels;

namespace KOK.ApiHandler.Controller
{
    public class RecordingController : MonoBehaviour
    {
        private string recordingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Recordings_Resource;

        private void Start()
        {
            //recordingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Recordings_Resource;
        }

        public void GetRecordingsByOwnerIdCoroutine(Guid accountId, Action<List<Recording>> onSuccess, Action<string> onError)
        {
            // Validate Account ID
            if (accountId == null)
            {
                Debug.Log("Failed to get Account by ID. Recording ID is null!");
                return;
            }

            // Prepare and send api request
            var url = recordingResourceUrl + "?OwnerId=" + accountId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Recording>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {                    
                    onError?.Invoke(errorValue);
                });
        }
    }
}
