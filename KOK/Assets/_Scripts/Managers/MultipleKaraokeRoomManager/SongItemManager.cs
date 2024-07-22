using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace KOK
{
    public class SongItemManager : MonoBehaviour
    {

        [SerializeField] GameObject _viewportContent;
        [SerializeField] GameObject _songHolderPrefab;
        [SerializeField] TMP_InputField _searchSongInput;
        [SerializeField] Toggle _favToggle;

        private NetworkRunner _runner;

        public void UpdateSongList()
        {
            try
            {
                if (_viewportContent == null) { return; }
                if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
                foreach (Transform child in _viewportContent.transform)
                {
                    Destroy(child.gameObject);
                }

                //List<SongDetail> songList = SongManager.songs;
                List<SongDetail> songList = _runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().SongList;
                

                string searchKeyword = _searchSongInput.text;
                if (!searchKeyword.IsNullOrEmpty())
                {
                    songList = songList.Where(s => s.SongName.ContainsInsensitive(searchKeyword)
                                                || s.Artist.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)
                                                || s.Singer.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)
                                                || s.Genre.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)).ToList();
                }

                //if (_favToggle.isOn)
                //{
                //    songList = songList.Where(s => _runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().FavoriteSongList.FirstOrDefault(f => f.SongId == s.SongId) != null).ToList();
                //}

                foreach (var song in songList)
                {
                    try
                    {
                        GameObject songHolder = Instantiate(_songHolderPrefab, _viewportContent.transform);
                        songHolder.name = song.SongName;
                        songHolder.GetComponentInChildren<SongBinding>().BindingData(song);
                        songHolder.transform.GetChild(0).name = song.SongId.ToString();
                        //if (_runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().IsFavoriteSong((Guid)song.SongId))
                        //{
                        //    _favToggle.isOn = true;
                        //}
                        //else
                        //{
                        //    _favToggle.isOn = false;
                        //}
                    }
                    catch { }
                }
            }
            catch { }
        }

        public void UpdateQueueSongList()
        {
            try
            {
                if (_viewportContent == null) { return; }
                if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
                foreach (Transform child in _viewportContent.transform)
                {
                    Destroy(child.gameObject);
                }

                NetworkRunner runner = NetworkRunner.Instances[0];
                List<NetworkString<_32>> queueSongCodeList = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().QueueSongCodeList.ToList().Where(songCode => !string.IsNullOrEmpty(songCode.ToString())).ToList();
                List<SongDetail> queueSongList = new();
                foreach (var item in queueSongCodeList)
                {
                    queueSongList.Add(_runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().GetSongBySongCode(item.ToString()));
                }
                List<NetworkString<_32>> singer1NameList = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().QueueSinger1List.ToList();
                List<NetworkString<_32>> singer2NameList = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().QueueSinger2List.ToList();


                for (int i = 0; i < queueSongList.Count; i++)
                {
                    try
                    {
                        GameObject songHolder = Instantiate(_songHolderPrefab, _viewportContent.transform);
                        songHolder.name = queueSongList[i].SongName;
                        songHolder.GetComponentInChildren<SongBinding>().BindingData(queueSongList[i]);

                        TMP_Dropdown _singer1Dropdown = songHolder.transform.Find("Singer1Dropdown").GetComponent<TMP_Dropdown>();
                        TMP_Dropdown _singer2Dropdown = songHolder.transform.Find("Singer2Dropdown").GetComponent<TMP_Dropdown>();
                        if (IsHostOwnedSong())
                        {
                            _singer1Dropdown.ClearOptions();
                            _singer1Dropdown.AddOptions(GetPlayerNameList());
                        }
                        else
                        {
                            _singer1Dropdown.ClearOptions();
                            _singer1Dropdown.AddOptions(GetPlayerNameListThatOwnedSong());
                        }
                        _singer2Dropdown.ClearOptions();
                        _singer2Dropdown.AddOptions(GetPlayerNameList());

                        _singer1Dropdown.value = _singer1Dropdown.options.FindIndex(option => option.text == singer1NameList[i].ToString());
                        _singer2Dropdown.value = _singer2Dropdown.options.FindIndex(option => option.text == singer2NameList[i].ToString());

                    }
                    catch { }
                }
            }
            catch { }
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
