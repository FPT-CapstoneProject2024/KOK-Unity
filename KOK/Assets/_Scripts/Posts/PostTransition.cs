using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System.Threading.Tasks;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using System;
using KOK.Assets._Scripts.Posts;
using KOK.Assets._Scripts.FileManager;

namespace KOK
{
    public class PostTransition : MonoBehaviour
    {
        public GameObject postPrefab;
        public Transform postParent;

        private List<GameObject> postGameObjList = new List<GameObject>();
        private int currentPostIndex = 0;

        public PostCommentLoader postCommentLoader;
        public MemberLoader memberLoader;
        public PostLoader postLoader;

        public TMP_Text captionText;
        public TMP_Text memberText;

        private int currentPage = 0;
        private int currentTotalPage = 0;
        private int totalPage = 0;
        private int triggerLoadPostIndex = 0;

        private List<Post> postDataList = new List<Post>();

        private void Start()
        {
            LoadMorePosts();
            //GetPostsFilterPaging();
        }

        public void GetPostsFilterPaging(int currentPage)
        {
            var pagingRequest = new PagingRequest
            {
                page = currentPage,     
                pageSize = 5,           
                OrderType = SortOrder.Ascending 
            };

            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PostController>()
                .GetPostsFilterPagingCoroutine( new PostFilter(),
                                                new PostOrderFilter(),
                                                pagingRequest,
                                                GetDynamicRR,
                                                OnError
                );
        }

        private void LoadMorePosts()
        {
            currentPage += 1;
            GetPostsFilterPaging(currentPage);
        }

        private void LoadPageTrigger()
        {
            if(currentTotalPage != totalPage)
            {
                triggerLoadPostIndex = currentTotalPage - 2;
            }
        }

        // This function exists so as SetInitialPosts's List<Post> does not have to change into DynamicResponseResult<Post>
        private void GetDynamicRR(DynamicResponseResult<Post> dynamicResponseResult)
        {
            totalPage = dynamicResponseResult.Metadata.Total;
            currentTotalPage += dynamicResponseResult.Results.Count;
            // if currentTotalPage % 5 = 0  =>  currentTotalPage - 1 = trigger page    else trigger page = 0

            //Debug.Log(totalPage);

            var postsData = dynamicResponseResult.Results;
            LoadPageTrigger();
            SetInitialPosts(postsData);
        }

        public void SetInitialPosts(List<Post> postsData)
        {
            //postDataList = new List<Post>();
            foreach (var postData in postsData)
            {
                GameObject post = Instantiate(postPrefab, postParent);
                post.GetComponentInChildren<TMP_Text>().text = postData.Caption;
                // Store PostId as a custom data associated with the GameObject
                post.name = postData.PostId.ToString(); // Use gameObject.name to store PostId

                // Center the post in the parent panel
                RectTransform postRectTransform = post.GetComponent<RectTransform>();
                postRectTransform.anchoredPosition = Vector2.zero;

                postDataList.Add(postData);
                postGameObjList.Add(post);
                post.SetActive(postGameObjList.Count == 1); // Activate only the first post initially
            }

            // Adjust currentPostIndex based on the initial setup
            //currentPostIndex = 0;

            LoadPost();
        }

        public void SwipeUp()
        {
            if (currentPostIndex < postGameObjList.Count - 1)
            {
                var currentIndex = currentPostIndex + 1;
                TransitionPost(currentPostIndex + 1);

                // trigger load more posts
                if (currentIndex == triggerLoadPostIndex)
                {
                    Debug.Log("trigger load posts");
                    LoadMorePosts();
                }
            }
            else
            {

            }
        }

        public void SwipeDown()
        {
            if (currentPostIndex > 0)
            {
                TransitionPost(currentPostIndex - 1);
            }
            else
            {

            }
        }

        void TransitionPost(int newPostIndex)
        {
            postGameObjList[newPostIndex].SetActive(true);

            postGameObjList[currentPostIndex].SetActive(false);
            currentPostIndex = newPostIndex;

            LoadPost();
        }

        void LoadPost()
        {
            Post currentPost = postDataList[currentPostIndex];

            // Stop current video
            StopVideo();

            // Load comments and member details for the new current post
            LoadCommentsForCurrentPost(currentPost);
            LoadVideoForCurrentPost(currentPost);

            // Update caption and member display after transition
            UpdateCaptionDisplay(currentPost);
            UpdateMemberDisplay(currentPost);
        }

        private void LoadCommentsForCurrentPost(Post post)
        {
            if (postCommentLoader != null)
            {
                postCommentLoader.GetPostComment(post.PostId.Value);
            }
        }

        private void LoadVideoForCurrentPost(Post post)
        {
            if (postLoader != null)
            {
                postLoader.GetRecordingById(post.RecordingId.Value);
            }
        }

        void StopVideo()
        {
            var videoLoader = GetComponentInChildren<VideoLoader>();
            videoLoader.StopPlaying();
        }

        void UpdateCaptionDisplay(Post post)
        {
            captionText.text = post.Caption;
        }

        void UpdateMemberDisplay(Post post)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<AccountController>()
                .GetAccountByIdCoroutine(post.MemberId.Value,
                      (member) =>
                      {
                          memberText.text = member.UserName;
                      },
                      OnError
                      );
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
