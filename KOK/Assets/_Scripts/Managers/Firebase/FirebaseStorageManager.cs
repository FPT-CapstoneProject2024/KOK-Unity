using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace KOK
{
    public class FirebaseStorageManager : Singleton<FirebaseStorageManager>
    {
        const string STORAGE_BUCKET_URL = "gs://kok-unity.appspot.com";
        const string VOICE_RECORDINGS_REF_NAME = "VoiceRecordings";

        private FirebaseStorage firebaseStorage;
        private StorageReference rootStorageReference;
        private StorageReference voiceRecordingReference;

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

        public void UploadRecordingByLocalFile(string localFilePath, Action<StorageMetadata> onSuccess, Action<AggregateException> onError)
        {
            string fileName = Path.GetFileName(localFilePath);
            StorageReference fileRef = voiceRecordingReference.Child(fileName);

            fileRef.PutFileAsync(localFilePath)
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
                        Debug.Log("Finished uploading...");
                        //LogFileMetadata(metadata);
                        onSuccess?.Invoke(metadata);
                    }
                });
        }

        public void DownloadRecordingFile(string cloudFileName, string localFilePath)
        {
            StorageReference fileRef = voiceRecordingReference.Child(cloudFileName);

            fileRef.GetDownloadUrlAsync()
                .ContinueWith((Task<Uri> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                    }
                    else
                    {
                        string downloadUrl = task.Result.ToString();
                        StartCoroutine(DownloadFileCoroutine(downloadUrl, localFilePath));
                    }
                });
        }

        private IEnumerator DownloadFileCoroutine(string downloadUrl, string localFilePath)
        {
            UnityWebRequest request = UnityWebRequest.Get(downloadUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(localFilePath, request.downloadHandler.data);
                Debug.Log($"File successfully downloaded to: {localFilePath}");
            }
            else
            {
                Debug.LogError($"Failed to download file: {request.error}");
            }
        }

        private void LogFileMetadata(StorageMetadata metadata)
        {
            Debug.Log($"Name: {metadata.Name}");
            Debug.Log($"Path: {metadata.Path}");
            Debug.Log($"Bucket: {metadata.Bucket}");
            Debug.Log($"Generation: {metadata.Generation}");
            Debug.Log($"Md5 Hash: {metadata.Md5Hash}");
        }
    }
}
