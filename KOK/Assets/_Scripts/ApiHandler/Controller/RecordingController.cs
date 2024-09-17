using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using Newtonsoft.Json;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace KOK.ApiHandler.Controller
{
    public class RecordingController : MonoBehaviour
    {
        private string recordingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Recordings_Resource;

        private void Start()
        {
            //recordingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Recordings_Resource;
        }

        public void AddRecordingCoroutine(CreateRecordingRequest request, Action<ResponseResult<Recording>> onSuccess, Action<ResponseResult<Recording>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            Debug.Log(recordingResourceUrl + "  |  " + jsonData);
            ApiHelper.Instance.PostCoroutine(recordingResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Recording>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Recording>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetRecordingByIdCoroutine(Guid recordingId, Action<ResponseResult<Recording>> onSuccess, Action<ResponseResult<Recording>> onError)
        {
            var url = recordingResourceUrl + $"/{recordingId.ToString()}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Recording>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Recording>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetRecordingsByIdCoroutine2(Guid recordingId, Action<List<Recording>> onSuccess, Action<string> onError)
        {
            // Validate Recording ID
            if (recordingId == null)
            {
                Debug.Log("Failed to get Recording by ID. Recording ID is null!");
                return;
            }

            // Prepare and send api request
            var url = recordingResourceUrl + "?RecordingId=" + recordingId.ToString();
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

        public void GetRecordingsByOwnerIdCoroutine(Guid accountId, Action<List<Recording>> onSuccess, Action<string> onError)
        {
            // Validate Account ID
            if (accountId == null)
            {
                Debug.Log("Failed to get Account by ID. Account ID is null!");
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

        public void DeleteRecordingsByIdCoroutine(Guid recordingId, Action onSuccess, Action onError)
        {
            if (recordingId == null)
            {
                Debug.Log("Failed to get Recording by ID. Recording ID is null!");
                return;
            }
            var url = recordingResourceUrl + $"/{recordingId.ToString()}";
            ApiHelper.Instance.DeleteCoroutine(url,
                (successValue) =>
                {
                    onSuccess?.Invoke();
                },
                (errorValue) =>
                {
                    onError?.Invoke();
                });

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
