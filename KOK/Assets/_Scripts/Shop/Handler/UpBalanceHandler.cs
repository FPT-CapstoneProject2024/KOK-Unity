using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class UpBalanceHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public TMP_Text UpBalanceText;

        private void OnEnable()
        {
            LoadUserUpBalance();
        }

        private void LoadUserUpBalance()
        {
            var accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (accountId.IsNullOrEmpty())
            {
                SetUpBalanceText("Tải số dư thất bại");
                return;
            }
            ApiHelper.Instance.GetComponent<AccountController>().GetAccountCoroutine(Guid.Parse(accountId), OnLoadAccountSuccess, OnLoadAccountError);
        }

        private void SetUpBalanceText(string text)
        {
            UpBalanceText.text = text;
        }

        private void OnLoadAccountSuccess(ResponseResult<Account> responseResult)
        {
            if (responseResult.Value == null || !(bool)responseResult.Result)
            {
                SetUpBalanceText("Tải số dư thất bại");
                return;
            }
            string upBalanceText = $"Số dư: {(int)responseResult.Value.UpBalance} UP";
            SetUpBalanceText(upBalanceText);
        }

        private void OnLoadAccountError(ResponseResult<Account> responseResult)
        {
            SetUpBalanceText("Tải số dư thất bại");
        }
    }
}
