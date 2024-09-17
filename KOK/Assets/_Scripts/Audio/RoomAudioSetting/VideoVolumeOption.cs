using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK
{
    public class VideoVolumeOption : MonoBehaviour
    {
        [SerializeField] Slider _videoVolumeSlider;
        [SerializeField] int _defaultValue = 50;
        [SerializeField] private VideoPlayer _videoPlayer;

        void OnEnable()
        {
            _videoPlayer = FindFirstObjectByType<VideoPlayer>();
            _videoVolumeSlider.value = _defaultValue;
            OnSliderValueChange();
        }

        public void OnSliderValueChange()
        {
            _videoPlayer.SetDirectAudioVolume(0, _videoVolumeSlider.value / 100f);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
