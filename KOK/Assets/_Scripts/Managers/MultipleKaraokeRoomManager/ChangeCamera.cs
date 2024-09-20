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
        [SerializeField] private GameObject _videoPlayer;

        int _camState = 0;

        private void OnMouseDown()
        {
            //Debug.LogError("Click");
            _camera1.enabled = !_camera1.enabled;
            _camera2.enabled = !_camera2.enabled;
        }

        public void ToggleCamera()
        {
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

        public void ToggleCameraAndRotateCanvas()
        {
            if (_camState == 0)
            {
                _camState = 1;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                _camState = 0;
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        public void RotateCamera()
        {
            if (_camState == 0)
            {
                _camState = 1;
                _videoPlayer.GetComponent<RectTransform>().localScale = new(2.2f, 2.2f, 0);
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                _camState = 0;
                _videoPlayer.GetComponent<RectTransform>().localScale = new(1, 1, 0);
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }


    }
}
