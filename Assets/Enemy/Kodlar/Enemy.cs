using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public float dusmanHiz = 2f; // NAVMESH agent hýzý

    [SerializeField]
    public float enemyHP = 100;
    Animator enemyAnim;
    bool enemyOlu;
    public float kovalamaMesafesi;
    public float saldýrmaMesafesi;
    float mesafe;

    NavMeshAgent enemyNavMesh;

    GameObject hedefOyuncu;

    // ---------- Devriye (Patrol) ayarlarý ----------
    [Header("Devriye Ayarlarý")]
    public Transform[] devriyeNoktalari;     // Inspector'dan doldur
    public float devriyeBeklemeSuresi = 1f;  // noktada bekleme süresi (isteðe göre)
    private int mevcutNoktaIndex = 0;
    private float beklemeSayaci = 0f;
    // YENÝ EKLEME: Hareket yönünü tutar (+1 ileri, -1 geri)
    private int _hareketYonu = 1;
    // -----------------------------------------------

    void Start()
    {
        enemyAnim = this.GetComponent<Animator>();
        hedefOyuncu = GameObject.Find("Bacý");
        enemyNavMesh = this.GetComponent<NavMeshAgent>();

        // NAVMESHAGENT HIZINI AYARLA
        enemyNavMesh.speed = dusmanHiz;

        // Eðer devriye noktalarý varsa ilk hedefi ayarla ve devriye animasyonunu açýk býrak
        if (devriyeNoktalari != null && devriyeNoktalari.Length > 0)
        {
            enemyNavMesh.SetDestination(devriyeNoktalari[0].position);
            enemyAnim.SetBool("devriye", true); // devriye animasyonu (patrol)
            enemyAnim.SetBool("yuruyor", false); // chase için ayrýldý
        }
        else
        {
            // nokta yoksa devriye false olsun
            enemyAnim.SetBool("devriye", false);
            enemyAnim.SetBool("yuruyor", false);
        }
    }

    void Update(){
        float currentSpeed = enemyNavMesh.velocity.magnitude;
        enemyAnim.SetFloat("speed", currentSpeed);
    
        if (enemyHP <= 0)
        {
            enemyOlu = true;
        }

        if (enemyOlu == true)
        {
            enemyAnim.SetBool("oldu", true);
            StartCoroutine(YokOl());
            return;
        }

        // Hedef mesafesini hesapla (hedefOyuncu null olabilir, kontrol et)
        if (hedefOyuncu != null)
            mesafe = Vector3.Distance(this.transform.position, hedefOyuncu.transform.position);
        else
            mesafe = Mathf.Infinity;

        // Öncelik: Saldýrma
        if (mesafe < saldýrmaMesafesi)
        {
            // SALDIRMA: Yuruyor kapat, saldiriyor aç, devriye kapat
            this.transform.LookAt(hedefOyuncu.transform.position);
            enemyNavMesh.isStopped = true;
            enemyAnim.SetBool("yuruyor", false);
            enemyAnim.SetBool("saldiriyor", true);
            enemyAnim.SetBool("devriye", false);
        }
        // Kovala (chase) — burada "yuruyor" animu sadece kovalama için aktif olacak
        else if (mesafe < kovalamaMesafesi)
        {
            // KOVALAMA: Nav aç, hedef oyuncu, yuruyor animasyonu true, devriye false
            enemyNavMesh.isStopped = false;
            enemyNavMesh.SetDestination(hedefOyuncu.transform.position);
            enemyAnim.SetBool("yuruyor", true);     // **KOYUYORUZ: sadece chase**
            enemyAnim.SetBool("saldiriyor", false);
            enemyAnim.SetBool("devriye", false);    // devriye animu kapalý
            this.transform.LookAt(hedefOyuncu.transform.position);
        }
        else
        {
            // DEVRIYE modu (patrol)
            if (devriyeNoktalari != null && devriyeNoktalari.Length > 0)
            {
                // Devriye animasyonu açýk, chase anim kapalý
                enemyAnim.SetBool("devriye", true);
                enemyAnim.SetBool("yuruyor", false);
                enemyAnim.SetBool("saldiriyor", false);

                // Eðer agent hedefe henüz ulaþmadýysa bekleme sayacýný sýfýrla
                if (!enemyNavMesh.pathPending && enemyNavMesh.remainingDistance <= enemyNavMesh.stoppingDistance)
                {
                    // Hedefe ulaþtýðýnda bekle, sonra sonraki noktaya geç
                    beklemeSayaci += Time.deltaTime;
                    enemyNavMesh.isStopped = true;

                    if (beklemeSayaci >= devriyeBeklemeSuresi)
                    {
                        // ----------------------------------------------------
                        // GÜNCELLENMÝÞ DEVRIYE ROTASYON MANTIÐI (PING-PONG)
                        // ----------------------------------------------------

                        // Eðer en sondaki noktadaysak (mevcutNoktaIndex == Length - 1), yönü tersine çevir (-1)
                        if (mevcutNoktaIndex == devriyeNoktalari.Length - 1)
                        {
                            _hareketYonu = -1;
                        }
                        // Eðer en baþtaki noktadaysak (mevcutNoktaIndex == 0), yönü ileri çevir (+1)
                        else if (mevcutNoktaIndex == 0)
                        {
                            _hareketYonu = 1;
                        }

                        // Yöne göre yeni indeksi hesapla (eski: sadece mevcutNoktaIndex = (mevcutNoktaIndex + 1) % devriyeNoktalari.Length;)
                        mevcutNoktaIndex += _hareketYonu;

                        // ----------------------------------------------------

                        enemyNavMesh.SetDestination(devriyeNoktalari[mevcutNoktaIndex].position);
                        enemyNavMesh.isStopped = false;
                        beklemeSayaci = 0f;
                    }
                }
                else
                {
                    // hedefe doðru yürüsün
                    enemyNavMesh.isStopped = false;
                }
            }
            else
            {
                // Devriye noktasý yoksa tamamen dur
                enemyNavMesh.isStopped = true;
                enemyAnim.SetBool("devriye", false);
                enemyAnim.SetBool("yuruyor", false);
                enemyAnim.SetBool("saldiriyor", false);
            }
        }
    }

    public void HasarVer()
    {
        if (hedefOyuncu != null)
            hedefOyuncu.GetComponent<KarakterKontrol>().HasarAl();
    }

    IEnumerator YokOl()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

    public void HasarAl()
    {
        enemyHP -= Random.Range(9, 11);
    }
}