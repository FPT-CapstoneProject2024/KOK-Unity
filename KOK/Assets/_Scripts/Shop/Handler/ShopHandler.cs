using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ShopHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public GameObject itemContainer;
        [SerializeField] public GameObject itemTemplate;
        [SerializeField] public TMP_Text itemListMessage;
        [Header("Paging Components")]
        [SerializeField] public TMP_Text pagingDisplay;
        [SerializeField] public Button previousButton;
        [SerializeField] public Button nextButton;
        [Header("Search Components")]
        [SerializeField] public TMP_InputField searchInput;
        [SerializeField] public TMP_Dropdown categoryDropdown;
        [Header("Loading Components")]
        [SerializeField] public LoadingManager loadingManager;

        private int currentPage = 1;
        private int totalPage = 1;
        private string searchKeyword = string.Empty;

        public void SetInitialState()
        {
            currentPage = 1;
            totalPage = 1;
            //filter = new SongFilter();
            searchKeyword = string.Empty;
            searchInput.text = searchKeyword;
            ClearContainer();
            SetShopItemMessage(string.Empty);
            DisableButton(previousButton);
            DisableButton(nextButton);
            pagingDisplay.text = $"{0}/{0}";
        }

        private void ClearContainer()
        {
            if (itemContainer.transform.childCount > 0)
            {
                while (itemContainer.transform.childCount > 0)
                {
                    DestroyImmediate(itemContainer.transform.GetChild(0).gameObject);
                }
            }
        }

        private void SetShopItemMessage(string message)
        {
            itemListMessage.text = message;
        }

        private void EnableButton(Button button)
        {
            button.enabled = true;
            button.gameObject.GetComponent<Image>().color = Color.white;
        }

        private void DisableButton(Button button)
        {
            button.enabled = false;
            button.gameObject.GetComponent<Image>().color = Color.gray;
        }
    }
}
