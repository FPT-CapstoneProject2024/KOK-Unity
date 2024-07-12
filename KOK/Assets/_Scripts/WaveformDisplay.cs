/*using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveformSlider : MonoBehaviour
{
    public RawImage waveformImage;
    public Slider slider;
    public ScrollRect scrollRect;
    public RectTransform playbackIndicator;
    public int height = 200;
    public Color waveformColor = Color.white;
    public Color backgroundColor = Color.black;
    public TMP_InputField startTimeInputField;
    public Button refreshButton;

    private AudioSource audioSource;
    public AudioClip audioClip;
    private Texture2D waveformTexture;
    private bool isDragging = false;
    private float endBufferTime = 0.05f; // Buffer time to stop playback before the actual end (for waveform display bug fixing)

    public List<string> audioNumbers;
    public List<AudioClip> audioClips;
    public List<RawImage> waveformImages;
    public List<Slider> sliders;
    public List<ScrollRect> scrollRects;
    public List<RectTransform> playbackIndicatorRectTransforms;

    void Start()
    {
        CreateWaveformAndAudioSource();
    }

    void Update()
    {
        if (!isDragging)
        {
            if (audioSource.time >= audioClip.length - endBufferTime)
            {
                // Pause the audio near the end
                audioSource.Pause();
            }

            slider.value = audioSource.time;

            // Calculate the normalized playback position
            float normalizedPosition = audioSource.time / audioClip.length;

            // Update the horizontal scroll position to keep the indicator fixed
            UpdateScrollRect(normalizedPosition);
        }
        else
        {
            isDragging = false;
        }
    }

    void CreateWaveformAndAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = false; // Ensure the audio doesn't loop by itself

        int waveformWidth = Mathf.CeilToInt(audioClip.length * 100); // 100 pixels per second
        int extraWidth = Mathf.CeilToInt(scrollRect.viewport.rect.width); // Extra viewport width at the end for bug fixing

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        waveformTexture = CreateWaveformTexture(samples, waveformWidth, extraWidth, height);
        waveformImage.texture = waveformTexture;
        waveformImage.rectTransform.sizeDelta = new Vector2(waveformWidth + extraWidth, height);

        slider.maxValue = audioClip.length;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.onValueChanged.AddListener(_ => isDragging = true);

        // Set up the refresh button click event
        refreshButton.onClick.AddListener(RefreshTrack);

        audioSource.Play();
    }

    Texture2D CreateWaveformTexture(float[] samples, int waveformWidth, int extraWidth, int height)
    {
        Texture2D texture = new Texture2D(waveformWidth + extraWidth, height);
        Color[] colors = new Color[(waveformWidth + extraWidth) * height];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = backgroundColor;
        }

        int stepSize = samples.Length / waveformWidth;

        for (int x = 0; x < waveformWidth; x++)
        {
            float sampleValue = samples[x * stepSize] * height / 2;
            int yStart = (int)((height / 2) - sampleValue);
            int yEnd = (int)((height / 2) + sampleValue);

            for (int y = yStart; y <= yEnd; y++)
            {
                colors[y * (waveformWidth + extraWidth) + x] = waveformColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    void OnSliderValueChanged(float value)
    {
        if (value < 0 || value >= audioClip.length - endBufferTime)
        {
            // If seeking to the end, pause the audio
            audioSource.Pause();
        }
        else
        {
            audioSource.time = value;

            // Resume playing if paused and seeking backward
            if (!audioSource.isPlaying && audioSource.time < audioClip.length - endBufferTime)
            {
                audioSource.Play();
            }
        }

        // Calculate the normalized playback position
        float normalizedPosition = value / audioClip.length;

        // Update the horizontal scroll position to keep the indicator fixed (at the start)
        UpdateScrollRect(normalizedPosition);
    }

    void UpdateScrollRect(float normalizedPosition)
    {
        float indicatorPosition = normalizedPosition * (waveformImage.rectTransform.rect.width - scrollRect.viewport.rect.width);

        float scrollWidth = scrollRect.viewport.rect.width;
        float contentWidth = waveformImage.rectTransform.rect.width;

        float desiredScrollPosition = indicatorPosition - playbackIndicator.anchoredPosition.x;
        float maxScrollPosition = contentWidth - scrollWidth;

        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(desiredScrollPosition / maxScrollPosition);
    }

    void RefreshTrack()
    {
        float startTimeInSeconds;
        if (!float.TryParse(startTimeInputField.text, out startTimeInSeconds))
        {
            Debug.LogWarning("Invalid input for startTimeInSeconds");
            return;
        }

        if (startTimeInSeconds >= 0)
        {
            // Trim audio clip at the start
            AudioClip trimmedClip = TrimAudioClip(audioClip, startTimeInSeconds, audioClip.length);
            if (trimmedClip != null)
            {
                audioSource.clip = trimmedClip;
            }
            else
            {
                Debug.LogWarning("Failed to trim audio clip");
                return;
            }
        }
        else
        {
            // Add silence at the start
            AudioClip extendedClip = ExtendAudioClip(audioClip, Mathf.Abs(startTimeInSeconds), audioClip.length);
            if (extendedClip != null)
            {
                audioSource.clip = extendedClip;
            }
            else
            {
                Debug.LogWarning("Failed to extend audio clip");
                return;
            }
        }

        // Reset audio playback
        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.Play();

        // Recreate waveform texture based on the updated audio clip
        int waveformWidth = Mathf.CeilToInt(audioSource.clip.length * 100);
        int extraWidth = Mathf.CeilToInt(scrollRect.viewport.rect.width);

        float[] samples = new float[audioSource.clip.samples];
        audioSource.clip.GetData(samples, 0);

        waveformTexture = CreateWaveformTexture(samples, waveformWidth, extraWidth, height);
        waveformImage.texture = waveformTexture;
        waveformImage.rectTransform.sizeDelta = new Vector2(waveformWidth + extraWidth, height);

        // Reset slider
        slider.maxValue = audioSource.clip.length;
        slider.value = 0f;

        // Calculate the new normalized playback position
        float normalizedPosition = slider.value / audioSource.clip.length;

        // Update the horizontal scroll position to keep the indicator fixed
        UpdateScrollRect(normalizedPosition);
    }

    AudioClip TrimAudioClip(AudioClip clip, float startTime, float endTime)
    {
        if (startTime < 0 || endTime > clip.length || startTime >= endTime)
        {
            Debug.LogWarning("Invalid trim parameters");
            return null;
        }

        int startSample = Mathf.FloorToInt(startTime * clip.frequency);
        int endSample = Mathf.FloorToInt(endTime * clip.frequency);
        float[] data = new float[(endSample - startSample) * clip.channels];

        clip.GetData(data, startSample);

        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", data.Length, clip.channels, clip.frequency, false);
        trimmedClip.SetData(data, 0);

        return trimmedClip;
    }

    AudioClip ExtendAudioClip(AudioClip clip, float silenceDuration, float newLength)
    {
        if (silenceDuration <= 0 || newLength <= 0)
        {
            Debug.LogWarning("Invalid extension parameters");
            return null;
        }

        int silenceSamples = Mathf.FloorToInt(silenceDuration * clip.frequency);
        int totalSamples = Mathf.FloorToInt(newLength * clip.frequency);

        float[] data = new float[totalSamples * clip.channels];
        clip.GetData(data, 0);

        AudioClip extendedClip = AudioClip.Create("ExtendedClip", data.Length + silenceSamples, clip.channels, clip.frequency, false);

        // Fill the beginning with silence
        for (int i = data.Length - 1; i >= silenceSamples; i--)
        {
            data[i] = data[i - silenceSamples];
        }

        for (int i = 0; i < silenceSamples; i++)
        {
            data[i] = 0f; // Silence, don't ask
        }

        extendedClip.SetData(data, 0);

        return extendedClip;
    }
}



*/


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveformDisplay : MonoBehaviour
{
    public List<RawImage> waveformImages;
    public List<Slider> sliders;
    public List<ScrollRect> scrollRects;
    public List<RectTransform> playbackIndicators;
    public List<TMP_InputField> startTimeInputFields;
    private List<AudioSource> audioSources = new List<AudioSource>();
    private List<AudioClip> originalAudioClips = new List<AudioClip>(); // Store original audio clips
    public List<AudioClip> audioClips;
    private List<Texture2D> waveformTextures = new List<Texture2D>();

    public Button refreshButton;

    public int height = 200;
    public Color waveformColor = Color.white;
    public Color backgroundColor = Color.black;
    private bool isDragging = false;
    private float endBufferTime = 0.05f; // Buffer time to stop playback before the actual end (for waveform display bug fixing)

    void Start()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            originalAudioClips.Add(audioClips[i]); // Store the original clip
            CreateWaveformAndAudioSource(i);
        }

        refreshButton.onClick.AddListener(RefreshTracks);
    }

    void Update()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (!isDragging)
            {
                if (audioSources[i].time >= audioClips[i].length - endBufferTime)
                {
                    // Pause the audio near the end
                    audioSources[i].Pause();
                }

                sliders[i].value = audioSources[i].time;

                // Calculate the normalized playback position
                float normalizedPosition = audioSources[i].time / audioClips[i].length;

                // Update the horizontal scroll position to keep the indicator fixed
                UpdateScrollRect(i, normalizedPosition);
            }
            else
            {
                isDragging = false;
            }
        }
    }

    void CreateWaveformAndAudioSource(int index)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClips[index];
        audioSource.loop = false; // Ensure the audio doesn't loop by itself
        audioSources.Add(audioSource);

        int waveformWidth = Mathf.CeilToInt(audioClips[index].length * 100); // 100 pixels per second
        int extraWidth = Mathf.CeilToInt(scrollRects[index].viewport.rect.width); // Extra viewport width at the end for bug fixing

        float[] samples = new float[audioClips[index].samples];
        audioClips[index].GetData(samples, 0);

        Texture2D waveformTexture = CreateWaveformTexture(samples, waveformWidth, extraWidth, height);
        waveformTextures.Add(waveformTexture);
        waveformImages[index].texture = waveformTexture;
        waveformImages[index].rectTransform.sizeDelta = new Vector2(waveformWidth + extraWidth, height);

        sliders[index].maxValue = audioClips[index].length;
        sliders[index].onValueChanged.AddListener(value => OnSliderValueChanged(index, value));
        sliders[index].onValueChanged.AddListener(_ => isDragging = true);

        audioSource.Play();
    }

    Texture2D CreateWaveformTexture(float[] samples, int waveformWidth, int extraWidth, int height)
    {
        Texture2D texture = new Texture2D(waveformWidth + extraWidth, height);
        Color[] colors = new Color[(waveformWidth + extraWidth) * height];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = backgroundColor;
        }

        int stepSize = samples.Length / waveformWidth;

        for (int x = 0; x < waveformWidth; x++)
        {
            float sampleValue = samples[x * stepSize] * height / 2;
            int yStart = (int)((height / 2) - sampleValue);
            int yEnd = (int)((height / 2) + sampleValue);

            for (int y = yStart; y <= yEnd; y++)
            {
                colors[y * (waveformWidth + extraWidth) + x] = waveformColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    void OnSliderValueChanged(int index, float value)
    {
        if (value < 0 || value >= audioClips[index].length - endBufferTime)
        {
            // If seeking to the end, pause the audio
            audioSources[index].Pause();
        }
        else
        {
            audioSources[index].time = value;

            // Resume playing if paused and seeking backward
            if (!audioSources[index].isPlaying && audioSources[index].time < audioClips[index].length - endBufferTime)
            {
                audioSources[index].Play();
            }
        }

        // Calculate the normalized playback position
        float normalizedPosition = value / audioClips[index].length;

        // Update the horizontal scroll position to keep the indicator fixed (at the start)
        UpdateScrollRect(index, normalizedPosition);
    }

    void UpdateScrollRect(int index, float normalizedPosition)
    {
        float indicatorPosition = normalizedPosition * (waveformImages[index].rectTransform.rect.width - scrollRects[index].viewport.rect.width);

        float scrollWidth = scrollRects[index].viewport.rect.width;
        float contentWidth = waveformImages[index].rectTransform.rect.width;

        float desiredScrollPosition = indicatorPosition - playbackIndicators[index].anchoredPosition.x;
        float maxScrollPosition = contentWidth - scrollWidth;

        scrollRects[index].horizontalNormalizedPosition = Mathf.Clamp01(desiredScrollPosition / maxScrollPosition);
    }

    void RefreshTracks()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            // Reset to the original clip before applying changes
            audioSources[i].clip = originalAudioClips[i];
            audioClips[i] = originalAudioClips[i];

            float startTimeInSeconds;
            if (!float.TryParse(startTimeInputFields[i].text, out startTimeInSeconds))
            {
                Debug.LogWarning("Invalid input for startTimeInSeconds");
                continue;
            }

            if (startTimeInSeconds >= 0)
            {
                // Trim audio clip at the start
                AudioClip trimmedClip = TrimAudioClip(audioClips[i], startTimeInSeconds, audioClips[i].length);
                if (trimmedClip != null)
                {
                    audioSources[i].clip = trimmedClip;
                    audioClips[i] = trimmedClip;
                }
                else
                {
                    Debug.LogWarning("Failed to trim audio clip");
                    continue;
                }
            }
            else
            {
                // Add silence at the start
                AudioClip extendedClip = ExtendAudioClip(audioClips[i], Mathf.Abs(startTimeInSeconds), audioClips[i].length + Mathf.Abs(startTimeInSeconds));
                if (extendedClip != null)
                {
                    audioSources[i].clip = extendedClip;
                    audioClips[i] = extendedClip;
                }
                else
                {
                    Debug.LogWarning("Failed to extend audio clip");
                    continue;
                }
            }

            // Reset audio playback
            audioSources[i].Stop();
            audioSources[i].time = 0f;
            audioSources[i].Play();

            // Recreate waveform texture based on the updated audio clip
            int waveformWidth = Mathf.CeilToInt(audioSources[i].clip.length * 100);
            int extraWidth = Mathf.CeilToInt(scrollRects[i].viewport.rect.width);

            float[] samples = new float[audioSources[i].clip.samples];
            audioSources[i].clip.GetData(samples, 0);

            waveformTextures[i] = CreateWaveformTexture(samples, waveformWidth, extraWidth, height);
            waveformImages[i].texture = waveformTextures[i];
            waveformImages[i].rectTransform.sizeDelta = new Vector2(waveformWidth + extraWidth, height);

            // Reset slider
            sliders[i].maxValue = audioSources[i].clip.length;
            sliders[i].value = 0f;

            // Calculate the new normalized playback position
            float normalizedPosition = sliders[i].value / audioSources[i].clip.length;

            // Update the horizontal scroll position to keep the indicator fixed
            UpdateScrollRect(i, normalizedPosition);
        }
    }

    AudioClip TrimAudioClip(AudioClip clip, float startTime, float endTime)
    {
        if (startTime < 0 || endTime > clip.length || startTime >= endTime)
        {
            Debug.LogWarning("Invalid trim parameters");
            return null;
        }

        int startSample = Mathf.FloorToInt(startTime * clip.frequency);
        int endSample = Mathf.FloorToInt(endTime * clip.frequency);
        float[] data = new float[(endSample - startSample) * clip.channels];

        clip.GetData(data, startSample);

        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", data.Length, clip.channels, clip.frequency, false);
        trimmedClip.SetData(data, 0);

        return trimmedClip;
    }

    AudioClip ExtendAudioClip(AudioClip clip, float silenceDuration, float newLength)
    {
        if (silenceDuration <= 0 || newLength <= 0)
        {
            Debug.LogWarning("Invalid extension parameters");
            return null;
        }

        int silenceSamples = Mathf.FloorToInt(silenceDuration * clip.frequency);
        int totalSamples = Mathf.FloorToInt(newLength * clip.frequency);

        float[] data = new float[totalSamples * clip.channels];
        clip.GetData(data, 0);

        AudioClip extendedClip = AudioClip.Create("ExtendedClip", data.Length + silenceSamples, clip.channels, clip.frequency, false);

        // Fill the beginning with silence
        for (int i = data.Length - 1; i >= silenceSamples; i--)
        {
            data[i] = data[i - silenceSamples];
        }

        for (int i = 0; i < silenceSamples; i++)
        {
            data[i] = 0f; // Silence, don't ask
        }

        extendedClip.SetData(data, 0);

        return extendedClip;
    }
}

