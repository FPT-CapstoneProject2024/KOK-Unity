using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class InAppTransactionBinding : MonoBehaviour
    {
        public InAppTransaction InAppTransaction { get; set; }

        [SerializeField] TMP_Text typeLabel;
        [SerializeField] TMP_Text upAmountIncreaseLabel;
        [SerializeField] TMP_Text upAmountDecreaseLabel;
        [SerializeField] TMP_Text dateTimeLabel;
        [SerializeField] Button detailButton;

        [SerializeField] TMP_Text upBeforeTransactionLabel;
        [SerializeField] TMP_Text songOrItemNameLabel;

        [SerializeField] InAppTransactionBinding inAppTransactionDetailBinding;

        string songOrItemName = string.Empty;
        public void UpdateUI()
        {
            InAppTransaction ??= new();

            name = InAppTransaction.InAppTransactionId.ToString();

            if (typeLabel != null) typeLabel.text = InAppTransaction.TransactionType;
            if (InAppTransaction != null)
            {
                //Debug.Log(InAppTransaction.TransactionType + " | " + InAppTransactionType.BUY_SONG.ToString());
                if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_SONG.ToString())
                || InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_ITEM.ToString()))
                {
                    upAmountIncreaseLabel.text = string.Empty;
                    upAmountDecreaseLabel.text = "Up amount: " + (int)InAppTransaction.UpTotalAmount;
                }
                else
                {
                    upAmountIncreaseLabel.text = "Up amount: " + (int)InAppTransaction.UpTotalAmount;
                    upAmountDecreaseLabel.text = string.Empty;
                }
            }

            if (dateTimeLabel != null) dateTimeLabel.text = InAppTransaction.CreatedDate.ToString("dd/MM/yyyy\nHH:mm:ss");

            if (songOrItemNameLabel != null) songOrItemNameLabel.text = songOrItemName;
            if (upBeforeTransactionLabel != null) upBeforeTransactionLabel.text = (int)InAppTransaction.UpAmountBefore + "";
        
        }

        public void UpdateUIDetail()
        {
            inAppTransactionDetailBinding = ProfileManager.Instance.InAppTransactionDetailPanel;
            inAppTransactionDetailBinding.gameObject.SetActive(true);
            if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_SONG.ToString()))
            {
                ApiHelper.Instance.GetComponent<SongController>()
                    .GetSongByIdCoroutine((System.Guid)InAppTransaction.SongId,
                    (result) => {
                        songOrItemName = result.Value.SongName;
                        inAppTransactionDetailBinding.songOrItemName = songOrItemName;
                        inAppTransactionDetailBinding.InAppTransaction = InAppTransaction;
                        inAppTransactionDetailBinding.UpdateUI();

                    },
                    (ex) => { Debug.LogError(ex); }
                    );
            }

        }
    }
}
