using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KOK
{
    public class ChangeCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera1;
        [SerializeField] private Camera _camera2;

        int _camState = 0;

        private void OnMouseDown()
        {
            //Debug.LogError("Click");
            _camera1.enabled = !_camera1.enabled;
            _camera2.enabled = !_camera2.enabled;
        }

        public void ToggleCamera()
        {
            Debug.LogError("Click");
            if (_camState == 0)
            {
                _camera1.gameObject.SetActive(false);
                _camera2.gameObject.SetActive(true);
                _camState = 1;
            } else
            {
                _camera1.gameObject.SetActive(true);
                _camera2.gameObject.SetActive(false);
                _camState = 0;
            }
        }

        
    }
}
