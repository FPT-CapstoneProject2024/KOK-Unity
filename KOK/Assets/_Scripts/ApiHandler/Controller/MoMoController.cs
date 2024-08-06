using KOK.ApiHandler.Context;
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


    }
}
