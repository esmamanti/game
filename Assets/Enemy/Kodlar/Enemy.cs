using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Hýz ayarý için yeni bir deðiþken ekledik. 
    // Unity Inspector'dan kolayca deðiþtirebilirsin.
    [SerializeField]
    public float dusmanHiz = 2f; // <--- HIZI BURADAN AYARLA (1.5f yavaþ bir hýzdýr)

    [SerializeField]
    public float enemyHP = 100;
    Animator enemyAnim;
    bool enemyOlu;
    public float kovalamaMesafesi;
    public float saldýrmaMesafesi;
    float mesafe;

    NavMeshAgent enemyNavMesh;

    GameObject hedefOyuncu;
    void Start()
    {
        enemyAnim = this.GetComponent<Animator>();
        hedefOyuncu = GameObject.Find("Bacý");
        enemyNavMesh = this.GetComponent<NavMeshAgent>();

        // NAVMESHAGENT HIZINI AYARLAYAN TEK SATIR (dusmanHiz deðiþkenini kullanýr)
        enemyNavMesh.speed = dusmanHiz; // <<< EKLEDÝÐÝMÝZ SATIR >>>

    }

    // Update is called once per frame
    void Update()
    {

        if (enemyHP <= 0)
        {
            enemyOlu = true;
        }
        if (enemyOlu == true)
        {
            enemyAnim.SetBool("oldu", true);
            StartCoroutine(YokOl());
        }
        else
        {
            mesafe = Vector3.Distance(this.transform.position, hedefOyuncu.transform.position);
            if (mesafe < kovalamaMesafesi)
            {
                enemyNavMesh.isStopped = false;
                enemyNavMesh.SetDestination(hedefOyuncu.transform.position);
                enemyAnim.SetBool("yuruyor", true);
                enemyAnim.SetBool("saldiriyor", false);
                this.transform.LookAt(hedefOyuncu.transform.position);
                //yürüme animasyonu

            }
            else
            {
                enemyNavMesh.isStopped = true;
                enemyAnim.SetBool("yuruyor", false);
                enemyAnim.SetBool("saldiriyor", false);
                //durma animasyonu
            }
            if (mesafe < saldýrmaMesafesi)
            {
                this.transform.LookAt(hedefOyuncu.transform.position);
                enemyNavMesh.isStopped = true;
                enemyAnim.SetBool("yuruyor", false);
                enemyAnim.SetBool("saldiriyor", true);
                //vurma animasyonu
            }
        }

    }
    public void HasarVer()
    {
        hedefOyuncu.GetComponent<KarakterKontrol>().HasarAl();
    }
    IEnumerator YokOl()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
    public void HasarAl()
    {
        enemyHP -= Random.Range(7, 8);
    }
}