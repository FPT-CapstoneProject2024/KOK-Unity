using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class TogglePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Toggle _toggle;

        void Start()
        {
            ToggleOnOffPanel();
        }

        public void ToggleOnOffPanel()
        {
            if (_toggle.isOn)
            {
                _panel.SetActive(true);
            }
            else if (!_toggle.isOn)
            {
                _panel.SetActive(false);
            }
        }
    }
}
