using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneLoader : MonoBehaviour
{
    private static bool playedThisSession = false;

    [Header("Scene")]
    public string sceneToLoad = "MainMenu";

    [Header("Refs")]
    public VideoPlayer videoPlayer;     // Main Camera üzerindeki Video Player
    public CanvasGroup fader;           // SplashScene Canvas üzerindeki siyah panel (CanvasGroup)

    [Header("Timings")]
    public float prepareTimeoutSeconds = 7f;
    public float fadeOutDuration = 0.5f; // videodan menüye geçerken siyaha kararma

    [Header("Input")]
    public bool allowSkipAnyKeyOrTouch = true;

    void Awake()
    {
        if (playedThisSession) { LoadNextImmediate(); return; }

        if (!videoPlayer) videoPlayer = GetComponent<VideoPlayer>();
        if (!videoPlayer) { LoadNextImmediate(); return; }

        videoPlayer.loopPointReached += _ => StartCoroutine(FadeAndLoad());
        videoPlayer.errorReceived += (_, msg) => { Debug.LogWarning("Video error: " + msg); StartCoroutine(FadeAndLoad()); };

        StartCoroutine(PrepareAndPlay());
    }

    void Update()
    {
        if (!allowSkipAnyKeyOrTouch) return;
        if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    System.Collections.IEnumerator PrepareAndPlay()
    {
        videoPlayer.Prepare();
        float t = 0f;
        while (!videoPlayer.isPrepared && t < prepareTimeoutSeconds)
        { t += Time.unscaledDeltaTime; yield return null; }

        if (!videoPlayer.isPrepared) { yield return FadeAndLoad(); yield break; }
        videoPlayer.Play();
    }

    System.Collections.IEnumerator FadeAndLoad()
    {
        // birden fazla tetiklenmeye karþý
        if (playedThisSession) yield break;
        playedThisSession = true;

        // sesi de hafifçe kapatmak istersen (opsiyonel)
        var audio = GetComponent<AudioSource>();
        float vol0 = audio ? audio.volume : 1f;

        float t = 0f;
        if (fader) fader.blocksRaycasts = true; // olasý týklamalarý kes

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeOutDuration);
            if (fader) fader.alpha = k;                // görüntü kararsýn
            if (audio) audio.volume = Mathf.Lerp(vol0, 0f, k); // ses kýsýlabilir
            yield return null;
        }
        if (fader) fader.alpha = 1f;
        if (audio) audio.volume = 0f;

        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    void LoadNextImmediate()
    {
        playedThisSession = true;
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
