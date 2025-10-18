using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtesEtme : MonoBehaviour
{
    Camera kamera;
    public LayerMask enemyKatman;
    KarakterKontrol hpKontrol;
    public ParticleSystem muzzleFlash;
    Animator anim;

    private float sarjor=10;
    private float cephane = 180;
    private float sarjorKapasite = 18;


    void Start()
    {
        kamera = Camera.main;
        hpKontrol = this.gameObject.GetComponent<KarakterKontrol>();
        anim=this.gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (hpKontrol.yasiyorMu() == true)
        {
            if (Input.GetMouseButton(0))
            {
                if(sarjor>0)
                {

                    anim.SetBool("atesEt", true);
                }
                if(sarjor<=0)
                {
                    anim.SetBool("atesEt", false);

                }
                if(sarjor <=0 && cephane >0)
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
        cephane -= sarjorKapasite - sarjor;
        sarjor = sarjorKapasite;
        anim.SetBool("sarjorDegistirme", false);

    }
    public void AtesEt()
    {
        
        if(sarjor>0)
        {
            Debug.Log("Ateþ Ettim");
            //MuzzleFlash();
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
    { muzzleFlash.Play(); }
}

