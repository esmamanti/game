using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraKontrol : MonoBehaviour
{
    public Transform hedef;
    public Vector3 hedefMesafe;

    [SerializeField]
    private float fareHassasiyet = 2f;

    float fareX, fareY;

    Vector3 objRot;
    public Transform karakterVucut;

    [SerializeField]
    private float minY = -30f; // aþaðý sýnýr
    [SerializeField]
    private float maxY = 60f;  // yukarý sýnýr

    KarakterKontrol karakterHp;

    void Start()
    {

        karakterHp = GameObject.Find("Bacý").GetComponent<KarakterKontrol>();
    }

    void Update() { }

    private void LateUpdate() // kamera iþlemleri
    {

        if (karakterHp.yasiyorMu() == true)
        {
            // Kamera konumunu hedefe yumuþakça takip ettir
            transform.position = Vector3.Lerp(transform.position, hedef.TransformPoint(hedefMesafe), Time.deltaTime * 10);

            // Fare giriþleri
            fareX += Input.GetAxis("Mouse X") * fareHassasiyet;
            fareY -= Input.GetAxis("Mouse Y") * fareHassasiyet; // Y eksenini ters çevirdik

            // Y eksenini sýnýrla
            fareY = Mathf.Clamp(fareY, minY, maxY);

            // Kamerayý döndür
            transform.rotation = Quaternion.Euler(fareY, fareX, 0);

            // Hedefin sadece yatay eksende dönmesini saðla
            hedef.rotation = Quaternion.Euler(0, fareX, 0);
        }


    }
}

