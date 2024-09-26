using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class SingleFavouriteManager : MonoBehaviour
    {
        [SerializeField] Toggle favToggle;
        [SerializeField] GameObject songCode;
        [SerializeField] SinglePlayerManager singlePlayerManager;

        bool isFirstLoad = true;

        private void OnEnable()
        {
            isFirstLoad = true;
            StartCoroutine(FirstLoadCooldown());
        }
        public void OnToggleValueChange()
        {
            if (isFirstLoad)
            {
                return;
            }

            if (singlePlayerManager == null) {
                singlePlayerManager = FindAnyObjectByType<SinglePlayerManager>();
            }

            singlePlayerManager.ClearSearchSongList();

            Guid songId = Guid.Parse(songCode.name.ToString());
            if (favToggle.isOn)
            {
                FindAnyObjectByType<ApiHelper>().gameObject
                        .GetComponent<FavoriteSongController>()
                        .AddFavoriteSongCoroutine(new AddFavoriteSongRequest() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)), SongId = songId },
                                                        (fsr) =>
                                                        {
                                                            Debug.Log(fsr);
                                                            singlePlayerManager.ReloadSong();
                                                        },
                                                        (ex) => Debug.LogError(ex));
            }
            else
            {
                FindAnyObjectByType<ApiHelper>().gameObject
                        .GetComponent<FavoriteSongController>()
                        .RemoveFavoriteSongCoroutine(new RemoveFavoriteSongRequest() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)), SongId = songId },
                                                        (fsr) =>
                                                        {
                                                            Debug.Log(fsr);
                                                            singlePlayerManager.ReloadSong();
                                                        },
                                                        (ex) => Debug.LogError(ex));
            }
        }

        IEnumerator FirstLoadCooldown()
        {
            yield return new WaitForSeconds(1f);
            isFirstLoad = false;
            singlePlayerManager = FindAnyObjectByType<SinglePlayerManager>();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
