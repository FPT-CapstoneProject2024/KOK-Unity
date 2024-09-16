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
        public static bool isToPurchasedSong = false;
        public static bool isToTransaction = false;

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

    }
}
