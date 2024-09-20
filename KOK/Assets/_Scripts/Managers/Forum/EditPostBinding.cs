using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class EditPostBinding : MonoBehaviour
    {
        [SerializeField] GameObject editPostPanel;
        [SerializeField] TMP_InputField captionInputField;
        [SerializeField] ForumNewFeedManager forumNewFeedManager;
        private Post post;

        public void InitEditPostPanel(Post post, ForumNewFeedManager forumNewFeedManager)
        {
            if (!editPostPanel.activeSelf)
            {
                editPostPanel.SetActive(true);
            }
            this.post = post;
            this.forumNewFeedManager = forumNewFeedManager;
            captionInputField.text = post.Caption;
            
        }
        public void EditPost()
        {
            ApiHelper.Instance.GetComponent<PostController>()
                .UpdatePostCoroutine(
                    (Guid)post.PostId,
                    new()
                    {
                        Caption = captionInputField.text,
                    },
                    (post) => {
                        forumNewFeedManager.MessageAlertManager.Alert("Chỉnh sửa post thành công!", true);
                        forumNewFeedManager.Refresh();
                    },
                    (ex) => {
                        forumNewFeedManager.MessageAlertManager.Alert("Chỉnh sửa post thất bại!", false);
                    }
                );


        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }


    }
}
