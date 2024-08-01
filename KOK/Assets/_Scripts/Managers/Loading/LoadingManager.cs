using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] List<Selectable> uiElementsNeedToDisable = new();
        [SerializeField] GameObject loadingSymbol;

        private void OnEnable()
        {
            //loadingSymbol.SetActive(false);
        }

        public void DisableUIElement()
        {
            loadingSymbol.SetActive(true);
            foreach (Selectable u in uiElementsNeedToDisable)
            {
                u.interactable = false;
            }
        }

        public void EnableUIElement()
        {
            loadingSymbol.SetActive(false);
            foreach (Selectable u in uiElementsNeedToDisable) { u.interactable = true; }
        }

        public void EnableLoadingSymbol()
        {
            loadingSymbol.SetActive(true);
        }

        public void DisableLoadingSymbol()
        {
            loadingSymbol.SetActive(false);
        }
    }
}
