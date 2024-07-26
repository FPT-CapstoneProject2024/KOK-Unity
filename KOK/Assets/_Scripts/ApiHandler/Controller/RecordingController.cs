using KOK.ApiHandler.Context;
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


    }
}
