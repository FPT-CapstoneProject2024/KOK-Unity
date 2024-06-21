using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Collections.Generic;

namespace KOK
{
    public class PostLoader : MonoBehaviour
    {
        [SerializeField] private List<Post> postList = new List<Post>();
        private string postBaseUrl = "https://localhost:7017/api/posts";
        public GameObject displayPanel;
        public GameObject postPrefab;
        private int currentPostIndex = 0;

        private SwipeDetector swipeDetector;

        void Start()
        {
            swipeDetector = GetComponent<SwipeDetector>();
            swipeDetector.OnSwipeUp.AddListener(OnSwipeUp);
            swipeDetector.OnSwipeDown.AddListener(OnSwipeDown);

            StartCoroutine(GetPosts(postBaseUrl));
        }

        IEnumerator GetPosts(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var posts = JsonConvert.DeserializeObject<List<Post>>(response);
                postList.AddRange(posts);

                // Load the initial set of posts
                LoadPosts();
            }
            else
            {
                Debug.LogError("Failed to fetch posts: " + request.error);
            }
        }

        void LoadPosts()
        {
            // Clear the display panel
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            // Instantiate and display posts
            for (int i = 0; i < postList.Count; i++)
            {
                GameObject postObj = Instantiate(postPrefab, displayPanel.transform);
                postObj.GetComponentInChildren<TMP_Text>().text = postList[i].Title; // Example: Display post title
                postObj.SetActive(i == currentPostIndex); // Show only the current post
            }
        }

        void OnSwipeUp()
        {
            if (currentPostIndex < postList.Count - 1)
            {
                currentPostIndex++;
                LoadPosts();
            }
        }

        void OnSwipeDown()
        {
            if (currentPostIndex > 0)
            {
                currentPostIndex--;
                LoadPosts();
            }
        }

        [System.Serializable]
        public class Post
        {
            public string Title { get; set; }
            // Add more properties as needed based on your API response
        }
    }
}
