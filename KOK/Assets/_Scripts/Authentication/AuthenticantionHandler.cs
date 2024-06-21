using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class AuthenticantionHandler : MonoBehaviour
    {

        [SerializeField] private FusionConnection fusionConnection;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnLoginButtonClick()
        {
            this.gameObject.SetActive(false);
            fusionConnection.OnLoginSuccess();

        }
    }
}
