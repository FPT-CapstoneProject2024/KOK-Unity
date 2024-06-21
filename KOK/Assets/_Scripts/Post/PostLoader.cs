using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;

namespace KOK
{
    public class PostLoader : MonoBehaviour
    {
        private string postBaseUrl = "https://localhost:7017/api/posts";
        public GameObject displayPanel;
        public GameObject captionDisplayPanel;
        public GameObject displayPrefab;
        public PostTransition postTransition;
        private int currentPostIndex = 0;
        private List<Post> loadedPosts = new List<Post>(); // Store loaded posts

        void Start()
        {
            StartCoroutine(GetPosts(postBaseUrl));
        }

        IEnumerator GetPosts(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<PostResponseObject>(response);
                var posts = responseObject.Results;

                if (postTransition != null)
                {
                    GeneratePosts(posts); // Pass loaded posts to the method
                }

                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        /*IEnumerator GetMemberInfo(Post post)
        {
            string url = $"https://localhost:7017/api/accounts/{post.MemberId}";

            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var member = JsonConvert.DeserializeObject<Account>(response);
                post.Member = member;
            }
            else
            {
                Debug.LogError($"Failed to fetch member info for post {post.PostId}: {request.error}");
            }
        }*/

        void GeneratePosts(List<Post> posts)
        {
            if (postTransition != null)
            {
                postTransition.SetInitialPosts(posts); // Call method in PostTransition to set posts
            }
        }

        // Method to get current post ID based on currentPostIndex
        public string GetCurrentPostId()
        {
            if (currentPostIndex >= 0 && currentPostIndex < loadedPosts.Count)
            {
                return loadedPosts[currentPostIndex].PostId.ToString();
            }
            return string.Empty; // Return empty string if index is out of bounds
        }
    }

    public class PostResponseObject
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Metadata Metadata { get; set; }
        public List<Post> Results { get; set; }
    }

    public class Metadata
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}




/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;

namespace KOK
{
    public class PostLoader : MonoBehaviour
    {
        private string postBaseUrl = "https://localhost:7017/api/posts";
        public GameObject displayPanel;
        public GameObject displayPrefab;
        private PostTransition postTransition;

        void Start()
        {
            postTransition = GetComponent<PostTransition>();
            StartCoroutine(GetPosts(postBaseUrl));
        }

        IEnumerator GetPosts(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<PostResponseObject>(response);
                var posts = responseObject.Results;

                GeneratePosts(posts);
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        void GeneratePosts(List<Post> posts)
        {
            List<GameObject> instantiatedPosts = new List<GameObject>();

            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Post post in posts)
            {
                GameObject gameObj = Instantiate(displayPrefab, displayPanel.transform);
                gameObj.transform.GetChild(1).GetComponent<TMP_Text>().text = post.MemberId.ToString();
                // gameObj.transform.GetChild(2).GetComponent<TMP_Text>().text = post.Comment;

                gameObj.SetActive(false); // Initially deactivate all posts

                // Center the post in the parent panel
                RectTransform postRectTransform = gameObj.GetComponent<RectTransform>();
                postRectTransform.anchoredPosition = Vector2.zero;

                instantiatedPosts.Add(gameObj); // Add the instantiated post to the list
            }

            //postTransition.SetPosts(instantiatedPosts);
        }
    }

    public class PostResponseObject
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Metadata Metadata { get; set; }
        public List<Post> Results { get; set; }
    }

    public class Metadata
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}
*/