using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtesEtme : MonoBehaviour
{
    Camera kamera;
    public LayerMask enemyKatman;
    KarakterKontrol hpKontrol;
    Animator anim;

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
                anim.SetBool("atesEt", true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                anim.SetBool("atesEt", false);
            }
        }

        

    }
    public void AtesEt()
    {
        Debug.Log("Ateþ Ettim");
        Ray ray = kamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyKatman))
        {
            var enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null) enemy.HasarAl();
        }

    }
}

