using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostTransition : MonoBehaviour
{
    public GameObject postPrefab;
    public Transform postParent;
    public float transitionDuration = 0.5f;

    private List<GameObject> posts = new List<GameObject>();
    private int currentPostIndex = 0;

    private SwipeDetector swipeDetector;

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

    void Start()
    {
        LoadInitialPosts();
    }

    void LoadInitialPosts()
    {
        for (int i = 0; i < 3; i++) // Load 3 initial posts for example
        {
            GameObject post = Instantiate(postPrefab, postParent);
            post.GetComponentInChildren<TMP_Text>().text = "Post " + (i + 1);
            posts.Add(post);
            post.SetActive(i == 0); // Only show the first post initially

            // Center the post in the parent panel
            RectTransform postRectTransform = post.GetComponent<RectTransform>();
            postRectTransform.anchoredPosition = Vector2.zero;
        }

        // Adjust currentPostIndex based on the initial setup
        currentPostIndex = 0;
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
    }
}
