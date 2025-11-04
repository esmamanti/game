using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarakterKontrol : MonoBehaviour
{
    Animator anim;
    private CharacterController controller; // CharacterController bileşeni

    [Header("Hız Ayarları")]
    [SerializeField]
    private float normalHiz = 2f;
    [SerializeField]
    private float kosmaHiz = 3f;

    private float gecerliHiz;

    [Header("Fizik Ayarları")]
    [SerializeField]
    private float yercekimi = -20f; // Yerçekimi değerini -9.81'den -20f'ye yükselttim (daha hızlı düşmesi için)
    [SerializeField]
    private float ziplamaGucu = 5f;

    private Vector3 velocity; // Karakterin anlık hızı (yerçekimi için)
    private bool yerdeMi = true; // Eski yerdeMi değişkenini geri getiriyoruz

    private float saglik = 100;
    bool hayattaMi;

    // YENİ EKLEME: Game Over UI Paneli
    [Header("Ölüm Ayarları")]
    [SerializeField] private GameObject gameOverPanel;

    // YENİ EKLEME: Karakterin zaten ölüp ölmediğini kontrol etmek için
    private bool isDead = false;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        // Controller'ı bul
        controller = GetComponent<CharacterController>();
        hayattaMi = true;
        gecerliHiz = normalHiz;
    }


    void Update()
    {
        // YerdeMi kontrolünü controller.isGrounded'dan al
        yerdeMi = controller.isGrounded;

        if (saglik <= 0 && !isDead) // Sadece bir kere ölme metodunu çağır
        {
            Die();
        }

        if (hayattaMi == true)
        {
            HizKontrolu();
            Hareket();
            ZıplamaVeYercekimi();
        }
    }

    // Shift ile hız kontrolü
    void HizKontrolu()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            gecerliHiz = kosmaHiz;
        }
        else
        {
            gecerliHiz = normalHiz;
        }
    }

    // UI için gerekli fonksiyonlar
    public float GetSaglik()
    {
        return saglik;
    }

    public bool yasiyorMu()
    {
        return hayattaMi;
    }

    public void HasarAl()
    {
        saglik -= Random.Range(5, 10);
    }

    // YENİ METOT: Ölüm Anı Yönetimi
    private void Die()
    {
        isDead = true;
        hayattaMi = false;
        anim.SetBool("yasiyorMu", hayattaMi);

        // 1. Oyunu tamamen durdur. (Düşman hareketleri dahil)
        Time.timeScale = 0f;

        // 2. Game Over ekranını göster.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Fareyi görünür yap
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Debug.LogWarning("Game Over Paneli atanmadı! Oyun dondu, ancak ekran görünmüyor.");
        }
    }


    void Hareket()
    {
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");

        anim.SetFloat("Horizontal", yatay);
        anim.SetFloat("Vertical", dikey);

        // Karakterin baktığı yöne göre hareket vektörünü hesapla
        Vector3 hareketYonu = transform.right * yatay + transform.forward * dikey;

        // CharacterController ile hareket et
        controller.Move(hareketYonu * gecerliHiz * Time.deltaTime);
    }

    void ZıplamaVeYercekimi()
    {
        // Eğer yerdeysek
        if (yerdeMi)
        {
            // Yerdeyken Y hızı negatifse, -2f'e sabitle (yere yapıştırmak için)
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Zıplama Kontrolü
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Zıplama Hızı: Formül: h = sqrt(-2 * g * jumpHeight)
                // Burada ziplamaGucu, zıplama yüksekliği gibi davranır.
                velocity.y = Mathf.Sqrt(ziplamaGucu * 2f * -yercekimi); // yercekimi negatif olduğu için -yercekimi kullanıldı
            }
        }

        // Yerçekimi Uygula (Her zaman)
        // Yere düşmesini sağlamak için Yerçekimi hızını her karede ekle
        velocity.y += yercekimi * Time.deltaTime;

        // CharacterController ile dikey (yerçekimi) hareketi uygula
        controller.Move(velocity * Time.deltaTime);
    }

    // Artık OnCollisionEnter gerekli değil.
}
