using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static Unity.Burst.Intrinsics.X86.Avx;

public class FFMPEG : MonoBehaviour
{
    private string audioUrl = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/VoiceRecordings%2Ftest.wav?alt=media&token=1c69358a-d7d3-427d-8527-8e3e04623bc4";
    private string videoUrl = "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FTúy%20Âm.mp4?alt=media&token=77cd5236-39e2-4120-95dd-ec64ed64cc3e";

    //private TestVoiceAudioModel testModel;

    public float startTimeInSeconds;
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_Text text3;

    private AudioSource audioSource;
    private AudioClip audioClip;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // Ensure the audio doesn't loop by itself

        //testModel = TestVoiceAudioModel.CreateSample();

        // Start the coroutine to download and process audio and video
        StartCoroutine(DownloadAndProcess());
    }

    // Test function
    IEnumerator DownloadAndProcess()
    {
        // Download audio and video files into the AudioProcess folder
        //yield return StartCoroutine(DownloadFile(audioUrl, "audio.wav", "AudioProcess"));
        //yield return StartCoroutine(DownloadFile(videoUrl, "video.mp4", "AudioProcess"));

        // Process the downloaded files
        string audioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "audio.wav");
        if (File.Exists(audioPath))
        {
            audioClip = LoadAudioClip(audioPath);
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            text3.text = "error";
        }
        string videoPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "video.mp4");

        yield return StartCoroutine(CombineAudioAndVideo(audioPath, videoPath));
        //yield return StartCoroutine(AddSilenceAndTrimAudio(audioPath, startTimeInSeconds));
    }

    IEnumerator DownloadFile(string url, string fileName, string folderName)
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

    AudioClip LoadAudioClip(string path)
    {
        // Load the audio clip from the specified path synchronously
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV); //file:// for mobile file path, don't ask
        www.SendWebRequest();

        while (!www.isDone)
        {
            // Wait for the request to complete
        }

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

    IEnumerator CombineAudioAndVideo(string audioPath, string videoPath)
    {
        string combinedFilePath = Path.Combine(Application.persistentDataPath, "AudioProcess", "combined_output.mp4");

        // FFmpeg command
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

    IEnumerator AddSilenceAndTrimAudio(string audioPath, float startTimeInSeconds)
    {
        // Calculate the duration from StartTime and EndTime
        //float duration = testModel.EndTime - startTimeInSeconds;

        // Paths for temporary and final audio files
        string tempAudioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "temp_audio.wav");
        string finalAudioPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "final_audio.wav");

        if (startTimeInSeconds < 0)
        {
            // Generate silence for the absolute value of negative start time
            float silenceDuration = Math.Abs(startTimeInSeconds);
            string silenceArgs = $"-y -f lavfi -t {silenceDuration} -i anullsrc=r=44100:cl=stereo \"{tempAudioPath}\"";
            yield return RunFFmpeg(silenceArgs);

            // Concatenate silence and the original audio
            string concatArgs = $"-y -i \"{tempAudioPath}\" -i \"{audioPath}\" -filter_complex \"[0][1]concat=n=2:v=0:a=1[out]\" -map \"[out]\" \"{finalAudioPath}\"";
            yield return RunFFmpeg(concatArgs);
        }
        else
        {
            // Trim the audio based on start time and duration
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
}
