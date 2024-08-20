using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class ShowPanelButton : MonoBehaviour
    {
        [SerializeField] GameObject panelToShow;
        [SerializeField] GameObject panelToHide;

        public void Show()
        {
            panelToShow.SetActive(true);
            Debug.Log("Show");
        }

        public void Hide() {
            panelToHide.SetActive(false);
        }

        public void ShowThenHide()
        {
            panelToShow.SetActive(true);
            panelToHide.SetActive(false);
        }
    }
}
