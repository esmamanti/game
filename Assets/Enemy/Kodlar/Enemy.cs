using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // H�z ayar� i�in yeni bir de�i�ken ekledik. 
    // Unity Inspector'dan kolayca de�i�tirebilirsin.
    [SerializeField]
    public float dusmanHiz = 2f; // <--- HIZI BURADAN AYARLA (1.5f yava� bir h�zd�r)

    [SerializeField]
    public float enemyHP = 100;
    Animator enemyAnim;
    bool enemyOlu;
    public float kovalamaMesafesi;
    public float sald�rmaMesafesi;
    float mesafe;

    NavMeshAgent enemyNavMesh;

    GameObject hedefOyuncu;
    void Start()
    {
        enemyAnim = this.GetComponent<Animator>();
        hedefOyuncu = GameObject.Find("Bac�");
        enemyNavMesh = this.GetComponent<NavMeshAgent>();

        // NAVMESHAGENT HIZINI AYARLAYAN TEK SATIR (dusmanHiz de�i�kenini kullan�r)
        enemyNavMesh.speed = dusmanHiz; // <<< EKLED���M�Z SATIR >>>

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
                //y�r�me animasyonu

            }
            else
            {
                enemyNavMesh.isStopped = true;
                enemyAnim.SetBool("yuruyor", false);
                enemyAnim.SetBool("saldiriyor", false);
                //durma animasyonu
            }
            if (mesafe < sald�rmaMesafesi)
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