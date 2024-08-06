using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOK
{
    public class ShopDropdownMenu : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private GameObject upPackagesCanvas;
        [SerializeField] private GameObject shopCanvas;

        private TMP_Dropdown dropdownMenu;

        void Awake()
        {
            dropdownMenu = GetComponent<TMP_Dropdown>();
        }

        private void SwitchToUpPackages()
        {
            upPackagesCanvas.SetActive(true);
            upPackagesCanvas.GetComponentInChildren<UpPackageHandler>().SetInitialState();
            shopCanvas.SetActive(false);
        }

        private void SwitchToHome()
        {
            SceneManager.LoadScene("Home");
        }

        public void OnMenuValueChange()
        {
            switch (dropdownMenu.value)
            {
                case 0:
                    SwitchToShop();
                    break;
                case 1:
                    SwitchToUpPackages();
                    break;
                case 2:
                    SwitchToHome();
                    break;
            }
        }

        private void SwitchToShop()
        {
            shopCanvas.SetActive(true);
            //upPackagesCanvas.GetComponentInChildren<UpPackageHandler>().SetInitialState();
            upPackagesCanvas.SetActive(false);
        }
    }
}
