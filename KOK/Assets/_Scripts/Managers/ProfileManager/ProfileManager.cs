using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
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

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
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
                    (ex) => {}
                );
        }

        public void SaveMemberInformation()
        {
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
                    },
                    (result) =>
                    {
                        Debug.LogError(result);
                    }

                );
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
