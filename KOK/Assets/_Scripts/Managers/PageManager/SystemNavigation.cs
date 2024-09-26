using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOK
{
    public class SystemNavigation : MonoBehaviour
    {
        private static bool isToPurchasedSong = false;
        private static bool isToTransaction = false;
        private static bool isToPackage = false;

        public static bool IsToPurchasedSong()
        {
            if (isToPurchasedSong)
            {
                isToPurchasedSong = false;
                return true;
            }
            return false;
        }
        public static bool IsToTransaction()
        {
            if (isToTransaction)
            {
                isToTransaction = false;
                return true;
            }
            return false;
        }
        public static bool IsToPackage()
        {
            if (isToPackage)
            {
                isToPackage = false;
                return true;
            }
            return false;
        }

        private void ResetChecker()
        {
            isToPurchasedSong = false;
            isToTransaction = false;
        }
        public void ToHome()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_home");
        }

        public void ToSingleKaraokeRoomScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SceneManager.LoadScene(sceneName: "SingleRoom");
        }

        public void ToMultipleKaraokeRoomScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SceneManager.LoadScene(sceneName: "MultipleRoom");
        }

        public void ToViewRecordingMenuScene()
        {
            ResetChecker();
            SceneManager.LoadScene(sceneName: "RecordingMenuScene");
        }

        public void ToEditScene()
        {
            ResetChecker();
            SceneManager.LoadScene(sceneName: "Quang");
        }

        public void ToSongsScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_song");
        }

        public void ToPurchasedSongScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            isToPurchasedSong = true;
            SceneManager.LoadScene(sceneName: "v_song");

        }

        public void ToShopScene()
        {
            ResetChecker();
            SceneManager.LoadScene(sceneName: "v_shop");
        }

        public void ToLogin()
        {
            ResetChecker();
            NotificationManager.Instance.DisconnectFromHub();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "Login");
            PlayerPrefsHelper.DeleteAll();
        }
        public void ToProfile()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_profile");
        }

        public void ToSingScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_sing");
        }

        public void ToNotificationScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_notification");
        }

        public void ToTransactionScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            isToTransaction = true;
            SceneManager.LoadScene(sceneName: "v_profile");
        }

        public void ToMessageScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            isToTransaction = true;
            SceneManager.LoadScene(sceneName: "v_messenger");
        }

        public void ToPackageScene()
        {
            ResetChecker();
            Screen.orientation = ScreenOrientation.Portrait;
            isToPackage = true;
            SceneManager.LoadScene(sceneName: "v_profile");
        }
    }
}
