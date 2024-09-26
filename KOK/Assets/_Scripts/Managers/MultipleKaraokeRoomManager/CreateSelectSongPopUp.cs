using Fusion;
using KOK.ApiHandler.DTOModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class CreateSelectSongPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject _selectSongPopUpPrefab;
        [SerializeField] private GameObject _buySongPopUpPrefab;
        private Transform _parent;
        [SerializeField] private GameObject _originalItem;
        private GameObject _popUp;

        private NetworkRunner _runner;


        public void SpawnPopupSingle()
        {
            //var networkRunner = NetworkRunner.Instances[0];
            //Debug.LogError("Player role: " + networkRunner.GetPlayerObject(networkRunner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerRole);

            if (_parent == null) { _parent = GameObject.Find("PopUpCanvas").transform; }
            if (transform.childCount > 0)
            {
                foreach (Transform child in _parent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            if (_originalItem.GetComponentInChildren<SongBinding>().Song.isPurchased)
            {

                _popUp = Instantiate(_selectSongPopUpPrefab, _parent);
                //Song code
                _popUp.transform.GetChild(0).GetChild(0).name = _originalItem.transform.GetChild(0).name;
                var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
                _popUp.GetComponentInChildren<SongBinding>().BindingData(song);
            }
            else
            {
                Debug.Log("Song not purchased");
                var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)song.SongId,
                    SongName = song.SongName,
                    Price = song.Price,
                    IsPurchased = song.isPurchased,
                    SongItem = _originalItem
                };

                _popUp = Instantiate(_buySongPopUpPrefab, _parent);
                _popUp.GetComponentInChildren<PurchasedSongPopup>().InitParam(songParam, 0);
            }


        }
        public void SpawnPopUp()
        {
            if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
            if (_parent == null) { _parent = GameObject.Find("PopUpCanvas").transform; }
            if (transform.childCount > 0)
            {
                foreach (Transform child in _parent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            Debug.LogError(_runner);
            Debug.LogError(_runner.GetPlayerObject(_runner.LocalPlayer));
            Debug.LogError(_runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>());
            Debug.LogError(_runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerRole);

            if (_runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerRole != 0)
            {
                GetComponentInChildren<SongBinding>().SuggestSong();
                return;
            }
            if (IsHostOwnedSong(_originalItem.GetComponentInChildren<SongBinding>().Song.SongCode))
            {

                _popUp = Instantiate(_selectSongPopUpPrefab, _parent);
                //Song code
                _popUp.transform.GetChild(0).GetChild(0).name = _originalItem.transform.GetChild(0).name;
                var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
                _popUp.GetComponentInChildren<SongBinding>().BindingData(song);
                //Singer 
                var singer1Dropdown = _popUp.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                singer1Dropdown.ClearOptions();
                singer1Dropdown.AddOptions(GetPlayerNameList());

                var singer2Dropdown = _popUp.transform.GetChild(2).GetComponent<TMP_Dropdown>();
                singer2Dropdown.ClearOptions();
                singer2Dropdown.AddOptions(GetPlayerNameList());
            }
            else
            {
                Debug.Log("Song not purchased");
                var song = _originalItem.GetComponentInChildren<SongBinding>().Song;
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)song.SongId,
                    SongName = song.SongName,
                    Price = song.Price,
                    IsPurchased = song.isPurchased,
                    SongItem = _originalItem
                };
                _popUp = Instantiate(_buySongPopUpPrefab, _parent);
                _popUp.GetComponentInChildren<PurchasedSongPopup>().InitParam(songParam, 1);
            }
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

        private bool IsHostOwnedSong(string songCode)
        {
            if (_runner == null) _runner = NetworkRunner.Instances[0];
            //Debug.LogError(_runner + " | " + _runner.LocalPlayer + " | " + _runner.GetPlayerObject(_runner.LocalPlayer));

            return _runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().GetSongBySongCode(songCode).isPurchased;

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }

}
