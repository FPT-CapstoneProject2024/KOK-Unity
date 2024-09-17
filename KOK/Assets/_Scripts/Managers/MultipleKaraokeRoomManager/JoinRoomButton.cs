using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class JoinRoomButton : MonoBehaviour
    {
        [SerializeField] string _roomName;
        
        void Start ()
        {
            _roomName = transform.parent.name;
        }
        public void JoinRoom()
        {
            FusionManager.Instance.JoinRoom(_roomName);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
