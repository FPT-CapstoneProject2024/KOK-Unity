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

        public void Init(PostComment postComment)
        {
            //Set avatar here
            userName.text = postComment.MemberId.ToString();
            commentContent.text = postComment.Comment;
            createDateTime.text = postComment.UploadTime.ToString();
        
        }

    }
}
