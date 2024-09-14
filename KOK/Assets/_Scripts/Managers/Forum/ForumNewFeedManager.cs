using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ForumNewFeedManager : MonoBehaviour
    {
        [SerializeField] TMP_Text title;
        [SerializeField] PostBinding postBinding;
        [SerializeField] Image ownAvatar;
        [SerializeField] Sprite forumIcon;
        private int currentPage = 1;
        private int currentPostIndex = 0;
        List<Post> postList = new();
        Account ownAccount;
        bool isOwnedProfile = false;
        public AlertManager MessageAlertManager;
        public AlertManager ConfirmAlertManager;

        private void Start()
        {
            postBinding.Clear();
            RefreshForum();
            LoadOwnAccount();
        }

        private void LoadOwnAccount()
        {
            ApiHelper.Instance.GetComponent<AccountController>()
                .GetAccountCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (account) =>
                    {
                        ownAccount = account.Value;
                        ownAvatar.sprite = Resources.Load<Sprite>(ownAccount.CharaterItemCode + "AVA");
                    },
                    (ex) => { }
                );
        }

        public void ShowPost()
        {
            postBinding.Init(postList[currentPostIndex], isOwnedProfile, this);
        }

        private void LoadNext()
        {
            if (isOwnedProfile)
            {
                return;
            }
            //Debug.LogError(isOwnedProfile);
            ApiHelper.Instance.GetComponent<PostController>()
            .GetPostsFilterPagingCoroutine(
                new PostFilter(),
                new PostOrderFilter(),
                new PagingRequest()
                {
                    page = currentPage + 1,
                    pageSize = 10,
                },
                (post) =>
                {
                    postList.Concat(post.Results);
                    currentPage++;
                },
                (msg) => { }
            );
        }

        public void PlayPrevPost()
        {
            if (currentPostIndex > 0)
            {
                currentPostIndex--;
                ShowPost();
            }
        }

        public void PlayNextPost()
        {
            if (currentPostIndex >= postList.Count - 2)
            {
                LoadNext();
            }
            if (currentPostIndex >= postList.Count - 1)
            {
                return;
            }
            currentPostIndex++;
            ShowPost();
        }

        public void ToggleBetweenForumAndProfile()
        {
            if (isOwnedProfile)
            {
                RefreshForum();
            }
            else
            {
                RefreshOwnPost();
            }
        }


        public void Refresh()
        {
            postList.Clear();
            postBinding.Clear();
            if (isOwnedProfile)
            {
                RefreshOwnPost();
            }
            else
            {
                RefreshForum();
            }
        }

        public void RefreshForum()
        {
            if (ownAccount != null)
            {
                ownAvatar.sprite = Resources.Load<Sprite>(ownAccount.CharaterItemCode + "AVA");
            }
            isOwnedProfile = false;
            title.text = "Forum";
            ApiHelper.Instance.GetComponent<PostController>()
            .GetPostsFilterPagingCoroutine(
                new PostFilter(),
                PostOrderFilter.UploadTime,
                new PagingRequest()
                {
                    page = 1,
                    pageSize = 10,
                },
                (posts) =>
                {                    
                    postList = posts.Results;
                    currentPage = 1;
                    currentPostIndex = 0;
                    isOwnedProfile = false;
                    ShowPost();
                },
                (msg) =>
                {
                    postList.Clear();
                    postBinding.Clear();
                    //Refresh();
                }
            );
        }

        public void RefreshOwnPost()
        {
            ownAvatar.sprite = forumIcon;
            title.text = "Profile";
            isOwnedProfile = true;
            ApiHelper.Instance.GetComponent<PostController>()
            .GetPostsFilterPagingCoroutine(
                new PostFilter()
                {
                    MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))
                },
                PostOrderFilter.UploadTime,
                new PagingRequest()
                {
                    page = 1,
                    pageSize = 1000,
                },
                (posts) =>
                {
                    postList = posts.Results;
                    currentPage = 1;
                    currentPostIndex = 0;
                    isOwnedProfile = true;
                    ShowPost();
                },
                (msg) =>
                {
                    postList.Clear();
                    postBinding.Clear();
                    //Refresh();
                }
            );
        }

        

    }
}
