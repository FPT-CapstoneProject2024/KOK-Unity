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
        [SerializeField] TMP_InputField UsernameInputField;
        [SerializeField] TMP_InputField EmailInputField;
        [SerializeField] TMP_Dropdown GenderDropdown;
        [SerializeField] TMP_InputField YearOfBirthInputField;
        [SerializeField] TMP_InputField PhoneNumberInputField;

        public void UpdateUI()
        {
            UsernameInputField.text = Account.UserName??string.Empty;
            EmailInputField.text = Account.Email ?? string.Empty;
            GenderDropdown.value = (int)Account.Gender;
            YearOfBirthInputField.text = Account.Yob.ToString() ?? string.Empty;
            PhoneNumberInputField.text = Account.PhoneNumber ?? string.Empty;
        }

        public void UpdateModel()
        {
            Account.UserName = UsernameInputField.text;
            Account.Email = EmailInputField.text;
        }
    }
}
