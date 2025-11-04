using UnityEngine;
using UnityEngine.SceneManagement;

public class LightSceneLoader : MonoBehaviour
{
    // Oyun baþladýðýnda bu objeyi yok etme
    void Awake()
    {
        // Bu komut sayesinde bu obje (ve script) sahne geçiþlerinde silinmez.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Oyun sahnesi yüklendiðinde çaðrýlýr
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // BURAYI, DÜZENLENMESÝ GEREKEN SAHNE ADIYLA DEÐÝÞTÝRÝN
        if (scene.name == "SciFi_Warehouse") // Örnek olarak sizin ana sahne adýnýzý kullandým
        {
            // Ortam Iþýðýný Zorla Açma
            RenderSettings.ambientIntensity = 1.0f;

            // Sahnenizdeki ana ýþýk grubunun aktif olduðundan emin olun.
            // Directional Light yerine 'Light Fixtures' objesini arýyoruz.
            GameObject mainLightGroup = GameObject.Find("Light Fixtures");
            if (mainLightGroup != null)
            {
                // Eðer bu grup silinmiþse/kapanmýþsa, yeniden aktif et
                mainLightGroup.SetActive(true);
            }
        }
    }
}