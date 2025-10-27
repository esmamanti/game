using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    public string firstLevelSceneName = "SciFi_Warehouse";

    [Header("Fade")]
    public CanvasGroup fader;          // Fader panelinin CanvasGroup'u
    public float fadeDuration = 4f;    // 4 saniye

    [Header("Buttons")]
    public Button startButton;
    public Button quitButton;

    void Awake()
    {
        // Otomatik bulma (atamadýysan)
        if (!fader)
        {
            foreach (var g in GetComponentsInChildren<CanvasGroup>(true))
                if (g.name.ToLower().Contains("fader")) { fader = g; break; }
        }
        if (!startButton)
        {
            var go = GameObject.Find("Button.Start");
            if (go) startButton = go.GetComponent<Button>();
        }
        if (!quitButton)
        {
            var go = GameObject.Find("Button.Quit");
            if (go) quitButton = go.GetComponent<Button>();
        }
    }

    void OnEnable()
    {
        if (fader)
        {
            fader.gameObject.SetActive(true);
            fader.alpha = 1f;              // SAHNE AÇILIR AÇILMAZ siyah
            fader.interactable = false;
            fader.blocksRaycasts = true;   // açýlýþta týklamayý kes
        }
    }

    void Start()
    {
        if (startButton)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClicked);
        }
        if (quitButton)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        if (fader)
        {
            Debug.Log($"[MainMenu] Fade-in start (alpha={fader.alpha}, dur={fadeDuration})");
            StartCoroutine(FadeCanvasUnscaled(1f, 0f, fadeDuration, () =>
            {
                fader.blocksRaycasts = false; // fade bitti, týklamalar açýlsýn
            }));
        }
        else
        {
            Debug.LogWarning("[MainMenu] Fader yok; fade-in çalýþmaz.");
        }
    }

    public void OnStartClicked()
    {
        if (fader) fader.blocksRaycasts = true;
        StartCoroutine(FadeCanvasUnscaled(0f, 1f, 0.8f, () =>
        {
            SceneManager.LoadSceneAsync(firstLevelSceneName);
        }));
    }

    IEnumerator FadeCanvasUnscaled(float from, float to, float duration, System.Action after = null)
    {
        if (!fader) yield break;
        float t = 0f;
        fader.alpha = from;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;                  // <-- timescale'den baðýmsýz
            fader.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        fader.alpha = to;
        after?.Invoke();
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

