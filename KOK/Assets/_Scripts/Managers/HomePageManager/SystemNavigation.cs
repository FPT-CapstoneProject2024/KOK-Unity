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

        public void ToHome()
        {
            SceneManager.LoadScene(sceneName: "Home");
        }

        public void ToSingleKaraokeRoomScene()
        {
            SceneManager.LoadScene(sceneName: "SingleRoom");
        }

        public void ToMultipleKaraokeRoomScene()
        {
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
            SceneManager.LoadScene(sceneName: "Song");
        }

        public void ToShopScene()
        {
            SceneManager.LoadScene(sceneName: "Shop");
        }

        public void ToLogin()
        {
            SceneManager.LoadScene(sceneName: "Login");
        }
        public void ToProfile()
        {
            SceneManager.LoadScene(sceneName: "Profile");
        }

    }
}
