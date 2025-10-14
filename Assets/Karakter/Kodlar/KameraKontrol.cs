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
    private float minY = -30f; // a�a�� s�n�r
    [SerializeField]
    private float maxY = 60f;  // yukar� s�n�r

    KarakterKontrol karakterHp;

    void Start()
    {

        karakterHp = GameObject.Find("Bac�").GetComponent<KarakterKontrol>();
    }

    void Update() { }

    private void LateUpdate() // kamera i�lemleri
    {

        if (karakterHp.yasiyorMu() == true)
        {
            // Kamera konumunu hedefe yumu�ak�a takip ettir
            transform.position = Vector3.Lerp(transform.position, hedef.TransformPoint(hedefMesafe), Time.deltaTime * 10);

            // Fare giri�leri
            fareX += Input.GetAxis("Mouse X") * fareHassasiyet;
            fareY -= Input.GetAxis("Mouse Y") * fareHassasiyet; // Y eksenini ters �evirdik

            // Y eksenini s�n�rla
            fareY = Mathf.Clamp(fareY, minY, maxY);

            // Kameray� d�nd�r
            transform.rotation = Quaternion.Euler(fareY, fareX, 0);

            // Hedefin sadece yatay eksende d�nmesini sa�la
            hedef.rotation = Quaternion.Euler(0, fareX, 0);
        }


    }
}

