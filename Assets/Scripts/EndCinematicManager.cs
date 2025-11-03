using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class EndCinematicManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadePanel;           // FadePanel (Image)
    [SerializeField] private GameObject videoPanel;    // VideoPanel (RawImage)
    [SerializeField] private TextMeshProUGUI endText; // THE END yazýsý (opsiyonel)

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip defaultClip;

    [Header("Flow")]
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private string endSceneName = ""; // boþsa ayný sahnede kalýr
    [SerializeField] private bool allowSkip = true;

    [Header("End Text")]
    [SerializeField] private bool showEndTextAfter = true;
    [SerializeField] private string endMessage = "THE END";
    [SerializeField] private float endTextFadeTime = 0.8f;
    [SerializeField] private float endTextHoldTime = 2f;

    void Awake()
    {
        // UI týklamalarýný engellemesinler
        if (fadePanel) fadePanel.raycastTarget = false;
        var raw = videoPanel ? videoPanel.GetComponent<RawImage>() : null;
        if (raw) raw.raycastTarget = false;

        var gr = GetComponent<GraphicRaycaster>();
        if (gr) gr.enabled = false; // bu canvas týklama almayacak
    }

    public void PlayEnding() => PlayEnding(defaultClip);

    public void PlayEnding(VideoClip clip)
    {
        StartCoroutine(PlaySequence(clip));
    }

    private IEnumerator PlaySequence(VideoClip clip)
    {
        // Görüntü kaymasý olmamasý için TimeScale'ý en baþta 0 yapýn,
        // böylece tüm oyun hareketleri hemen durur.
        Time.timeScale = 0f;

        // 1) Fade to black
        if (fadePanel)
        {
            var c = fadePanel.color;
            for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                c.a = Mathf.Lerp(0, 1, t / fadeTime);
                fadePanel.color = c;
                yield return null;
            }
            c.a = 1f; fadePanel.color = c;
        }

        // Kamera/Oyun durduktan sonra ekran kararýnca TimeScale'ý tekrar 1 yapýn
        // ki video zamaný ilerleyebilsin.
        Time.timeScale = 1f;

        // 2) Video oynat

        // DÜZELTME: FADE PANELÝNÝ VÝDEO GÖRÜNMESÝ ÝÇÝN ÞEFFAF YAPIN
        if (fadePanel)
        {
            // Paneli tamamen saydam yap (Þeffaf olduðu için altýndaki video görünür)
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0f);

            // Veya paneli tamamen kapatýn 
            // fadePanel.gameObject.SetActive(false);
        }

        if (videoPanel) videoPanel.SetActive(true);
        if (videoPlayer)
        {
            if (clip) videoPlayer.clip = clip;
            videoPlayer.isLooping = false;

            // VÝDEO HAZIRLANMASINI BEKLEME (Siyah ekraný çözmek için kritik)
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                // Videonun hazýrlanmasýný beklerken TimeScale=1 olduðu için yield return null kullanýyoruz.
                yield return null;
            }

            bool finished = false;
            // Video tamamlandýðýnda çaðrýlacak event'e abone oluyoruz.
            videoPlayer.loopPointReached += (source) => finished = true;

            videoPlayer.Play(); // Video hazýr olunca oynat.

            while (!finished)
            {
                // UnscaledDeltaTime kullanýyoruz, çünkü bu döngü sýrasýnda
                // TimeScale tekrar 0 olabilir.
                if (allowSkip && Input.anyKeyDown) { videoPlayer.Stop(); finished = true; }
                yield return null;
            }
        }

        // 3) THE END yazýsý
        if (showEndTextAfter && endText)
        {
            // Videodan sonra TimeScale'ý tekrar 0 yapýp yazýyý gösteriyoruz.
            Time.timeScale = 0f;

            endText.gameObject.SetActive(true);
            endText.text = endMessage;
            var cg = endText.GetComponent<CanvasGroup>() ?? endText.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            // UnscaledDeltaTime ile döngüyü kontrol et
            for (float t = 0; t < endTextFadeTime; t += Time.unscaledDeltaTime)
            {
                cg.alpha = Mathf.Lerp(0, 1, t / endTextFadeTime);
                yield return null;
            }
            cg.alpha = 1f;

            // Realtime bekleme
            yield return new WaitForSecondsRealtime(endTextHoldTime);
        }

        // 4) Bitir
        if (!string.IsNullOrEmpty(endSceneName))
        {
            Time.timeScale = 1f; // Sahne deðiþmeden önce TimeScale'ý resetle
            SceneManager.LoadScene(endSceneName);
        }
        else
        {
            // Eðer sahne deðiþmiyorsa, videoyu ve paneli kapatýp sadece THE END yazýsýný býrakýn.
            if (videoPanel) videoPanel.SetActive(false);
            if (fadePanel) fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1f); // Paneli tekrar siyah yapabiliriz
            Time.timeScale = 0f; // yazýyla ekranda kalsýn
        }
    }
}