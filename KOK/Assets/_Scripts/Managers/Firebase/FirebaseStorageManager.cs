using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace KOK
{
    public class FirebaseStorageManager : Singleton<FirebaseStorageManager>
    {
        const string STORAGE_BUCKET_URL = "gs://kok-unity.appspot.com";
        const string VOICE_RECORDINGS_REF_NAME = "VoiceRecordings";
        const string ROOM_LOG_REF_NAME = "RoomLogs";

        private FirebaseStorage firebaseStorage;
        private StorageReference rootStorageReference;
        private StorageReference voiceRecordingReference;
        private StorageReference roomLoggingReference;

        private void Start()
        {
            InitializeFirebaseStorage();
        }

        private void InitializeFirebaseStorage()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    firebaseStorage = FirebaseStorage.DefaultInstance;
                    rootStorageReference = firebaseStorage.GetReferenceFromUrl(STORAGE_BUCKET_URL);
                    voiceRecordingReference = rootStorageReference.Child(VOICE_RECORDINGS_REF_NAME);
                    roomLoggingReference = rootStorageReference.Child(ROOM_LOG_REF_NAME);
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                }
            });

        }

        public void UploadRecordingByStream(string fileName, byte[] dataStream)
        {
            StorageReference fileRef = voiceRecordingReference.Child(fileName);

            fileRef.PutBytesAsync(dataStream)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError(task.Exception.ToString());
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and md5hash.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);
                        Debug.Log($"File metadata: {metadata}");
                    }
                });
        }

        public void UploadVoiceRecordingByLocalFile(string localFilePath, Action<StorageMetadata> onSuccess, Action<AggregateException> onError)
        {
            string fileName = Path.GetFileName(localFilePath);
            StorageReference fileRef = voiceRecordingReference.Child(fileName);
            string local_file_uri = string.Format("{0}://{1}", Uri.UriSchemeFile, localFilePath);
            fileRef.PutFileAsync(local_file_uri)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        onError?.Invoke(task.Exception);
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and download URL.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading voice record...");
                        onSuccess?.Invoke(metadata);
                    }
                });
        }

        public void GetRecordingDownloadUrl(string localFilePath, Action<Uri> onSuccess, Action<Uri> onError)
        {
            string fileName = Path.GetFileName(localFilePath);
            StorageReference fileRef = voiceRecordingReference.Child(fileName);
            fileRef.GetDownloadUrlAsync().ContinueWith(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    onError?.Invoke(task.Result);
                }
                else
                {
                    Debug.Log("File downloaded successfully.");
                    onSuccess?.Invoke(task.Result);
                }
            });
        }

        public async Task<string> GetRecordingDownloadUrl(string cloudFileName)
        {
            StorageReference fileRef = voiceRecordingReference.Child(cloudFileName);
            try
            {
                Uri downloadUri = await fileRef.GetDownloadUrlAsync();
                return downloadUri.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to get voice recording download Url: " + ex.ToString());
                return null;
            }
        }

        public void DownloadVoiceRecordingFile(string localFilePath, Action onSuccess, Action onError)
        {
            //    string fileName = Path.GetFileName(localFilePath);
            //    StorageReference fileRef = voiceRecordingReference.Child(fileName);
            string fileName = Path.GetFileName(localFilePath);
            fileName = fileName.Replace(".zip", "");
            fileName = fileName.Replace(".wav", "");
            fileName = fileName + ".zip";
            StorageReference fileRef = voiceRecordingReference.Child(fileName);
            Debug.Log(fileRef
                + "\n" + localFilePath
                + "\n" + fileName);

            fileRef.GetFileAsync(localFilePath).ContinueWith(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError(task.Exception.ToString());
                    onError?.Invoke();
                }
                else
                {
                    Debug.Log("File downloaded successfully at " + fileName);
                    try
                    {
                        onSuccess?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception in onSuccess: " + ex.ToString());
                    }
                }
            });
        }

        //public async Task DownloadVoiceRecordingFile(string downloadUrl, string localFilePath)
        //{
        //    UnityWebRequest request = UnityWebRequest.Get(downloadUrl);
        //    var operation = request.SendWebRequest();

        //    while (!operation.isDone)
        //    {
        //        await Task.Yield();
        //    }

        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        File.WriteAllBytes(localFilePath, request.downloadHandler.data);
        //        Debug.Log($"File successfully downloaded to: {localFilePath}");
        //    }
        //    else
        //    {
        //        Debug.LogError($"Failed to download file: {request.error}");
        //    }
        //}

        private void LogFileMetadata(StorageMetadata metadata)
        {
            Debug.Log($"Name: {metadata.Name}");
            Debug.Log($"Path: {metadata.Path}");
            Debug.Log($"Bucket: {metadata.Bucket}");
            Debug.Log($"Generation: {metadata.Generation}");
            Debug.Log($"Md5 Hash: {metadata.Md5Hash}");
        }

        //public IEnumerator DownLoadRoomLogFile(string downloadUrl, string localFilePath, Action<string> onSuccess, Action<string> onError)
        //{
        //    UnityWebRequest request = UnityWebRequest.Get(downloadUrl);
        //    //var operation = request.SendWebRequest();

        //    yield return request.SendWebRequest();

        //    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        //    {
        //        Debug.Log($"{downloadUrl} - GET - {request.error}");
        //        onError?.Invoke(request.downloadHandler.text);
        //    }
        //    else
        //    {
        //        onSuccess?.Invoke(request.downloadHandler.text);
        //    }
        //}
        public void DownLoadRoomLogFile(string localFilePath, Action onSuccess, Action onError)
        {
            string fileName = Path.GetFileName(localFilePath);
            StorageReference fileRef = roomLoggingReference.Child(fileName);
            fileRef.GetFileAsync(localFilePath).ContinueWith(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    onError?.Invoke();
                }
                else
                {
                    Debug.Log("File downloaded successfully.");
                    onSuccess?.Invoke();
                }
            });
        }

        public void UploadRoomLogFile(string localFilePath, Action<StorageMetadata> onSuccess, Action<AggregateException> onError)
        {
            string fileName = Path.GetFileName(localFilePath);
            //fileName.Replace(".txt", "");
            StorageReference fileRef = roomLoggingReference.Child(fileName);


            string local_file_uri = string.Format("{0}://{1}", Uri.UriSchemeFile, localFilePath);

            fileRef.PutFileAsync(local_file_uri)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError(task.Exception.ToString());
                        onError?.Invoke(task.Exception);
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and download URL.
                        StorageMetadata metadata = task.Result;
                        Debug.Log("Finished uploading...");
                        onSuccess?.Invoke(metadata);
                    }
                });
        }
    }
}
