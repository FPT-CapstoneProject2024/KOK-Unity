using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.VersionControl;
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
        private const string ER_SignUp_InvalidEmail = "Địa chỉ email không hợp lệ";
        private const string ER_SignUp_EmptyUsername = "Tên hiển thị không được trống";
        private const string ER_SignUp_UsernameLength = "Tên hiển thị phải có độ dài từ 6 - 15 ký tự";
        private const string ER_SignUp_UsernameRegex = "Tên hiển thị chỉ được chứa ký tự chữ hoặc số và không có khoảng cách";
        private const string ER_SignUp_EmptyPassword = "Mật khẩu không được trống";
        private const string ER_SignUp_PasswordLength = "Mật khẩu phải có độ dài từ 6 - 32 ký tự";
        private const string ER_SignUp_PasswordRegex = "Mật khẩu phải có ít nhât 1 ký tự chữ (hoa hoặc thường), 1 ký tự số, 1 ký tự đặc biệt và không có khoảng cách";
        private const string ER_SignUp_EmptyConfirmPassword = "Xác nhận mật khẩu không được trống";
        private const string ER_SignUp_InvalidConfirmPassword = "Xác nhận mật khẩu không trùng với mật khẩu";
        private const string ER_SignUp_InvalidGender = "Giới tính không hợp lệ";
        #endregion
        #region VerificationErrorMessage
        private const string ER_Verify_EmptyCode = "Mã xác thực không được trống";
        private const string ER_Verify_CodeRequiredLength = "Mã xác thực phải có độ dài là 6 ký tự";
        private const string ER_Verify_CodeRegex = "Mã xác thực phải có giá trị trong khoảng 100000 - 999999";
        private const string ER_Verify_EmptyEmail = "Email để xác thực không tồn tại";
        #endregion

        [Header("Fusion")]
        [SerializeField] private FusionManager fusionConnection;
        [Header("Canvases")]
        [SerializeField] private GameObject signUpCanvas;
        [SerializeField] private GameObject verificationCanvas;
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
        [Header("Verification Components")]
        [SerializeField] private TMP_InputField verificationCodeInputField;
        [SerializeField] private TMP_Text verificationErrorMessage;
        [SerializeField] private TMP_Text verificationMessage;

        private string VerificationEmail = string.Empty;

        private void Start()
        {
            SwitchToLogin();
            VerificationEmail = string.Empty;
        }

        #region UI

        public void SwitchToSignUp()
        {
            signUpCanvas.SetActive(true);
            gameObject.SetActive(false);
            verificationCanvas.SetActive(false);
            SetSignUpErrorMessage(string.Empty);
            VerificationEmail = string.Empty;
        }

        public void SwitchToLogin()
        {
            gameObject.SetActive(true);
            signUpCanvas.SetActive(false);
            verificationCanvas.SetActive(false);
            SetLoginErrorMessage(string.Empty);
            VerificationEmail = string.Empty;
        }

        public void SwitchToVerification()
        {
            verificationCanvas.SetActive(true);
            signUpCanvas.SetActive(false);
            gameObject.SetActive(false);
            SetVerificationErrorMessage(string.Empty);
            SetVerificationMessage(string.Empty);
            VerificationEmail = string.Empty;
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

        private void OnLoginSuccess(LoginResponse result)
        {
            // Check result
            if ((bool)!result.Result)
            {
                // Login failed
                SetLoginErrorMessage(result.Message);
                return;
            }

            Debug.Log("Login Success");

            // Set login data to PlayerPrefs
            PlayerPrefsHelper.DeleteLoginData();
            PlayerPrefsHelper.SetLoginData(result);

            // Set jwt token to APIHelper
            ApiHelper.Instance.SetJwtToken(result.AccessToken);

            // Validate user's account status
            if (result.Value.AccountStatus != AccountStatus.ACTIVE)
            {
                Debug.Log(ER_Login_NotVerifyUser);
                // Validation logic
                SendAndSwitchToVerification(new ResponseResult<Account>()
                {
                    Message = result.Message,
                    Result = result.Result,
                    Value = result.Value
                });
                return;
            }

            // Switch to home scene
            SceneManager.LoadScene("Home");
        }

        private void OnLoginError(LoginResponse result)
        {
            SetLoginErrorMessage(result.Message);
        }

        private void SetLoginErrorMessage(string errorMessage)
        {
            loginErrorMessage.text = errorMessage;
        }

        public void ClearLoginInputs()
        {
            loginEmailInputField.text = string.Empty;
            loginPasswordInputField.text = string.Empty;
            loginErrorMessage.text = string.Empty;
        }

        #endregion

        #region SignUp

        private void SetSignUpErrorMessage(string errorMessage)
        {
            signUpErrorMessage.text = errorMessage;
        }

        public void ClearSignUpInputs()
        {
            signUpEmailInputField.text = string.Empty;
            signUpUsernameInputField.text = string.Empty;
            signUpPasswordInputField.text = string.Empty;
            signUpConfirmPasswordInputField.text = string.Empty;
            signUpGenderDropDown.value = 0;
            signUpErrorMessage.text = string.Empty;
        }

        public void OnSignUpButtonClick()
        {
            var validateResult = ValidateSignUpData();
            if (validateResult == null) 
            {
                return;
            }
            ApiHelper.Instance.GetComponent<AuthenticationController>().SignUpCoroutine(validateResult, OnSignUpSuccess, OnSignUpError);
        }

        private void OnSignUpSuccess(ResponseResult<Account> responseResult)
        {
            if (!(bool)responseResult.Result)
            {
                SetSignUpErrorMessage(responseResult.Message);
                return;
            }

            // Switch to verification canvas
            SendAndSwitchToVerification(responseResult);
        }

        private void OnSignUpError(ResponseResult<Account> responseResult)
        {
            SetSignUpErrorMessage(responseResult.Message);
        }

        private MemberRegisterRequest ValidateSignUpData()
        {
            if (!ValidateSignUpEmail(signUpEmailInputField.text.Trim()))
            {
                return null;
            }
            if (!ValidateSignUpUsername(signUpUsernameInputField.text))
            {
                return null;
            }
            if (!ValidateSignUpPassword(signUpPasswordInputField.text))
            {
                return null;
            }
            if (!ValidateSignUpConfirmPassword(signUpConfirmPasswordInputField.text, signUpPasswordInputField.text))
            {
                return null;
            }
            if (!ValidateSignUpGender(signUpGenderDropDown.value))
            {
                return null;
            }
            return new MemberRegisterRequest()
            {
                Email = signUpEmailInputField.text.Trim(),
                Username = signUpUsernameInputField.text,
                Password = signUpPasswordInputField.text,
                Gender = (AccountGender)signUpGenderDropDown.value + 1,
            };
        }

        private bool ValidateSignUpEmail(string signUpEmail)
        {
            if (signUpEmail.IsNullOrEmpty())
            {
                SetSignUpErrorMessage(ER_SignUp_EmptyEmail);
                return false;
            }

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(signUpEmail))
            {
                SetSignUpErrorMessage(ER_SignUp_InvalidEmail);
                return false;
            }
            return true;
        }

        private bool ValidateSignUpUsername(string signUpUsername)
        {
            if (signUpUsername.IsNullOrEmpty())
            {
                SetSignUpErrorMessage(ER_SignUp_EmptyUsername);
                return false;
            }

            if (signUpUsername.Length < 6 || signUpUsername.Length > 15)
            {
                SetSignUpErrorMessage(ER_SignUp_UsernameLength);
                return false;
            }

            Regex usernameRegex = new Regex(@"^[A-Za-z\d]+$");
            if (!usernameRegex.IsMatch(signUpUsername))
            {
                SetSignUpErrorMessage(ER_SignUp_UsernameRegex);
                return false;
            }
            return true;
        }

        private bool ValidateSignUpPassword(string signUpPassword)
        {
            if (signUpPassword.IsNullOrEmpty())
            {
                SetSignUpErrorMessage(ER_SignUp_EmptyPassword);
                return false;
            }

            if (signUpPassword.Length < 6 || signUpPassword.Length > 32)
            {
                SetSignUpErrorMessage(ER_SignUp_PasswordLength);
                return false;
            }

            Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$");
            if (!passwordRegex.IsMatch(signUpPassword))
            {
                SetSignUpErrorMessage(ER_SignUp_PasswordRegex);
                return false;
            }
            return true;
        }

        private bool ValidateSignUpConfirmPassword(string confirmPassword, string signUpPassword)
        {
            if (confirmPassword.IsNullOrEmpty())
            {
                SetSignUpErrorMessage(ER_SignUp_EmptyConfirmPassword);
                return false;
            }

            if (confirmPassword != signUpPassword)
            {
                SetSignUpErrorMessage(ER_SignUp_InvalidConfirmPassword);
                return false;
            }
            return true;
        }

        private bool ValidateSignUpGender(int genderValue)
        {
            if (!Enum.IsDefined(typeof(AccountGender), genderValue + 1))
            {
                SetSignUpErrorMessage(ER_SignUp_InvalidGender);
                return false;
            }
            return true;
        }

        public void SendAndSwitchToVerification(ResponseResult<Account> responseResult)
        {
            SwitchToVerification();
            VerificationEmail = responseResult.Value.Email;
            ApiHelper.Instance.GetComponent<AuthenticationController>().SendVerificationCoroutine(VerificationEmail, OnSendVerificationSuccess, OnSendVerificationError);
        }

        #endregion

        #region Verification

        private void SetVerificationErrorMessage(string errorMessage)
        {
            verificationMessage.text = string.Empty;
            verificationErrorMessage.text = errorMessage;
        }

        private void SetVerificationMessage(string message)
        {
            verificationErrorMessage.text = string.Empty;
            verificationMessage.text = message;
        }

        private bool ValidateVerificationCode(string verificationCode)
        {
            if (verificationCode.IsNullOrEmpty())
            {
                SetVerificationErrorMessage(ER_Verify_EmptyCode);
                return false;
            }

            if(verificationCode.Length != 6)
            {
                SetVerificationErrorMessage(ER_Verify_CodeRequiredLength);
                return false;
            }

            Regex codeRegex = new Regex(@"^(100000|[1-9]\d{5}|[1-8]\d{5}|9\d{5}|999999)$");
            if (!codeRegex.IsMatch(verificationCode)) 
            {
                SetVerificationErrorMessage(ER_Verify_CodeRegex);
                return false;
            }
            return true;
        }

        public void OnVerifyButtonClick()
        {
            var validateResult = ValidateVerificationCode(verificationCodeInputField.text.Trim());
            if (!validateResult)
            {
                return;
            }
            MemberAccountVerifyRequest verifyRequest = new MemberAccountVerifyRequest() 
            {
                AccountEmail = VerificationEmail,
                VerifyCode = verificationCodeInputField.text.Trim()
            };
            ApiHelper.Instance.GetComponent<AuthenticationController>().VerifyMemberAccountCoroutine(verifyRequest, OnVerificationSuccess, OnVerificationError);
        }

        public void OnSendVerificationSuccess(ResponseResult<bool> responseResult)
        {
            if (!(bool)responseResult.Result)
            {
                SetVerificationErrorMessage(responseResult.Message);
                return;
            }
            SetVerificationMessage(responseResult.Message);
        }

        public void OnSendVerificationError(ResponseResult<bool> responseResult)
        {
            SetVerificationErrorMessage(responseResult.Message);
        }

        public void ResendVerificationCode()
        {
            if (string.IsNullOrEmpty(VerificationEmail))
            {
                SetVerificationErrorMessage(ER_Verify_EmptyEmail);
            }
            ApiHelper.Instance.GetComponent<AuthenticationController>().SendVerificationCoroutine(VerificationEmail, OnSendVerificationSuccess, OnSendVerificationError);
        }

        public void OnVerificationSuccess(ResponseResult<Account> responseResult)
        {
            if (!(bool)responseResult.Result)
            {
                SetVerificationErrorMessage(responseResult.Message);
                return;
            }

            PlayerPrefs.SetString(PlayerPrefsHelper.LoginDataKeys[10], responseResult.Value.AccountStatus.ToString());

            // Switch to login page if user is NOT logged-in
            if (ApiHelper.Instance.IsJwtTokenEmpty())
            {
                SwitchToLogin();
            }
            // Switch to home page if user is logged-in
            else
            {
                SceneManager.LoadScene("Home");
            }
        }

        public void OnVerificationError(ResponseResult<Account> responseResult)
        {
            SetVerificationErrorMessage(responseResult.Message);
        }

        #endregion
    }
}
