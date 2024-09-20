using KOK.ApiHandler.DTOModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ProfileBinding : MonoBehaviour
    {
        public Account Account { get; set; }
        [SerializeField] Image avatar;
        [SerializeField] TMP_Text UpLabel;
        [SerializeField] TMP_InputField UsernameInputField;
        [SerializeField] TMP_InputField EmailInputField;
        [SerializeField] TMP_Dropdown GenderDropdown;
        [SerializeField] TMP_InputField YearOfBirthInputField;
        [SerializeField] TMP_InputField PhoneNumberInputField;

        public void UpdateUI()
        {
            avatar.sprite = Resources.Load<Sprite>(Account.CharaterItemCode + "AVA");
            UsernameInputField.text = Account.UserName ?? string.Empty;
            EmailInputField.text = Account.Email ?? string.Empty;
            GenderDropdown.value = (int)Account.Gender - 1;
            YearOfBirthInputField.text = Account.Yob.ToString() ?? string.Empty;
            PhoneNumberInputField.text = Account.PhoneNumber ?? string.Empty;
            UpLabel.text = "" + (int)Account.UpBalance;
        }

        public void UpdateModel()
        {
            Account.UserName = UsernameInputField.text;
            //Account.Email = EmailInputField.text;
            Account.Gender = (AccountGender)GenderDropdown.value + 1;
            Account.PhoneNumber = PhoneNumberInputField.text;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
