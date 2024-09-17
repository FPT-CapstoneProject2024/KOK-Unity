using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool isSwipeInProgress = false;
    public float swipeThreshold = 50f;

    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;

    public RectTransform swipePanel;

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (IsTouchInSwipePanel(touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUp = touch.position;
                    fingerDown = touch.position;
                }

                if (!isSwipeInProgress && touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }
        }

        // Mouse swipe detection for testing in the editor
        if (IsTouchInSwipePanel(Input.mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                fingerUp = Input.mousePosition;
                fingerDown = Input.mousePosition;
            }

            if (!isSwipeInProgress && Input.GetMouseButtonUp(0))
            {
                fingerDown = Input.mousePosition;
                CheckSwipe();
            }
        }
    }

    void CheckSwipe()
    {
        float verticalMove = Mathf.Abs(fingerDown.y - fingerUp.y);

        if (verticalMove > swipeThreshold)
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                OnSwipeUp?.Invoke();
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                OnSwipeDown?.Invoke();
            }
            fingerUp = fingerDown;
            isSwipeInProgress = true; // Set flag to prevent multiple swipes until transition completes
        }
    }

    bool IsTouchInSwipePanel(Vector2 touchPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(swipePanel, touchPosition, null, out localPoint);
        return swipePanel.rect.Contains(localPoint);
    }

    public void ResetSwipeState()
    {
        isSwipeInProgress = false; // Reset flag when transition completes
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
