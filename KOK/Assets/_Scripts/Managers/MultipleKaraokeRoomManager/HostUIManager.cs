using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KOK
{
    public class HostUIManager : NetworkBehaviour
    {
        bool isUIEnable = true;
        NetworkRunner runner;
        [SerializeField] List<GameObject> HostUIList = new();
        [SerializeField] int role;

        void OnEnable()
        {
            if (HasStateAuthority)
            {
                HostUIList.Clear();
                List<HostUIMask> tmp = FindObjectsOfType<HostUIMask>().ToList();
                foreach (var t in tmp)
                {
                    HostUIList.Add(t.gameObject);
                }
                StartCoroutine(CheckHost());
            }
        }

        IEnumerator CheckHost()
        {
            if (HasStateAuthority)
            {
                //Debug.LogError(role + " | " + isUIEnable);
                yield return new WaitForSeconds(5f);
                runner = FindAnyObjectByType<NetworkRunner>();
                role = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerRole;
                if (role == 0)
                {
                    if (!isUIEnable)
                    {
                        foreach (var ui in HostUIList)
                        {
                            ui.SetActive(true);
                        }
                        isUIEnable = true;
                    }
                }
                else
                {
                    if (isUIEnable)
                    {
                        foreach (var ui in HostUIList)
                        {
                            ui.SetActive(false);
                        }
                        isUIEnable = false;
                    }
                }
                StartCoroutine(CheckHost());
            }
        }
    }
}
