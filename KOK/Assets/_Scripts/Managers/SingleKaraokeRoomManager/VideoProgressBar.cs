using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK
{
    public class VideoProgressBar : MonoBehaviour
    {
        [SerializeField] VideoPlayer videoPlayer;
        [SerializeField] Slider progressBar;

        void OnEnable()
        {
            if (videoPlayer == null)
            {
                videoPlayer = FindAnyObjectByType<VideoPlayer>();
            }
            if (progressBar == null)
            {
                progressBar = GetComponent<Slider>();
            }
            StartCoroutine(UpdateProgressBar());
        }

        IEnumerator UpdateProgressBar()
        {
            yield return new WaitForSeconds(0.2f);
            if (videoPlayer.length > 0)
            {
                progressBar.value = (float)(videoPlayer.time / videoPlayer.length);
            }
            //Debug.LogError(videoPlayer.time + " | " + videoPlayer.length + " | " + progressBar.value.ToString());
            StartCoroutine(UpdateProgressBar());
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
