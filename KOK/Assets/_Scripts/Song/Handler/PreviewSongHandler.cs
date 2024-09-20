using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace KOK
{
    public class PreviewSongHandler : MonoBehaviour
    {
        void Start()
        {
            //gameObject.SetActive(false);
        }

        public void OnClosePreviewSong()
        {
            StopAllCoroutines();
            gameObject.GetComponentInChildren<VideoPlayer>().Stop();
            gameObject.SetActive(false);
        }

        public void OnOpenPreviewSong(string songUrl)
        {
            gameObject.SetActive(true);
            gameObject.GetComponentInChildren<VideoPlayer>().url = songUrl;
            gameObject.GetComponentInChildren<VideoPlayer>().Play();
            StartCoroutine(AutoStopVideo(45));
        }

        IEnumerator AutoStopVideo(int second)
        {
            yield return new WaitForSeconds(second);
            gameObject.GetComponentInChildren<VideoPlayer>().Stop();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
