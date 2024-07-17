using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace KOK
{
    public class AuthenticationHandler : MonoBehaviour
    {
        #region LoginErrorMessage
        private const string ER_Login_EmptyEmail = "Địa chỉ email không được trống";
        private const string ER_Login_InvalidEmail = "Địa chỉ email không hợp lệ";
        private const string ER_Login_EmptyPassword = "Mật khẩu không được trống";
        private const string ER_Login_NotVerifyUser = "Tài khoản của người dùng chưa được xác thực, vui lòng xác thực tài khoản";
        #endregion
        #region SignUpErrorMessage
        private const string ER_SignUp_EmptyEmail = "Địa chỉ email không được trống";
        #endregion

        [Header("Fusion")]
        [SerializeField] private FusionManager fusionConnection;
        [Header("Canvases")]
        [SerializeField] private GameObject signUpCanvas;
        [Header("Login Components")]
        [SerializeField] private TMP_InputField loginEmailInputField;
        [SerializeField] private TMP_InputField loginPasswordInputField;
        [SerializeField] private TMP_Text loginErrorMessage;
        [Header("Sign Up Components")]
        [SerializeField] private TMP_InputField signUpEmailInputField;
        [SerializeField] private TMP_InputField signUpUsernameInputField;
        [SerializeField] private TMP_InputField signUpPasswordInputField;
        [SerializeField] private TMP_InputField signUpConfirmPasswordInputField;
        [SerializeField] private TMP_Dropdown signUpGenderDropDown;
        [SerializeField] private TMP_Text signUpErrorMessage;

        private void Start()
        {
            loginErrorMessage.text = string.Empty;
            gameObject.SetActive(true);
            signUpCanvas.SetActive(false);
        }

        #region UI

        public void SwitchToSignUp()
        {
            signUpCanvas.SetActive(true);
            gameObject.SetActive(false);
            SetSignUpErrorMessage(string.Empty);
        }

        public void SwitchToLogin()
        {
            gameObject.SetActive(true);
            signUpCanvas.SetActive(false);
            SetLoginErrorMessage(string.Empty);
        }

        #endregion

        #region Login

        public void OnLoginButtonClick()
        {
            //this.gameObject.SetActive(false);
            //fusionConnection.OnLoginSuccess();
            var validateResult = ValidateLoginCredentials(loginEmailInputField.text.Trim(), loginPasswordInputField.text);
            if (validateResult == null)
            {
                return;
            }
            ApiHelper.Instance.gameObject.GetComponent<AuthenticationController>().LoginCoroutine(validateResult, OnLoginSuccess, OnLoginError);
        }

        private LoginRequest ValidateLoginCredentials(string loginEmail, string loginPassword)
        {
            // Validate email
            if (loginEmail.IsNullOrEmpty())
            {
                SetLoginErrorMessage(ER_Login_EmptyEmail);
                return null;
            }

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(loginEmail))
            {
                SetLoginErrorMessage(ER_Login_InvalidEmail);
                return null;
            }

            // Validate password
            if (loginPassword.IsNullOrEmpty())
            {
                SetLoginErrorMessage(ER_Login_EmptyPassword);
                return null;
            }

            return new LoginRequest()
            {
                Email = loginEmail,
                Password = loginPassword
            };
        }

        public void OnLoginSuccess(LoginResponse result)
        {
            // Check result
            if ((bool)!result.Result)
            {
                // Login failed
                SetLoginErrorMessage(result.Message);
                return;
            }
            // Validate user's account status
            if (result.Value.AccountStatus != AccountStatus.ACTIVE)
            {
                Debug.Log(ER_Login_NotVerifyUser);
                // Validation logic
            }

            Debug.Log("Login Success");

            // Set login data to PlayerPrefs
            PlayerPrefsHelper.DeleteLoginData();
            PlayerPrefsHelper.SetLoginData(result);

            // Set jwt token to APIHelper
            ApiHelper.Instance.SetJwtToken(result.AccessToken);

            // Switch to home scene
            SceneManager.LoadScene("Home");
        }

        public void OnLoginError(LoginResponse result)
        {
            SetLoginErrorMessage(result.Message);
        }

        private void SetLoginErrorMessage(string errorMessage)
        {
            loginErrorMessage.text = errorMessage;
        }

        #endregion

        #region SignUp

        private void SetSignUpErrorMessage(string errorMessage)
        {
            signUpErrorMessage.text = errorMessage;
        }

        #endregion
    }
}
