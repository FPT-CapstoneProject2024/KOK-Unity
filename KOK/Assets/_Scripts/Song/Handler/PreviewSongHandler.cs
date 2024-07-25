using UnityEngine;
using UnityEngine.Video;

namespace KOK
{
    public class PreviewSongHandler : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnClosePreviewSong()
        {
            gameObject.GetComponentInChildren<VideoPlayer>().Stop();
            gameObject.SetActive(false);
        }

        public void OnOpenPreviewSong(string songUrl)
        {
            gameObject.GetComponentInChildren<VideoPlayer>().url = songUrl;
            gameObject.SetActive(true);
            gameObject.GetComponentInChildren<VideoPlayer>().Play();
        }
    }
}
