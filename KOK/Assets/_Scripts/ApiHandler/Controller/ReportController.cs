using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.Assets._Scripts.ApiHandler.Controller
{
    public class ReportController: MonoBehaviour
    {
        private string reportResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Accounts_Report;

        private void Start()
        {
            reportResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Accounts_Report;
        }

        public void CreateReportCoroutine(CreateReportRequest newReport, Action<Report> onSuccess, Action<string> onError)
        {
            var jsonData = JsonConvert.SerializeObject(newReport);
            var url = reportResourceUrl;

            ApiHelper.Instance.PostCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Report>>(successValue);
                    onSuccess?.Invoke(result.Value);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to create new account: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
