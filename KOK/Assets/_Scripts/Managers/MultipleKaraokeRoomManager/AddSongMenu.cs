using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class AddSongMenu : MonoBehaviour
    {
        public TMP_Dropdown Singer1DropDown;
        public TMP_Dropdown Singer2DropDown;

        void Start ()
        {
           
        }
        public void OnButtonAddSongClick()
        {
            FusionManager.Instance.AddSongToQueue(GetComponentInChildren<SongBinding>().Song.SongCode,
                                                    Singer1DropDown.options[Singer1DropDown.value].text,
                                                    Singer2DropDown.options[Singer2DropDown.value].text);
            Destroy(gameObject);
        }

        public void OnButtonPrioritizeSongClick() {
            FusionManager.Instance.PrioritizeSongToQueue(GetComponentInChildren<SongBinding>().Song.SongCode,
                                                    Singer1DropDown.options[Singer1DropDown.value].text,
                                                    Singer2DropDown.options[Singer2DropDown.value].text);
            Destroy(gameObject);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
