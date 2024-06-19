using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PostLoader : MonoBehaviour
{
    public GameObject postPrefab;
    public Transform postContainer;
    public GameObject commentPrefab;
    public Transform commentContainer;
    public Text captionText;
    public ScrollRect commentScrollView;
    public GameObject loadMoreButton;
    public int commentsPerLoad = 10;

    private List<Post> posts = new List<Post>();
    private int currentIndex = 0;
    private GameObject currentPost;
    private List<PostComment> currentComments = new List<PostComment>();
    private int commentsLoaded = 0;
    private string baseUrl = "https://localhost:7017/api/items";

    void Start()
    {
        // Simulate loading posts (replace with actual API call)
        LoadPosts();
    }

    void LoadPosts()
    {
        // Simulate fetching posts from API or database
        StartCoroutine(GetPostsFromApi(baseUrl));
    }

    IEnumerator GetPostsFromApi(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            List<Post> loadedPosts = JsonConvert.DeserializeObject<List<Post>>(response);

            posts.AddRange(loadedPosts);

            // Display initial post
            DisplayPost(currentIndex);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    void DisplayPost(int index)
    {
        if (currentPost != null)
        {
            Destroy(currentPost);
        }

        if (index < 0 || index >= posts.Count)
        {
            Debug.LogWarning("Invalid post index.");
            return;
        }

        Post postData = posts[index];
        currentPost = Instantiate(postPrefab, postContainer);
        captionText.text = postData.Caption;

        // Load video URL and display video (implement this part)

        LoadComments(postData);
    }

    void LoadComments(Post post)
    {
        currentComments = new List<PostComment>(post.PostComments);
        commentsLoaded = 0;
        UpdateCommentScrollView();
    }

    void UpdateCommentScrollView()
    {
        // Clear existing comments in the ScrollView
        foreach (Transform child in commentContainer)
        {
            Destroy(child.gameObject);
        }

        // Calculate number of comments to display
        int toLoad = Mathf.Min(commentsPerLoad, currentComments.Count - commentsLoaded);

        // Instantiate comment prefabs in the ScrollView
        for (int i = 0; i < toLoad; i++)
        {
            GameObject commentObj = Instantiate(commentPrefab, commentContainer);
            Text commentText = commentObj.GetComponentInChildren<Text>();
            commentText.text = $"{currentComments[commentsLoaded + i].Member.UserName}: {currentComments[commentsLoaded + i].Comment}";
        }

        // Update commentsLoaded counter
        commentsLoaded += toLoad;

        // Activate or deactivate load more button based on remaining comments
        loadMoreButton.SetActive(commentsLoaded < currentComments.Count);
    }

    public void LoadMoreComments()
    {
        UpdateCommentScrollView();
    }

    public void ShowNextPost()
    {
        currentIndex++;
        if (currentIndex >= posts.Count)
        {
            currentIndex = 0; // Loop back to the first post
        }
        DisplayPost(currentIndex);
    }

    public void ShowPreviousPost()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = posts.Count - 1; // Loop back to the last post
        }
        DisplayPost(currentIndex);
    }
}















/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using Newtonsoft.Json;
using static KOK.ShopLayout;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using System;

public class PostLoader : MonoBehaviour
{
    public GameObject postPrefab;
    public Transform postContainer;
    public GameObject commentItemPrefab; // Add this
    public GameObject displayButton; // Add this
    public Transform displayPanel; // Add this
    public List<Post> posts; // Assume Post is a class containing video URL, comments, etc.
    private int currentIndex = 0;
    private GameObject currentPost;
    private List<PostComment> currentComments;
    private int commentsLoaded = 0; // Add this
    private const int commentsPerLoad = 10; // Add this

    void Start()
    {
        LoadPost(currentIndex);
    }

