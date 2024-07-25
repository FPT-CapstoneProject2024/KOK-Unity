using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK.UISprite
{
    public class ToggleSwapSprite : MonoBehaviour
    {
        [SerializeField] private Sprite _toggleOn;
        [SerializeField] private Sprite _toggleOff;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Image _toggleBackground;

        void Start ()
        {
            ToggleSprite();
        }
        public void ToggleSprite()
        {
            if (_toggle.isOn)
            {
                _toggleBackground.sprite = _toggleOn;
            }
            else if (!_toggle.isOn)
            {
                _toggleBackground.sprite = _toggleOff;
            }
        }
    }
}
