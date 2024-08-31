using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace KOK
{
    public class CommentBinding : MonoBehaviour
    {
        [SerializeField] Image avatar;
        [SerializeField] TMP_Text userName;
        [SerializeField] TMP_Text commentContent;
        [SerializeField] TMP_Text createDateTime;
        [SerializeField] RectTransform commentHeaderRect;
        [SerializeField] RectTransform commentContentRect;
        [SerializeField] RectTransform rectTransform;
        private Transform commentViewPortContent;

        [Header("Reply")]
        [SerializeField] Button showReplyButton;
        Action<PostComment> switchToReply;
        [SerializeField] GameObject replyPanel;
        [SerializeField] GameObject childCommentPrefab;
        List<CommentBinding> replyCommentBindingList = new();
        private bool isShowReply = false;
        public float originCommentHeight = 0;
        public float commentHeight = 0;
        public bool ready = false;

        [Header("Create Comment")]
        [SerializeField] TMP_InputField commentInputField;

        [Header("Option")]
        [SerializeField] TMP_Dropdown optionDropdown;
        List<string> ownPostOptions = new List<string>() { "Chỉnh sửa", "Xoá", "" };
        List<string> otherPostOptions = new List<string>() { "Báo cáo", "" };
        ForumNewFeedManager forumNewFeedManager;
        PostBinding postBinding;

        [Header("Edit Comment")]
        [SerializeField] EditCommentBinding editCommentBinding;
        [SerializeField] TMP_InputField editInputField;

        private PostComment postComment;
        public void Init(PostComment postComment, Transform commentViewPortContent, ForumNewFeedManager forumNewFeedManager, PostBinding postBinding, Action<PostComment> switchToReply)
        {
            this.StopAllCoroutines();
            ready = false;
            gameObject.name = postComment.UploadTime.ToString();
            //Set avatar here
            avatar.sprite = Resources.Load<Sprite>(postComment.Member.CharaterItemCode + "AVA");
            userName.text = postComment.Member.UserName.ToString();
            commentContent.text = postComment.Comment;
            createDateTime.text = postComment.UploadTime.ToString();
            this.commentViewPortContent = commentViewPortContent;
            this.postComment = postComment;
            this.switchToReply = switchToReply;
            this.forumNewFeedManager = forumNewFeedManager;
            this.postBinding = postBinding;
            StartCoroutine(SetCommentSize());

        }

        IEnumerator SetCommentSize()
        {
            yield return new WaitForSeconds(0.1f);

            InitOptionDropdownValue();
            //Debug.LogError(postComment.CommentType + " | " + rectTransform);
            commentHeight = commentHeaderRect.sizeDelta.y + commentContentRect.rect.height;
            //Debug.LogError(postComment.CommentType + " | " + commentHeight);

            if (postComment.CommentType == 0)
            {
                if (postComment.InverseParentComment != null)
                {
                    if (postComment.InverseParentComment.Count > 0)
                    {
                        commentHeight -= 10;
                        replyPanel.SetActive(true);
                        isShowReply = true;
                        showReplyButton.gameObject.SetActive(true);
                        replyCommentBindingList.Clear();
                        postComment.InverseParentComment.Reverse();
                        foreach (var comment in postComment.InverseParentComment)
                        {
                                var commentObject = Instantiate(childCommentPrefab, replyPanel.transform);
                                commentObject.GetComponent<CommentBinding>().Init(comment, replyPanel.transform, forumNewFeedManager, postBinding, null);
                                replyCommentBindingList.Add(commentObject.GetComponent<CommentBinding>());
                        }
                        showReplyButton.GetComponent<TMP_Text>().text = $"Câu trả lời ({postComment.InverseParentComment.Count})";
                    }
                    else
                    {
                        showReplyButton.gameObject.SetActive(false);
                        commentHeight -= 20;
                    }
                }

            }
            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, commentHeight);
            originCommentHeight = commentHeight;
            //Debug.LogError(postComment.Comment + " 2 | " + commentHeight + " | " + originCommentHeight);

            yield return new WaitForSeconds(0.1f);
            ready = true;
        }

        public void DeactiveReplyPanel()
        {
            //Debug.Log(gameObject.name + " | " + replyPanel);
            if (replyPanel != null)
            {
                if (replyPanel.activeSelf)
                {
                    isShowReply = true;
                    ToggleCommentPanel();
                }
            }
        }

        public void ToggleCommentPanel()
        {
            StartCoroutine(ToggleCommentPanelCoroutine());
        }
        IEnumerator ToggleCommentPanelCoroutine()
        {
            if (isShowReply)
            {
                commentHeight = originCommentHeight;
                rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, commentHeight);
                replyPanel.SetActive(false);
                isShowReply = false;
            }
            else
            {
                commentHeight = originCommentHeight;
                foreach (var replyBinding in replyCommentBindingList)
                {
                    commentHeight += replyBinding.commentHeight;
                }
                replyPanel.SetActive(true);
                isShowReply = true;
                rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, commentHeight);
                //yield return new WaitForSeconds(0.01f);
                //replyPanel.SetActive(false);
                //replyPanel.SetActive(true);
            }
            yield return new WaitForSeconds(0.01f);
            commentViewPortContent.gameObject.SetActive(false);
            commentViewPortContent.gameObject.SetActive(true);
        }

        public void SwitchToReply()
        {
            switchToReply.Invoke(postComment);
        }

        private void InitOptionDropdownValue()
        {
            optionDropdown.ClearOptions();
            if (postComment.MemberId.Equals(Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))))
            {
                optionDropdown.AddOptions(ownPostOptions);
                optionDropdown.value = ownPostOptions.Count - 1;
            }
            else
            {
                optionDropdown.AddOptions(otherPostOptions);
                optionDropdown.value = otherPostOptions.Count - 1;
            }
        }

        public void OnOptionDropdownValueChange()
        {
            //Debug.Log(optionDropdown.options[optionDropdown.value]);
            if (optionDropdown.options[optionDropdown.value].text.Equals("Xoá"))
            {
                OpenDeletePostConfirm();
            }
            else if (optionDropdown.options[optionDropdown.value].text.Equals("Chỉnh sửa"))
            {
                OpenEditCommentPanel();
            }

            if (postComment.MemberId.Equals(Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))))
            {
                optionDropdown.value = ownPostOptions.Count - 1;
            }
            else
            {
                optionDropdown.value = otherPostOptions.Count - 1;
            }
        }

        private void OpenEditCommentPanel()
        {
            postBinding.OpenEditCommentPanel(postComment);
        }

        private void OpenDeletePostConfirm()
        {
            forumNewFeedManager.ConfirmAlertManager.Confirm
                (
                    "Xác nhận xoá comment",
                    () =>
                    {
                        ApiHelper.Instance.GetComponent<PostCommentController>()
                            .DeleteCommentByIdCoroutine
                            (
                                (Guid)postComment.CommentId,
                                () =>
                                {
                                    forumNewFeedManager.MessageAlertManager.Alert("Xoá comment thành công", true);
                                    postBinding.ShowCommentPanel();
                                },
                                () =>
                                {
                                    forumNewFeedManager.MessageAlertManager.Alert("Xoá comment thất bại!", false);
                                }
                            );
                    }
                );
        }
    }
}
