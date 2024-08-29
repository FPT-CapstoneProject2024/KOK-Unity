using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransactions;
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

        [SerializeField] AlertManager messageAlert;

        string songOrItemName = string.Empty;
        public void UpdateUI()
        {
            InAppTransaction ??= new();

            name = InAppTransaction.InAppTransactionId.ToString();

            if (InAppTransaction != null)
            {
                if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_SONG.ToString())
                || InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_ITEM.ToString()))
                {
                    upAmountIncreaseLabel.text = string.Empty;
                    upAmountDecreaseLabel.text = "Số lượng up: " + (int)InAppTransaction.UpTotalAmount;
                }
                else
                {
                    upAmountIncreaseLabel.text = "Số lượng UP: " + (int)InAppTransaction.UpTotalAmount;
                    upAmountDecreaseLabel.text = string.Empty;
                }
            }

            if (dateTimeLabel != null) dateTimeLabel.text = "Ngày: " + InAppTransaction.CreatedDate.ToString("dd/MM/yyyy\nGiờ: HH:mm:ss");

            if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_SONG.ToString()))
            {
                if (typeLabel != null) typeLabel.text = "Giao dịch mua bài hát";
                if (songOrItemNameLabel != null) songOrItemNameLabel.text = "Bài hát: " + songOrItemName;
            }
            else if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_ITEM.ToString()))
            {
                if (typeLabel != null) typeLabel.text = "Giao dịch mua vật phẩm";
                if (songOrItemNameLabel != null) songOrItemNameLabel.text = "Vật phẩm: " + songOrItemName;
            }

            if (upBeforeTransactionLabel != null) upBeforeTransactionLabel.text = "Số dư trước: " + (int)InAppTransaction.UpAmountBefore + "";

        }

        public void UpdateUIDetail()
        {
            inAppTransactionDetailBinding = ProfileManager.Instance.InAppTransactionDetailPanel;
            if (InAppTransaction.TransactionType.Equals(InAppTransactionType.BUY_SONG.ToString()))
            {
                ApiHelper.Instance.GetComponent<SongController>()
                    .GetSongByIdCoroutine((System.Guid)InAppTransaction.SongId,
                    (result) =>
                    {
                        songOrItemName = result.Value.SongName;
                        inAppTransactionDetailBinding.gameObject.SetActive(true);
                        inAppTransactionDetailBinding.songOrItemName = songOrItemName;
                        inAppTransactionDetailBinding.InAppTransaction = InAppTransaction;
                        inAppTransactionDetailBinding.UpdateUI();

                    },
                    (ex) =>
                    {
                        Debug.LogError(ex);
                        messageAlert = FindAnyObjectByType<AlertManager>();
                        messageAlert.Alert("Tải thông tin thất bại!", false);
                    }
                    );
            }

        }
    }
}
