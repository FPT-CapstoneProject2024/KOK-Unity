using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System.Threading.Tasks;
using KOK.ApiHandler.DTOModels;

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

        private Camera mainCamera;
        public VideoPlayer videoPlayer;

        void OnEnable()
        {
            swipeDetector = FindObjectOfType<SwipeDetector>();
            swipeDetector.OnSwipeUp.AddListener(SwipeUp);
            swipeDetector.OnSwipeDown.AddListener(SwipeDown);

            mainCamera = Camera.main; // Reference to the main camera   
        }

        void OnDisable()
        {
            swipeDetector.OnSwipeUp.RemoveListener(SwipeUp);
            swipeDetector.OnSwipeDown.RemoveListener(SwipeDown);
        }

        void PlayVideo()
        {
            if (currentPostIndex == 0)
            {
                videoPlayer.Stop();
                videoPlayer.url = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FBu%E1%BB%93n%20Th%C3%AC%20C%E1%BB%A9%20Kh%C3%B3c%20%C4%90i.mp4?alt=media&token=f46adca8-0cd5-4860-896c-dd8efb3bf05a";
                videoPlayer.Prepare();
                videoPlayer.Play();
            }
            if (currentPostIndex == 1)
            {
                videoPlayer.Stop();
                videoPlayer.url = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FM%E1%BB%99t%20%C4%90%C3%AAm%20Say.mp4?alt=media&token=ff66ba2c-ef79-4900-9b0d-c672c3ce7c6e";
                videoPlayer.Prepare();
                videoPlayer.Play();
            }
            if (currentPostIndex == 2)
            {
                videoPlayer.Stop();
                videoPlayer.url = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FT%C3%BAy%20%C3%82m.mp4?alt=media&token=77cd5236-39e2-4120-95dd-ec64ed64cc3e";
                videoPlayer.Prepare();
                videoPlayer.Play();
            }
            if (currentPostIndex == 3)
            {
                videoPlayer.Stop();
                videoPlayer.url = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FBu%E1%BB%93n%20Th%C3%AC%20C%E1%BB%A9%20Kh%C3%B3c%20%C4%90i.mp4?alt=media&token=f46adca8-0cd5-4860-896c-dd8efb3bf05a";
                videoPlayer.Prepare();
                videoPlayer.Play();
            }
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
            // Update caption and member display initially
            UpdateCaptionDisplay();
            UpdateMemberDisplay();
        }

        void SwipeUp()
        {
            if (currentPostIndex < posts.Count - 1)
            {
                StartCoroutine(TransitionPost(currentPostIndex + 1, true)); // true for swipe up
                PlayVideo();
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
                PlayVideo();
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
            float screenHeight = mainCamera.orthographicSize * 2;

            Vector3 startPostPosition = posts[currentPostIndex].transform.localPosition;
            Vector3 endPostPosition = isSwipeUp ? new Vector3(0, startPostPosition.y + screenHeight, startPostPosition.z)
                                                 : new Vector3(0, startPostPosition.y - screenHeight, startPostPosition.z);
            Vector3 newPostStartPosition = isSwipeUp ? new Vector3(0, startPostPosition.y - screenHeight, startPostPosition.z)
                                                      : new Vector3(0, startPostPosition.y + screenHeight, startPostPosition.z);

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

            // Load comments and member details for the new current post
            LoadCommentsForCurrentPost();
            LoadMemberForCurrentPost();

            // Update caption and member display after transition
            UpdateCaptionDisplay();
            UpdateMemberDisplay();
        }

        void ResetPostPositions(bool toCurrentPosition)
        {
            StartCoroutine(ResetPostPositionsCoroutine(toCurrentPosition));
        }

        IEnumerator ResetPostPositionsCoroutine(bool toCurrentPosition)
        {
            float elapsedTime = 0;
            float screenHeight = mainCamera.orthographicSize * 2;

            Vector3 startPostPosition = posts[currentPostIndex].transform.localPosition;
            Vector3 endPostPosition = new Vector3(0, startPostPosition.y - screenHeight, startPostPosition.z);

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

            // Load comments and member details for the current post after resetting positions
            LoadCommentsForCurrentPost();
            LoadMemberForCurrentPost();

            // Update caption and member display after resetting positions
            UpdateCaptionDisplay();
            UpdateMemberDisplay();
        }

        private async Task LoadCommentsForCurrentPost()
        {
            if (postCommentLoader != null)
            {
                string currentPostId = GetCurrentPostId();
                if (!string.IsNullOrEmpty(currentPostId))
                {
                    await postCommentLoader.LoadPostCommentsAsync(currentPostId);
                }
            }
        }

        private async Task LoadMemberForCurrentPost()
        {
            if (memberLoader != null)
            {
                string currentPostId = GetCurrentPostId();
                if (!string.IsNullOrEmpty(currentPostId))
                {
                    await memberLoader.GetPostMemberAsync(currentPostId);
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
            /*if (memberText != null && currentPostIndex >= 0 && currentPostIndex < posts.Count)
            {
                memberText.text = memberLoader.GetMemberName(posts[currentPostIndex].name);
            }*/
        }
    }
}
