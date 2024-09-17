using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class ReportBinding : MonoBehaviour
    {
        public CreateReportRequest reportRequest { get; set; } = new();
        private Guid reportedContentId;
        [SerializeField] TMP_Dropdown categoryDropdown;
        [SerializeField] TMP_InputField descriptionInput;
        ForumNewFeedManager forumNewFeedManager;

        public void Init(Guid reportedContentId, Guid reportedMemberId, ReportType reportType, ForumNewFeedManager forumNewFeedManager)
        {
            reportRequest = new();
            reportRequest.ReporterId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId));
            reportRequest.ReportedAccountId = reportedMemberId;
            reportRequest.ReportType = (int)reportType;
            this.reportedContentId = reportedContentId;

            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(ReportCategory.List);
            categoryDropdown.value = 0;
            descriptionInput.text = "";

            this.forumNewFeedManager = forumNewFeedManager;
        }

        public void SendReport()
        {
            switch (reportRequest.ReportType)
            {
                case 0:
                    reportRequest.PostId = reportedContentId;
                    break;
                case 1:
                    reportRequest.CommentId = reportedContentId;
                    break;
                case 2:
                    reportRequest.RoomId = reportedContentId;
                    break;
                default:
                    reportRequest.RoomId = reportedContentId;
                    break;
            }
            reportRequest.ReportCategory = categoryDropdown.value;
            reportRequest.Reason = descriptionInput.text;
            Debug.Log(reportRequest);
            ApiHelper.Instance.GetComponent<ReportController>()
                .CreateReportCoroutine(
                    reportRequest,
                    (report) =>
                    {
                        forumNewFeedManager.MessageAlertManager.Alert("Gửi báo cáo thành công!", true);
                        gameObject.SetActive(false);
                    },
                    (ex) =>
                    {
                        forumNewFeedManager.MessageAlertManager.Alert("Gửi báo cáo thất bại!", false);
                    }
                );
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
