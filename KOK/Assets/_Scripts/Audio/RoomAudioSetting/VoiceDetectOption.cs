using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace KOK
{
    public class VoiceDetectOption : MonoBehaviour
    {
        [SerializeField] Slider _voiceDetectSlider;
        [SerializeField] int _defaultValue = 50;
        private Recorder _recorder;

        private void Awake()
        {
            _voiceDetectSlider.value = _defaultValue;
            
        }

        public void OnSliderValueChange()
        {
            if (_recorder == null)
            {
                _recorder = FindAnyObjectByType<Recorder>();
                //Debug.Log(_recorder.gameObject.name);
            }
            _recorder.VoiceDetectionThreshold = _voiceDetectSlider.value/100;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
