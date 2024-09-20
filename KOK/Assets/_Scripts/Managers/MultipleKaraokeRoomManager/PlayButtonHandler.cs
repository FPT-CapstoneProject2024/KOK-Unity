using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK
{
    public class PlayButtonHandler : MonoBehaviour
    {
        [SerializeField] private Sprite _buttonOn;
        [SerializeField] private Sprite _buttonOff;
        [SerializeField] private Button _button;
        [SerializeField] private VideoPlayer _videoPlayer;
        private Image _image;

        void Start()
        {
            _image = _button.gameObject.GetComponent<Image>();

        }

        void OnEnable()
        {
            StartCoroutine(SwapSprite());
        }
        IEnumerator SwapSprite()
        {
            yield return new WaitForSeconds(0.5f);
            if (_videoPlayer.isPlaying)
            {
                _image.sprite = _buttonOn;
            }
            else
            {
                _image.sprite = _buttonOff;
            }
            StartCoroutine(SwapSprite());
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
