using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class CreateSelectSongPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject _popUpPrefab;
        private Transform _parent;
        [SerializeField] private GameObject _originalItem;
        private GameObject _popUp;

        private void Start()
        {
            _parent = GameObject.Find("PopUpCanvas").transform;
        }

        public void SpawnPopUp()
        {
            if (transform.childCount > 0) {
                foreach (Transform child in _parent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            _popUp = Instantiate(_popUpPrefab, _parent);
            //Song code
            _popUp.transform.GetChild(0).GetChild(0).name = _originalItem.transform.GetChild(0).name;
            var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
            _popUp.GetComponentInChildren<SongBinding>().BindingData(song);
            //Singer 1
            if (IsHostOwnedSong())
            {
                var singer1Dropdown = _popUp.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                singer1Dropdown.ClearOptions();
                singer1Dropdown.AddOptions(GetPlayerNameList());
            }
            else
            {
                var singer1Dropdown = _popUp.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                singer1Dropdown.ClearOptions();
                singer1Dropdown.AddOptions(GetPlayerNameListThatOwnedSong());
            }

            var singer2Dropdown = _popUp.transform.GetChild(2).GetComponent<TMP_Dropdown>();
            singer2Dropdown.ClearOptions();
            singer2Dropdown.AddOptions(GetPlayerNameList());

        }

        public void SpawnPopupSingle()
        {
            if (transform.childCount > 0)
            {
                foreach (Transform child in _parent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            _popUp = Instantiate(_popUpPrefab, _parent);
            //Song code
            _popUp.transform.GetChild(0).GetChild(0).name = _originalItem.transform.GetChild(0).name;
            var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
            _popUp.GetComponentInChildren<SongBinding>().BindingData(song);
        }
        private List<string> GetPlayerNameList()
        {
            NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();
            List<string> runnerNames = new List<string>();
            foreach (var player in runner.ActivePlayers)
            {
                PlayerNetworkBehavior playerNetworkBehavior = runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>();
                runnerNames.Add(playerNetworkBehavior.PlayerName.ToString());
            }
            return runnerNames;
        }

        private List<string> GetPlayerNameListThatOwnedSong()
        {
            NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();
            List<string> runnerNames = new List<string>();
            foreach (var player in runner.ActivePlayers)
            {
                PlayerNetworkBehavior playerNetworkBehavior = runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>();
                //Call check owned song function here
                runnerNames.Add(playerNetworkBehavior.PlayerName.ToString());
            }
            return runnerNames;
        }

        private bool IsHostOwnedSong()
        {
            //Add check host owned song here
            return true;
        }
    }

}
