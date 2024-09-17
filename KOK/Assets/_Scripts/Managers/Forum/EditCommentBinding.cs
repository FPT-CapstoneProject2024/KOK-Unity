using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class EditCommentBinding : MonoBehaviour
    {
        [SerializeField] GameObject editCommentPanel;
        [SerializeField] TMP_InputField captionInputField;
        PostBinding postBinding;
        ForumNewFeedManager forumNewFeedManager;
        private PostComment comment;

        public void InitEditCommentPanel(PostComment comment, ForumNewFeedManager forumNewFeedManager, PostBinding postBinding)
        {
            if (!editCommentPanel.activeSelf)
            {
                editCommentPanel.SetActive(true);
            }
            this.comment = comment;
            this.postBinding = postBinding;
            this.forumNewFeedManager = forumNewFeedManager;
            captionInputField.text = comment.Comment;

        }
        public void EditComment()
        {
            if (captionInputField.text.IsNullOrEmpty()) { return; }
            ApiHelper.Instance.GetComponent<PostCommentController>()
                .UpdateCommentCoroutine(
                    (Guid)comment.CommentId,
                    new()
                    {
                        Comment = captionInputField.text,
                    },
                    (comment) => {
                        //forumNewFeedManager.MessageAlertManager.Alert("Chỉnh sửa comment thành công!", true);
                        gameObject.SetActive(false);
                        postBinding.ShowCommentPanel();
                    },
                    (ex) => {
                        forumNewFeedManager.MessageAlertManager.Alert("Chỉnh sửa comment thất bại!", false);
                    }
                );


        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
