using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private RectTransform rectTransform;
        private Transform commentViewPortContent;
        public void Init(PostComment postComment, Transform commentViewPortContent)
        {
            //Set avatar here
            userName.text = postComment.Member.UserName.ToString();
            commentContent.text = postComment.Comment;
            createDateTime.text = postComment.UploadTime.ToString();
            this.commentViewPortContent = commentViewPortContent;
            Invoke(nameof(SetCommentSize), 0.1f);
        }

        private void SetCommentSize()
        {
            rectTransform = GetComponent<RectTransform>();
            Debug.Log(commentHeaderRect.sizeDelta.y + " + " + commentContentRect.rect.height + " = " + (commentHeaderRect.sizeDelta.y + commentContentRect.rect.height));
            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, commentHeaderRect.sizeDelta.y + commentContentRect.rect.height - 10);
            commentViewPortContent.gameObject.SetActive(false);
            commentViewPortContent.gameObject.SetActive(true);

        }

    }
}
