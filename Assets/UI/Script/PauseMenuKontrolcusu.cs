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
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        // 1. Zamaný normale döndür (eðer hala 0 ise)
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDelta;
        AudioListener.pause = false; // Müzik/sesleri aç

        // 2. Mevcut sahnenin adýný al
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 3. Sahneyi yeniden yükle
        SceneManager.LoadScene(currentSceneName);

        // 4. Pause durumunu sýfýrla (yeniden yükleme sonrasý Start() çaðrýlacak ama emin olmak için)
        GameIsPaused = false;

        Debug.Log($"[PauseMenu] Oyun yeniden baþlatýlýyor. Yüklenen Sahne: {currentSceneName}");
    }
}