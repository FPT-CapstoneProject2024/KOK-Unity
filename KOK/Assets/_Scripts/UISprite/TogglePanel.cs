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
        [SerializeField] private Transform _parent;

        void Start()
        {
            ToggleOnOffPanel();
            _parent = GameObject.Find("ItemPanelCanvas").transform;
        }

        public void ToggleOnOffPanel()
        {
            if (_toggle.isOn)
            {
                foreach(Transform panel in _parent.transform)
                {
                    panel.gameObject.SetActive(false);
                }
                _panel.SetActive(true);
            }
            else if (!_toggle.isOn)
            {
                _panel.SetActive(false);
            }
        }
    }
}
