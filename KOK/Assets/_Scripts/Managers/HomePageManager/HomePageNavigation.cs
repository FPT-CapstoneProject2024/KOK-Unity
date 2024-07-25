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
    }
}
