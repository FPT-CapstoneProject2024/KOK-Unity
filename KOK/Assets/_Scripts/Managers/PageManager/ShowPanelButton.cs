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
