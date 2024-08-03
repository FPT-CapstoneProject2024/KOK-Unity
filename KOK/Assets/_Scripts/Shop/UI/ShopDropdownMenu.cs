using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOK
{
    public class ShopDropdownMenu : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private GameObject upPackagesCanvas;

        private TMP_Dropdown dropdownMenu;

        void Awake()
        {
            dropdownMenu = GetComponent<TMP_Dropdown>();
        }

        private void SwitchToUpPackages()
        {
            upPackagesCanvas.SetActive(true);
            upPackagesCanvas.GetComponentInChildren<UpPackageHandler>().SetInitialState();
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
                    SwitchToUpPackages();
                    break;
                case 1:
                    SwitchToHome();
                    break;
            }
        }
    }
}
