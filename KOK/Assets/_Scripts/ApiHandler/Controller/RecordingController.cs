using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class RecordingController : MonoBehaviour
    {
        private string recordingResourceUrl = string.Empty;

        private void Start()
        {
            recordingResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Recordings_Resource;
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


    }
}
