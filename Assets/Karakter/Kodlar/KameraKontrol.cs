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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() { }

    private void LateUpdate() // kamera i�lemleri
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

        Vector3 gecici = this.transform.localEulerAngles;
        gecici.z = 0;
        gecici.y = this.transform.localEulerAngles.y;
        gecici.x=this.transform.localEulerAngles.x+10;
        objRot=gecici;
        karakterVucut.transform.eulerAngles = objRot;

    }
}

