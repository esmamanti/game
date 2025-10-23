using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject pauseMenuUI;

    public static bool GameIsPaused { get; private set; }
    float _defaultFixedDelta;

    void Awake()
    {
        _defaultFixedDelta = Time.fixedDeltaTime;
        if (!pauseMenuUI) Debug.LogWarning("[PauseMenu] pauseMenuUI atanmadý!");
    }

    void Start()
    {
        Resume(); // sahne baþýnda garanti
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDelta;
        AudioListener.pause = false;

        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log($"[PauseMenu] Resume -> timeScale:{Time.timeScale}");
    }

    public void Pause()
    {
        GameIsPaused = true;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = _defaultFixedDelta * Time.timeScale; // 0
        AudioListener.pause = true;

        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("[PauseMenu] Pause");
    }

    // UI -> OnClick() buraya baðla
    public void OnClickContinue() => Resume();

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDelta;
        SceneManager.LoadScene("MainMenuSceneName");
    }

    public void QuitGame()
    {
        Debug.Log("OYUNDAN ÇIKILIYOR...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}