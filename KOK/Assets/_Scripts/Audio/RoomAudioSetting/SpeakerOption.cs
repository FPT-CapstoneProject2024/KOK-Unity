using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class SpeakerOption : MonoBehaviour
    {
        [SerializeField] Slider _speakerSlider;
        [SerializeField] int _defaultValue = 50;
        private List<AudioSource> _speakerAudioSource;

        private void Start()
        {
            if (_speakerSlider == null)
            {
                _speakerSlider = GetComponent<Slider>();
            }
            _speakerSlider.value = _defaultValue;
            _speakerAudioSource = new();
        }

        public void OnSliderValueChange()
        {
            _speakerAudioSource = FindObjectsByType<AudioSource>(FindObjectsSortMode.InstanceID).ToList();
            foreach (var audioSource in _speakerAudioSource)
            {
                audioSource.volume = _speakerSlider.value / 100;
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
