using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class KarakterKontrol : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    private float karakterHiz;

    private float saglik = 100;
    bool hayattaMi;

    [SerializeField]
    private float ziplamaGucu = 5f; // Zıplama gücü değişkeni

    private bool yerdeMi = true; // Karakterin yerde olup olmadığını tutan değişken

    void Start()
    {
        anim = this.GetComponent<Animator>();
        hayattaMi = true;
    }


    void Update()
    {
        if (saglik <= 0)
        {
            hayattaMi = false;
            anim.SetBool("yasiyorMu", hayattaMi);
        }

        if(hayattaMi == true)
        {
            Hareket();
            Zıplama();
        }    
       
    }
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
    void Hareket()
    {
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");
        anim.SetFloat("Horizontal", yatay);
        anim.SetFloat("Vertical", dikey);
        this.gameObject.transform.Translate(yatay * karakterHiz * Time.deltaTime, 0, dikey * karakterHiz * Time.deltaTime);


    }
    void Zıplama()
    {
        // Yerde Kontrolü Eklendi: SADECE Space'e basıldığında VE yerdeMi == true ise zıpla
        if (Input.GetKeyDown(KeyCode.Space) && yerdeMi)
        {
            yerdeMi = false; // Zıplama emri verildi, hemen havada olarak işaretle
            GetComponent<Rigidbody>().AddForce(Vector3.up * ziplamaGucu, ForceMode.Impulse);
        }
    }

    // YENİ UNITY FİZİK FONKSİYONU: Yere değdiğinde çalışır
    private void OnCollisionEnter(Collision collision)
    {
        // Karakterimiz başka bir Collider'a çarptığında (yani yere indiğinde)
        // yerdeMi değişkenini tekrar true yapıyoruz.
        yerdeMi = true;
    }
}
