using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KOK
{
    public class ForumNewFeedManager : MonoBehaviour
    {
        [SerializeField] PostBinding postBinding;
        private int currentPage = 1;
        private int currentPostIndex = 0;
        List<Post> postList = new();

        private void Start()
        {
            Refresh();
        }

        public void ShowPost()
        {
            postBinding.Init(postList[currentPostIndex]);
        }

        public void Refresh()
        {
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
                    ShowPost();
                },
                (msg) => { }
            );
        }

        private void LoadNext()
        {
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
            if (currentPostIndex != 0)
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

            currentPostIndex++;
            ShowPost();
        }

        public void CreatePost()
        {

        }

    }
}
