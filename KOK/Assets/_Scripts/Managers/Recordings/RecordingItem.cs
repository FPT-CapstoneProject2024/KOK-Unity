using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class RecordingItem : MonoBehaviour
    {
        [SerializeField] AlertManager messageAlert;
        [SerializeField] AlertManager confirmAlert;
        [SerializeField] Recording recording;
        [SerializeField] RecordingLoader recordingLoader; 

        public void Init(Recording recording, AlertManager messageAlert, AlertManager confirmAlert, RecordingLoader recordingLoader)
        {
            this.recording = recording;
            this.confirmAlert = confirmAlert;
            this.messageAlert = messageAlert;
            this.recordingLoader = recordingLoader;
        }
        public void OpenConfirmDelete()
        {
            if (confirmAlert == null) {
                confirmAlert = GameObject.Find("ConfirmAlert").GetComponent<AlertManager>() ;
            }

            confirmAlert.gameObject.SetActive(true);
            confirmAlert.GetComponent<AlertManager>()
                .Confirm($"Xác nhận xoá bản thu âm \n{recording.RecordingName}?",
                            () => {
                                ApiHelper.Instance.GetComponent<RecordingController>()
                                .DeleteRecordingsByIdCoroutine(recording.RecordingId,
                                    () => {
                                        //Refresh recording list
                                        recordingLoader.RefreshRecordingList();
                                        messageAlert.Alert("Xoá thông tin thành công!", true);
                                    },
                                    () => {
                                        messageAlert.Alert("Xoá thông tin thất bại!", false);
                                    }
                                );
                            });

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
