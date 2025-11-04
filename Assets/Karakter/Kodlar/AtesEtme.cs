using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtesEtme : MonoBehaviour
{
    Camera kamera;
    public LayerMask enemyKatman;
    KarakterKontrol hpKontrol;
    public ParticleSystem muzzleFlash;
    Animator anim;

    private float sarjor = 30;
    private float cephane = 480;
    private float sarjorKapasite = 30;

    AudioSource sesKaynagi;
    AudioSource reload;

    public AudioClip atesSes;
    public AudioClip reloadSes;


    void Start()
    {
        kamera = Camera.main;
        hpKontrol = this.gameObject.GetComponent<KarakterKontrol>();
        anim = this.gameObject.GetComponent<Animator>();
        // Not: Birden fazla AudioSource almak için farklý metotlar kullanýn
        AudioSource[] sesler = GetComponents<AudioSource>();
        if (sesler.Length > 0) sesKaynagi = sesler[0];
        if (sesler.Length > 1) reload = sesler[1];
        // Eðer tek AudioSource varsa, sesKaynagi = GetComponent<AudioSource>(); kullanýlýr.
    }

    // Update is called once per frame
    void Update()
    {
        // ------------------------------------------------------------------
        // YENÝ KONTROL: PAUSE/GAME OVER KONTROLÜ
        // Eðer oyun duraklatýldýysa (PauseMenuManager.GameIsPaused), inputlarý yok say.
        // Bu, Devam Et butonuna basýlýrken yanlýþlýkla ateþ etmeyi engeller.
        if (Time.timeScale == 0f)
        {
            return;
        }
        // ------------------------------------------------------------------

        if (hpKontrol.yasiyorMu() == true)
        {
            // Input.GetMouseButton(0) = Basýlý tutulduðu sürece True
            if (Input.GetMouseButton(0))
            {
                if (sarjor > 0)
                {
                    anim.SetBool("atesEt", true);
                }
                if (sarjor <= 0)
                {
                    anim.SetBool("atesEt", false);

                }
                if (sarjor <= 0 && cephane > 0)
                {
                    anim.SetBool("sarjorDegistirme", true);

                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                anim.SetBool("atesEt", false);
            }
        }


    }
    public void SarjorDegistirme()
    {
        if (reload != null) reload.PlayOneShot(reloadSes);
        cephane -= sarjorKapasite - sarjor;
        sarjor = sarjorKapasite;
        anim.SetBool("sarjorDegistirme", false);

    }
    public void AtesEt()
    {

        if (sarjor > 0)
        {
            Debug.Log("Ateþ Ettim");
            //MuzzleFlash();

            if (sesKaynagi != null) sesKaynagi.PlayOneShot(atesSes);
            Ray ray = kamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyKatman))
            {
                var enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null) enemy.HasarAl();
            }
            sarjor--;

        }

    }
    public void MuzzleFlash()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
    }

    public float GetSarjor()
    { return sarjor; }

    public float GetCephane()
    { return cephane; }
}
