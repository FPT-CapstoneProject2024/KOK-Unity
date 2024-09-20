using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class KaraokeRoomController : MonoBehaviour
    {
        private string karaokeRoomsResourceUrl = string.Empty;

        private void Awake()
        {
            karaokeRoomsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.KaraokeRooms_Resource;
        }

        private void Update()
        {
            #region Testing
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    var request = new AddKaraokeRoomRequest()
            //    {
            //        MemberId = Guid.Parse("bba26a12-ce0b-4572-a4c3-0a77da21e323"),
            //        KaraokeRoomId = Guid.Parse("36439d06-c3a9-4880-b40d-0a2827621074"),
            //    };
            //    AddKaraokeRoomCoroutine(request,
            //        (value) =>
            //        {
            //            Debug.Log(value);
            //        },
            //        (value) =>
            //        {
            //            Debug.Log(value);
            //        });
            //}
            //else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    var filter = new KaraokeRoomFilter()
            //    {
            //        MemberId = Guid.Parse("bba26a12-ce0b-4572-a4c3-0a77da21e323"),
            //        KaraokeRoomName = ""
            //    };
            //    GetMemberKaraokeRoomCoroutine(filter, KaraokeRoomOrderFilter.KaraokeRoomId, new PagingRequest(),
            //        (value) =>
            //        {
            //        },
            //        (value) =>
            //        {
            //        });
            //}
            #endregion
        }

        public void AddKaraokeRoomCoroutine(AddKaraokeRoomRequest request, Action<ResponseResult<KaraokeRoom>> onSuccess, Action<ResponseResult<KaraokeRoom>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            //Debug.LogError(karaokeRoomsResourceUrl + "  |  " + jsonData);
            ApiHelper.Instance.PostCoroutine(karaokeRoomsResourceUrl, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<KaraokeRoom>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<KaraokeRoom>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetKaraokeRoomByIdCoroutine(Guid karaokeRoomId, Action<ResponseResult<KaraokeRoom>> onSuccess, Action<ResponseResult<KaraokeRoom>> onError)
        {
            var url = karaokeRoomsResourceUrl + $"/{karaokeRoomId.ToString()}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<KaraokeRoom>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<KaraokeRoom>>(errorValue);
                    onError?.Invoke(result);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
