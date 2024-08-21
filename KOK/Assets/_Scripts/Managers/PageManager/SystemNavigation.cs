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
        public void ToHome()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_home");
        }

        public void ToSingleKaraokeRoomScene()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SceneManager.LoadScene(sceneName: "SingleRoom");
        }

        public void ToMultipleKaraokeRoomScene()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SceneManager.LoadScene(sceneName: "MultipleRoom");
        }

        public void ToViewRecordingMenuScene()
        {
            SceneManager.LoadScene(sceneName: "RecordingMenuScene");
        }

        public void ToEditScene()
        {
            SceneManager.LoadScene(sceneName: "Quang");
        }

        public void ToSongsScene()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_song");
        }

        public void ToPurchasedSongScene()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            isToPurchasedSong = true;
            SceneManager.LoadScene(sceneName: "v_song");

        }

        public void ToShopScene()
        {
            SceneManager.LoadScene(sceneName: "Shop");
        }

        public void ToLogin()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "Login");
            PlayerPrefsHelper.DeleteAll();
        }
        public void ToProfile()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_profile");
        }

        public void ToSingScene()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(sceneName: "v_sing");
        }

    }
}
