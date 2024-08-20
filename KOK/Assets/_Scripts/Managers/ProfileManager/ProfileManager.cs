using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.InAppTransaction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class ProfileManager : MonoBehaviour
    {
        public static ProfileManager Instance { get; private set; }

        [SerializeField] List<TMP_InputField> inputFieldForm = new List<TMP_InputField>();
        [SerializeField] ProfileBinding profileBinding;

        [SerializeField] Transform inAppTransactionPanel;
        [SerializeField] Transform inAppTransactionContentViewPort;
        [SerializeField] GameObject inAppTransactionItemPrefab;


        [SerializeField] public InAppTransactionBinding InAppTransactionDetailPanel;

        [SerializeField] GameObject loading;
        [SerializeField] AlertManager messageAlert;


        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else { Destroy(gameObject); }
        }

        public void Start()
        {
            LoadMemberInformation();
        }

        public void LoadMemberInformation()
        {
            ApiHelper.Instance.GetComponent<AccountController>()
                .GetAccountByIdCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (account) =>
                    {
                        profileBinding.Account = account;
                        Debug.Log("111" + account);
                        profileBinding.UpdateUI();
                        PlayerPrefsHelper.SetProfileData(account);
                    },
                    (ex) => { }
                );
        }

        public void SaveMemberInformation()
        {
            loading.SetActive(true);
            profileBinding.UpdateModel();
            var account = profileBinding.Account;
            UpdateAccountRequest updateAccountRequest = new UpdateAccountRequest()
            {
                UserName = account.UserName,
                //Email = account.Email,
                Gender = account.Gender.ToString(),
                UpBalance = (decimal)account.UpBalance,
                //Yob = account.Yob,
                PhoneNumber = account.PhoneNumber,
                CharacterItemId = account.CharacterItemId,
                RoomItemId = account.RoomItemId,

            };
            ApiHelper.Instance.GetComponent<AccountController>()
                .UpdateAccountCoroutine(
                    (Guid)account.AccountId,
                    updateAccountRequest,
                    (result) =>
                    {
                        PlayerPrefsHelper.SetProfileData(result.Value);
                        Debug.Log("Save data success: " + result.Value);
                        messageAlert.Alert("Lưu thông tin thành công!", true);
                        loading.SetActive(false);
                    },
                    (result) =>
                    {
                        Debug.LogError(result);
                        messageAlert.Alert("Lưu thông tin thất bại!", true);
                        loading.SetActive(false);
                    }

                );
        }

        public void GetInAppTransactionList()
        {
            loading.SetActive(true);
            List<InAppTransaction> inAppTransactions = new();
            ApiHelper.Instance.GetComponent<InAppTransactionController>()
                .GetInAppTransactionsByMemberIdCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (result) =>
                    {
                        inAppTransactions = result;
                        //Debug.Log(inAppTransactions.Count);
                        ShowInAppTransactionPanel(inAppTransactions);
                        loading.SetActive(false);
                    },
                    (ex) =>
                    {
                        Debug.LogError(ex);
                        loading.SetActive(false);
                    }
                );
        }

        private void ShowInAppTransactionPanel(List<InAppTransaction> inAppTransactions)
        {
            inAppTransactionPanel.gameObject.SetActive(true);
            foreach (Transform child in inAppTransactionContentViewPort)
            {
                Destroy(child.gameObject);
            }
            Debug.Log(inAppTransactions.Count);
            foreach (InAppTransaction transaction in inAppTransactions)
            {
                var inAppTransactionItem = Instantiate(inAppTransactionItemPrefab, inAppTransactionContentViewPort);
                inAppTransactionItem.GetComponentInChildren<InAppTransactionBinding>().InAppTransaction = transaction;
                inAppTransactionItem.GetComponentInChildren<InAppTransactionBinding>().UpdateUI();

            }
        }


        public void PopUpCharacterSelect()
        {

        }

        public void ChangeCharacter(string characterCode)
        {

        }

        public void PopUpRoomSelect()
        {

        }

        public void ChangeRoom(string roomCode)
        {

        }

    }

}
