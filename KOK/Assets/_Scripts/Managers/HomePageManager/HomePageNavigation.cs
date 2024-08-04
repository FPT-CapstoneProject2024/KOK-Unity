using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOK
{
    public class HomePageNavigation : MonoBehaviour
    {

        public void ToSingleKaraokeRoomScene()
        {
            SceneManager.LoadScene(sceneName: "SingleRoom");
        }
        
        public void ToMultipleKaraokeRoomScene()
        {
            SceneManager.LoadScene(sceneName: "MultipleRoom");
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
    }
}
