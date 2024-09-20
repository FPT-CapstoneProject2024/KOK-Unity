using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Context;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using Unity.VisualScripting;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;

namespace KOK
{
    public class MemberLoader : MonoBehaviour
    {
        //[SerializeField] private List<Post> postList = new List<Post>();
        //[SerializeField] private List<PostRate> postRateList = new List<PostRate>();
        private List<Account> memberList = new List<Account>();
        private Account member;
        private string accountsResourceUrl = string.Empty;
        //private string accountBaseUrl = "https://localhost:7017/api/accounts";
        public GameObject displayPanel;
        public GameObject displayPrefab;
        private int currentPostIndex = 0;

        void Start()
        {
            //StartCoroutine(GetPostComments(postRateBaseUrl, "83096D27-3238-4D2D-A6B0-3F2D009DFA14"));
            accountsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Accounts_Resource;
        }

        public void GetPostMemberCoroutine(Guid postId)
        {
            //FindAnyObjectByType<ApiHelper>().gameObject
            //    .GetComponent<PostController>()
            //    .GetPostByIdCoroutine(  postId,
            //                            GetMemberCoroutine,
            //                            OnError
            //    );
        }

        public void GetMemberCoroutine(List<Post> posts)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<AccountController>()
                .GetAccountByIdCoroutine(   posts[0].MemberId.Value,
                                            MemberGenerate,
                                            OnError
                );
        }

        void MemberGenerate(Account member)
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject gameObj = Instantiate(displayPrefab, displayPanel.transform);
            gameObj.transform.GetComponent<TMP_Text>().text = member.UserName;

        }

        private void OnError(string error)
        {
            Debug.LogError(error);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