    void LoadPost(int index)
    {
        if (currentPost != null)
        {
            Destroy(currentPost);
        }

        if (index < 0 || index >= posts.Count) return;

        Post postData = posts[index];
        currentPost = Instantiate(postPrefab, postContainer);
        VideoPlayer videoPlayer = currentPost.transform.Find("VideoPlaceholder").GetComponent<VideoPlayer>();
        Text captionText = currentPost.transform.Find("Caption").GetComponent<Text>();
        Transform commentsContent = currentPost.transform.Find("CommentsScrollView/Viewport/Content");
        Button loadMoreButton = currentPost.transform.Find("LoadMoreButton").GetComponent<Button>();

        // Ensure this is correct for your data model
        //videoPlayer.url = postData.Recording.VideoUrl; // Adjust according to your actual property name
        captionText.text = postData.Caption;

        currentComments = new List<PostComment>(postData.PostComments); // Convert ICollection to List
        commentsLoaded = 0;
        ClearComments(commentsContent);
        LoadMoreComments(commentsContent);

        loadMoreButton.onClick.AddListener(() => LoadMoreComments(commentsContent));

        //videoPlayer.Play();
    }

    void ClearComments(Transform commentsContent)
    {
        foreach (Transform child in commentsContent)
        {
            Destroy(child.gameObject);
        }
    }

    void LoadMoreComments(Transform commentsContent)
    {
        int toLoad = Mathf.Min(commentsPerLoad, currentComments.Count - commentsLoaded);
        for (int i = 0; i < toLoad; i++)
        {
            PostComment postComment = currentComments[commentsLoaded];
            StartCoroutine(LoadCommentItem(postComment, commentsContent));
            commentsLoaded++;
        }
    }

    IEnumerator LoadCommentItem(PostComment postComment, Transform commentsContent)
    {
        // Fetch the account details for the comment
        UnityWebRequest request = UnityWebRequest.Get($"your_api_endpoint/accounts/{postComment.MemberId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Account account = JsonConvert.DeserializeObject<Account>(response);

            // Create the comment item
            GameObject commentItem = Instantiate(commentItemPrefab, commentsContent);
            Text commentText = commentItem.transform.Find("CommentText").GetComponent<Text>();
            Text usernameText = commentItem.transform.Find("UsernameText").GetComponent<Text>();

            commentText.text = postComment.Comment;
            usernameText.text = account.UserName;
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    public void ShowNextPost()
    {
        currentIndex++;
        if (currentIndex >= posts.Count)
        {
            currentIndex = 0; // Loop back to the first post
        }
        LoadPost(currentIndex);
    }

    public void ShowPreviousPost()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = posts.Count - 1; // Loop back to the last post
        }
        LoadPost(currentIndex);
    }

    IEnumerator GetPostComments(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;

            var postData = JsonConvert.DeserializeObject<PostData>(response);
            var postComments = postData.Comments;

            if (currentComments == null)
            {
                currentComments = new List<PostComment>();
            }

            // clear before adding
            currentComments.Clear();

            foreach (CommentData postComment in postComments)
            {
                currentComments.Add(new PostComment
                {
                    CommentId = postComment.CommentId,
                    Comment = postComment.Comment,
                    MemberId = postComment.MemberId,
                    PostId = postComment.PostId
                });
            }

            LayoutGenerate();

            Debug.Log(response);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    void LayoutGenerate()
    {
        for (int i = 0; i < currentComments.Count; i++)
        {
            GameObject gameObj = Instantiate(displayButton, displayPanel);
            gameObj.transform.GetChild(0).GetComponent<TMP_Text>().text = currentComments[i].Comment;

            int index = i;
            gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                ItemClicked(index);
            });
        }
    }

    void ItemClicked(int index)
    {
        // Implement what happens when an item is clicked
        Debug.Log("Comment clicked: " + currentComments[index].Comment);
    }
}

[Serializable]
public class PostData
{
    public Guid PostId { get; set; }
    public string Caption { get; set; }
    public string VideoURL { get; set; }
    public List<CommentData> Comments { get; set; }
}

[Serializable]
public class CommentData
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public Guid MemberId { get; set; }
    public Guid PostId { get; set; }
}
*/