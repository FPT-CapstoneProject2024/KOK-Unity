using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using KOK.ApiHandler.Context;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.PostComment;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace KOK
{
    public class PostCommentLoader : MonoBehaviour
    {
        private List<Post> postList = new List<Post>();
        private List<PostComment> postCommentList = new List<PostComment>();
        private List<PostComment> postReplyList = new List<PostComment>();
        private string postCommentsResourceUrl = string.Empty;
        public GameObject displayPanel;
        public GameObject parentCommentPrefab;
        public GameObject childCommentDisplayContent;
        public GameObject childCommentPrefab;
        //public GameObject childCommentDisplayPanel;
        public GameObject fullDisplayPanel;
        public GameObject fullDisplayContent;
        public GameObject replyPrefab;
        private int currentPostIndex = 0;

        void Start()
        {
            postCommentsResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.PostComments_Resource;
        }


        /* public void GetCommentsFilterPaging()
         {
             postCommentList.Clear();
             FindAnyObjectByType<ApiHelper>().gameObject
                 .GetComponent<PostCommentController>()
                 .GetPostCommentsFilterPagingCoroutine(new PostCommentFilter(),
                                                     new PostCommentOrderFilter(),
                                                     new PagingRequest(),
                                                     CommentsGenerate,
                                                     OnError
                 );
         }*/

        public void GetPostComment(Guid postId)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PostCommentController>()
                .GetPostCommentsByPostIdCoroutine(postId,
                                            ParentCommentsGenerate,
                                            OnError
                );
        }

        public void GetReplies(Guid commentId)
        {
            postReplyList.Clear();
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PostCommentController>()
                .GetPostRepliesByCommentIdCoroutine(commentId,
                                                    ChildCommentsGenerate,
                                                    OnError
                );
        }

        void ParentCommentsGenerate(List<PostComment> postCommentList)
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (PostComment postComment in postCommentList)
            {
                if (postComment.CommentType.Equals(PostCommentType.PARENT) && postComment.Status.Equals(PostCommentStatus.ACTIVE))
                {
                    GameObject parentCommentObj = Instantiate(parentCommentPrefab, displayPanel.transform);
                    parentCommentObj.transform.GetChild(1).GetComponent<TMP_Text>().text = postComment.MemberId.ToString();
                    parentCommentObj.transform.GetChild(2).GetComponent<TMP_Text>().text = TruncateComment(postComment.Comment);
                    parentCommentObj.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        OpenComment(postComment.MemberId, postComment.Comment, postComment.CommentId.Value);
                    });
                    //commentObjects[postComment.CommentId.Value] = parentCommentObj;
                }
            }
        }

        void ChildCommentsGenerate(List<PostComment> postCommentList)
        {
            foreach (PostComment postComment in postCommentList)
            {
                if (postComment.CommentType.Equals(PostCommentType.CHILD) && postComment.Status.Equals(PostCommentStatus.ACTIVE))
                {
                    GenerateFullComment(postComment.MemberId, postComment.Comment, childCommentDisplayContent, true, replyPrefab);             
                }
            }
        }

        private void OnError(string error)
        {
            foreach (Transform child in displayPanel.transform)
            {
                Destroy(child.gameObject);
            }
            Debug.LogError(error);
        }

        private string TruncateComment(string comment)
        {
            if (comment.Length > 64)
            {
                return comment.Substring(0, 61) + " ...";
            }
            return comment;
        }

        private void OpenComment(Guid memberId, string fullText, Guid commentId)
        {
            fullDisplayPanel.SetActive(true);

            GenerateFullComment(memberId, fullText, fullDisplayContent, false, childCommentPrefab);

            // Generate replies
            GetReplies(commentId);
        }

        private void GenerateFullComment(Guid memberId, string fullText, GameObject displayContent, bool isChild, GameObject prefab)
        {
            if (isChild)
            {
                GameObject fullDisplayObj = Instantiate(prefab, displayContent.transform);

                TMP_Text nameTMP = fullDisplayObj.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
                TMP_Text commentTMP = fullDisplayObj.transform.GetChild(0).transform.GetChild(2).GetComponent<TMP_Text>();

                nameTMP.text = memberId.ToString();
                commentTMP.text = fullText;

                RectTransform rectTransform = fullDisplayObj.GetComponent<RectTransform>();
                RectTransform rectTransform2 = fullDisplayObj.transform.GetChild(0).GetComponent<RectTransform>();

                // Calculate the preferred height of the text
                commentTMP.enableWordWrapping = true;
                commentTMP.ForceMeshUpdate();
                float preferredHeight = commentTMP.GetPreferredValues().y + 30;

                if(preferredHeight < 51f)
                {
                    preferredHeight = 51f;
                }
                // Adjust the RectTransform size to fit the full comment
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
                rectTransform2.sizeDelta = new Vector2(rectTransform2.sizeDelta.x, preferredHeight);

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform2);
            }
            else
            {
                GameObject fullDisplayObj = Instantiate(prefab, displayContent.transform);

                TMP_Text nameTMP = fullDisplayObj.transform.GetChild(1).GetComponent<TMP_Text>();
                TMP_Text commentTMP = fullDisplayObj.transform.GetChild(2).GetComponent<TMP_Text>();

                nameTMP.text = memberId.ToString();
                commentTMP.text = fullText;

                RectTransform rectTransform = fullDisplayObj.GetComponent<RectTransform>();

                // Calculate the preferred height of the text
                commentTMP.enableWordWrapping = true;
                commentTMP.ForceMeshUpdate();
                float preferredHeight = commentTMP.GetPreferredValues().y + 30;

                if (preferredHeight < 51f)
                {
                    preferredHeight = 51f;
                }

                // Adjust the RectTransform size to fit the full comment
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}