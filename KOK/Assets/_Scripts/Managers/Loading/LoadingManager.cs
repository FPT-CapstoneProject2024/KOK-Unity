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
            try
            {
                loadingSymbol.SetActive(true);
                foreach (Selectable u in uiElementsNeedToDisable)
                {
                    u.interactable = false;
                }
            }
            catch { }
        }

        public void EnableUIElement()
        {
            try
            {
                loadingSymbol.SetActive(false);
                foreach (Selectable u in uiElementsNeedToDisable) { u.interactable = true; }
            }
            catch { }
        }

        public void EnableLoadingSymbol()
        {
            loadingSymbol.SetActive(true);
        }

        public void DisableLoadingSymbol()
        {
            loadingSymbol.SetActive(false);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
