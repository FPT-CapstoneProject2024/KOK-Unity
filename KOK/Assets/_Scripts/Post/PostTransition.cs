using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using KOK.Assets._Scripts.Post;

namespace KOK
{
    public class PostTransition : MonoBehaviour
    {
        public GameObject postPrefab;
        public Transform postParent;
        public float transitionDuration = 0.5f;

        private List<GameObject> posts = new List<GameObject>();
        private int currentPostIndex = 0;

        private SwipeDetector swipeDetector;
        public PostCommentLoader postCommentLoader;
        public MemberLoader memberLoader;

        public TMP_Text captionText;
        public TMP_Text memberText;

        void OnEnable()
        {
            swipeDetector = FindObjectOfType<SwipeDetector>();
            swipeDetector.OnSwipeUp.AddListener(SwipeUp);
            swipeDetector.OnSwipeDown.AddListener(SwipeDown);
        }

        void OnDisable()
        {
            swipeDetector.OnSwipeUp.RemoveListener(SwipeUp);
            swipeDetector.OnSwipeDown.RemoveListener(SwipeDown);
        }

        public void SetInitialPosts(List<Post> postsData)
        {
            foreach (var postData in postsData)
            {
                GameObject post = Instantiate(postPrefab, postParent);
                post.GetComponentInChildren<TMP_Text>().text = postData.Caption;

                // Store PostId as a custom data associated with the GameObject
                post.name = postData.PostId.ToString(); // Use gameObject.name to store PostId

                // Center the post in the parent panel
                RectTransform postRectTransform = post.GetComponent<RectTransform>();
                postRectTransform.anchoredPosition = Vector2.zero;

                posts.Add(post);
                post.SetActive(posts.Count == 1); // Activate only the first post initially
            }

            // Adjust currentPostIndex based on the initial setup
            currentPostIndex = 0;

            // Load comments for the initial post
            LoadCommentsForCurrentPost();

            LoadMemberForCurrentPost();
            // Update caption display initially
            UpdateCaptionDisplay();
        }

        void SwipeUp()
        {
            if (currentPostIndex < posts.Count - 1)
            {
                StartCoroutine(TransitionPost(currentPostIndex + 1, true)); // true for swipe up
            }
            else
            {
                ResetPostPositions(false); // Reset to current post position without transitioning
            }
        }

        void SwipeDown()
        {
            if (currentPostIndex > 0)
            {
                StartCoroutine(TransitionPost(currentPostIndex - 1, false)); // false for swipe down
            }
            else
            {
                ResetPostPositions(true); // Reset to current post position without transitioning
            }
        }

        IEnumerator TransitionPost(int newPostIndex, bool isSwipeUp)
        {
            float elapsedTime = 0;
            Vector3 startPostPosition = posts[currentPostIndex].transform.localPosition;
            Vector3 endPostPosition = isSwipeUp ? new Vector3(0, startPostPosition.y + Screen.height, startPostPosition.z)
                                                 : new Vector3(0, startPostPosition.y - Screen.height, startPostPosition.z);
            Vector3 newPostStartPosition = isSwipeUp ? new Vector3(0, startPostPosition.y - Screen.height, startPostPosition.z)
                                                      : new Vector3(0, startPostPosition.y + Screen.height, startPostPosition.z);

            posts[newPostIndex].SetActive(true);
            posts[newPostIndex].transform.localPosition = newPostStartPosition;

            while (elapsedTime < transitionDuration)
            {
                posts[currentPostIndex].transform.localPosition = Vector3.Lerp(startPostPosition, endPostPosition, elapsedTime / transitionDuration);
                posts[newPostIndex].transform.localPosition = Vector3.Lerp(newPostStartPosition, startPostPosition, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            posts[currentPostIndex].SetActive(false);
            currentPostIndex = newPostIndex;

            swipeDetector.ResetSwipeState(); // Reset swipe state after transition

            // Load comments for the new current post
            LoadCommentsForCurrentPost();

            LoadMemberForCurrentPost();

            // Update caption display after transition
            UpdateCaptionDisplay();
        }

        void ResetPostPositions(bool toCurrentPosition)
        {
            StartCoroutine(ResetPostPositionsCoroutine(toCurrentPosition));
        }

        IEnumerator ResetPostPositionsCoroutine(bool toCurrentPosition)
        {
            float elapsedTime = 0;
            Vector3 startPostPosition = posts[currentPostIndex].transform.localPosition;
            Vector3 endPostPosition = new Vector3(0, startPostPosition.y - Screen.height, startPostPosition.z);

            while (elapsedTime < transitionDuration)
            {
                posts[currentPostIndex].transform.localPosition = Vector3.Lerp(startPostPosition, endPostPosition, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset to current post position without transitioning further
            if (toCurrentPosition)
            {
                posts[currentPostIndex].transform.localPosition = startPostPosition;
            }

            swipeDetector.ResetSwipeState(); // Reset swipe state after resetting positions

            // Load comments for the new current post after resetting positions
            LoadCommentsForCurrentPost();

            // Update caption display after resetting positions
            UpdateCaptionDisplay();
        }

        void LoadCommentsForCurrentPost()
        {
            if (postCommentLoader != null)
            {
                string currentPostId = GetCurrentPostId();
                if (!string.IsNullOrEmpty(currentPostId))
                {
                    postCommentLoader.StartLoadingPostComments(currentPostId);
                }
            }
        }

        void LoadMemberForCurrentPost()
        {
            if (memberLoader != null)
            {
                string currentPostId = GetCurrentPostId();
                if (!string.IsNullOrEmpty(currentPostId))
                {
                    memberLoader.LoadPostMember(currentPostId);
                }
            }
        }

        string GetCurrentPostId()
        {
            if (currentPostIndex >= 0 && currentPostIndex < posts.Count)
            {
                // Retrieve PostId from the stored custom data (gameObject.name)
                return posts[currentPostIndex].name;
            }
            return string.Empty; // Return empty string if index is out of bounds
        }

        void UpdateCaptionDisplay()
        {
            if (captionText != null && currentPostIndex >= 0 && currentPostIndex < posts.Count)
            {
                // Update the UI Text with the caption of the current post
                captionText.text = posts[currentPostIndex].GetComponentInChildren<TMP_Text>().text;
            }
        }

        void UpdateMemberDisplay()
        {
            if (memberText != null && currentPostIndex >= 0 && currentPostIndex < posts.Count)
            {
                memberText.text = posts[currentPostIndex].GetComponentInChildren<TMP_Text>().text;
            }
        }
    }
}
