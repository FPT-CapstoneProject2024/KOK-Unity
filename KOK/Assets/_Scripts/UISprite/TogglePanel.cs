using System;
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
        [SerializeField] private List<TogglePanel> _groupTogglePanel;

        void Start()
        {
            ToggleOnOffPanel();
        }

        public void ToggleOnOffPanel()
        {
            foreach (var togglePanel in _groupTogglePanel)
            {
                if (togglePanel != this) { togglePanel.ToggleOffPanel(); }

            }
            if (_toggle.isOn)
            {
                _panel.SetActive(true);
            }
            else if (!_toggle.isOn)
            {
                _panel.SetActive(false);
            }
        }

        public void ToggleOffPanel()
        {
            try
            {
                _toggle.isOn = false;
                _panel.SetActive(false);
            }
            catch (Exception) { }
        }
    }
}
