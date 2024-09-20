using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace KOK.Assets._Scripts.Shop
{
    public class ItemPreviewDisplay : MonoBehaviour
    {
        public TMP_Text itemNameDisplay;
        public TMP_Text itemDescriptionDisplay;
        public TMP_Text itemBuyPriceDisplay;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowPopup(Item item)
        {
            gameObject.SetActive(true);
            Display(item);
        }

        public void HidePopup()
        {
            gameObject.SetActive(false);
        }

        public void Display(Item item)
        {
            itemNameDisplay.text = item.ItemName;
            itemDescriptionDisplay.text = item.ItemDescription;
            itemBuyPriceDisplay.text = item.ItemBuyPrice.ToString() + " Stars";
        }
    }
}
