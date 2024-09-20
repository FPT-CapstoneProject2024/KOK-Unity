
using KOK;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class FFMPEG : MonoBehaviour
{
    public float startTimeInSeconds;
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_Text text3;

    //private AudioSource audioSource;
    private AudioClip audioClip;

    void Start()
    {
        /*audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;*/

        //StartCoroutine(DownloadAndProcess());
    }

    //IEnumerator DownloadAndProcess()
    //{
    //    yield return StartCoroutine(DownloadFile(audioUrl, "test.wav", "AudioProcess"));
    //    yield return StartCoroutine(DownloadFile(videoUrl, "Tuý Âm.mp4", "AudioProcess"));

    //    string audioPath = Path.Combine(Application.persistentDataPath, "test.wav");
    //    string videoPath = Path.Combine(Application.persistentDataPath, "Tuý Âm.mp4");

    //    //yield return StartCoroutine(ExtractAudioFromVideo(videoPath));
    //    //yield return StartCoroutine(CombineAudioAndVideo(audioPath, videoPath));
    //}

    public void DownloadFile2(string localZipFilePath, Action onSuccess, Action onError)
    {
        Debug.LogError("DownloadFile2");
        string folderPath = Path.Combine(Application.persistentDataPath, "AudioProcess");
        //string filePath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Get download url
        string downloadUrl = String.Empty;

        // Download .zip file
        string wavFilePath = string.Empty;
        FirebaseStorageManager.Instance
            .DownloadVoiceRecordingFile(localZipFilePath,
                                        () =>
                                        {
                                            wavFilePath = FileCompressionHelper.ExtractWavFileFromZip(localZipFilePath, folderPath);
                                            onSuccess.Invoke();
                                        },
                                        () =>
                                        {
                                            Debug.LogError("Failed to extract zipped voice recording file");
                                           onError.Invoke();
                                        }
            );
    }

    // Cleanup 
    public void CleanUp(string localZipFilePath)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "AudioProcess");
        string wavFilePath = FileCompressionHelper.ExtractWavFileFromZip(localZipFilePath, folderPath);
        try
        {
            if (File.Exists(localZipFilePath))
            {
                File.Delete(localZipFilePath);
                Debug.Log($"Deleted local zip file: {localZipFilePath}");
            }

            if (File.Exists(wavFilePath))
            {
                File.Delete(wavFilePath);
                Debug.Log($"Deleted local wav file: {wavFilePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to clean up files: {ex}");
        }
    }

    public IEnumerator DownloadFile(string url, string fileName, string folderName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        string filePath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading file: " + request.error);
            text2.text = request.error;
        }
        else
        {
            File.WriteAllBytes(filePath, request.downloadHandler.data);
            Debug.Log("Downloaded file: " + filePath);
            if (File.Exists(filePath))
            {
                text2.text = filePath;
            }
            else
            {
                text2.text = "download error";
            }
        }
    }

    // Message: Download xuong folder Music chi hoat dong voi wav file, mp4 file k download xuong do dc
    public IEnumerator DownloadFileToMusicFolder(string url, string fileName)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading file: " + request.error);
            text2.text = request.error;
        }
        else
        {
            byte[] fileData = request.downloadHandler.data;

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            try
            {
                using (MemoryStream stream = new MemoryStream(fileData))
                {
                    AndroidJavaObject fileInputStream = new AndroidJavaObject("java.io.ByteArrayInputStream", stream.ToArray());
                    AndroidJavaClass fileUtils = new AndroidJavaClass("com.capstoneproject2024.kok.SaveToMusicFolder");

                    bool success = fileUtils.CallStatic<bool>("saveFileToMusicFolder", context, fileInputStream, fileName);

                    if (success)
                    {
                        Debug.Log("File saved successfully to Music folder: " + fileName);
                        text2.text = "Saved to Music folder: " + fileName;
                    }
                    else
                    {
                        Debug.LogError("Failed to save file to Music folder.");
                        text2.text = "Failed to save to Music folder";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception: " + e.Message);
                text2.text = "Exception: " + e.Message;
            }
        }
    }

    public AudioClip LoadAudioClipWavHelper(string path)
    {
        return WavHelper.WavFileToAudioClip(path);
    }

    public AudioClip LoadAudioClip(string path)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV);
        www.SendWebRequest();

        while (!www.isDone) { }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio clip: " + www.error);
            audioClip = null;
        }
        else
        {
            audioClip = DownloadHandlerAudioClip.GetContent(www);
        }
        return audioClip;
    }

    public IEnumerator CombineAudioAndVideo(string audioPath, string videoPath)
    {
        string combinedFilePath = Path.Combine(Application.persistentDataPath, "combined_output.mp4");

        string arguments = $"-y -i \"{videoPath}\" -i \"{audioPath}\" -filter_complex \"[0:a][1:a]amix=inputs=2:duration=first:dropout_transition=2\" -c:v copy -c:a aac \"{combinedFilePath}\"";

        yield return RunFFmpeg(arguments);

        if (File.Exists(combinedFilePath))
        {
            Debug.Log("Combined file created successfully: " + combinedFilePath);
        }
        else
        {
            Debug.LogError("Failed to create combined file.");
        }

        yield return null;
    }

    public IEnumerator CombineAudioAndAudio(string audioPath1, string audioPath2)
    {
        string combinedFilePath = Path.Combine(Application.persistentDataPath, "combined_audio.wav");

        string arguments = $"-y -i \"{audioPath1}\" -i \"{audioPath2}\" -filter_complex \"[0:a][1:a]amix=inputs=2:duration=first:dropout_transition=2\" -c:a pcm_s16le \"{combinedFilePath}\"";

        yield return RunFFmpeg(arguments);

        if (File.Exists(combinedFilePath))
        {
            Debug.Log("Combined file created successfully: " + combinedFilePath);
        }
        else
        {
            Debug.LogError("Failed to create combined file.");
        }

        yield return null;
    }

    public IEnumerator ExtractAudioFromVideo(string videoPath)
    {
        string extractedAudioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "extractedAudio.wav");
        Debug.Log(videoPath);
        string arguments = $"-i \"{videoPath}\" -q:a 0 -map a \"{extractedAudioPath}\"";

        yield return RunFFmpeg(arguments);

        if (File.Exists(extractedAudioPath))
        {
            Debug.Log("Extracted file created successfully: " + extractedAudioPath);
        }
        else
        {
            Debug.LogError("Failed to create extracted file.");
        }

        yield return null;
    }

    public IEnumerator AddSilenceAndTrimAudio(string audioPath, float startTimeInSeconds, int index)
    {
        string tempAudioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", $"{index}temp_silence.wav");
        string finalAudioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", $"{index}temp_audio.wav");

        if (startTimeInSeconds < 0)
        {
            float silenceDuration = Math.Abs(startTimeInSeconds);
            string silenceArgs = $"-y -f lavfi -t {silenceDuration} -i anullsrc=r=44100:cl=stereo \"{tempAudioPath}\"";
            yield return RunFFmpeg(silenceArgs);

            string concatArgs = $"-y -i \"{tempAudioPath}\" -i \"{audioPath}\" -filter_complex \"[0][1]concat=n=2:v=0:a=1[out]\" -map \"[out]\" \"{finalAudioPath}\"";
            yield return RunFFmpeg(concatArgs);
        }
        else
        {
            string trimArgs = $"-y -i \"{audioPath}\" -ss {startTimeInSeconds} -c copy \"{finalAudioPath}\"";
            yield return RunFFmpeg(trimArgs);
        }

        Debug.Log("Silence added and audio trimmed successfully: " + finalAudioPath);
    }

    IEnumerator RunFFmpeg(string arguments)
    {
        using (AndroidJavaClass configClass = new AndroidJavaClass("com.arthenica.mobileffmpeg.Config"))
        {
            AndroidJavaObject paramVal = new AndroidJavaClass("com.arthenica.mobileffmpeg.Signal").GetStatic<AndroidJavaObject>("SIGXCPU");
            configClass.CallStatic("ignoreSignal", new object[] { paramVal });

            using (AndroidJavaClass ffmpeg = new AndroidJavaClass("com.arthenica.mobileffmpeg.FFmpeg"))
            {
                int rc = ffmpeg.CallStatic<int>("execute", new object[] { arguments });
                Debug.Log("MobileFFmpeg result: " + rc);
            }
        }

        yield return null;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
