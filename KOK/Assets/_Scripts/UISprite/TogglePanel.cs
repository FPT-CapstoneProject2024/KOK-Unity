using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            var tmp = new List<TogglePanel>();
            foreach (var panel in _groupTogglePanel) { tmp.Add(panel); }
            foreach (TogglePanel togglePanel in tmp)
            {
                if (togglePanel == this)
                {
                    _groupTogglePanel.Remove(togglePanel);
                }
            }
            ToggleOffPanel();
            
        }

        public void ToggleOnOffPanel()
        {
            Debug.Log("ToggleOnOffPanel " + _panel.name);
            if (_toggle.isOn)
            {
                foreach (var togglePanel in _groupTogglePanel)
                {
                    if (togglePanel != this) { togglePanel.ToggleOffPanel(); }

                }
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
