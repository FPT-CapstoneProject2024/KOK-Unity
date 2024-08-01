using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ButtonSwapSprite : MonoBehaviour
    {
        [SerializeField] private Sprite _buttonOn;
        [SerializeField] private Sprite _buttonOff;
        [SerializeField] private Button _button;
        private Image _image;

        void Start()
        {
            _image = _button.gameObject.GetComponent<Image>();
        }
        public void SwapSprite()
        {
            if (_image.sprite == _buttonOff)
            {
                _image.sprite = _buttonOn;
            }
            else if (_image.sprite == _buttonOn)
            {
                _image.sprite = _buttonOff;
            }
        }
    }
}
