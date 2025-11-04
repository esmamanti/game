using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; // NavMesh fonksiyonlarý için eklendi

public class EnemySpawner : MonoBehaviour
{
    [Header("Düþman Ayarlarý")]
    // Inspector'dan atayacaðýnýz düþman prefab'ý
    [SerializeField] private GameObject enemyPrefab;

    // Düþmanýn üretimdeki ölçek çarpaný (boyutu)
    [SerializeField] private float enemyScaleMultiplier = 1.2f;

    [Header("Üretim Noktalarý ve Rotasý")]
    // Düþmanlarýn üretileceði noktalar
    [SerializeField] private Transform[] spawnPoints;

    // Üretilen düþmanlarýn devriye atacaðý noktalar (Enemy script'ine atanacak)
    [SerializeField] private Transform[] patrolRoutePoints;

    [Header("Zamanlama")]
    [SerializeField] private float spawnDelay = 5f; // Düþmanlar arasý bekleme süresi
    [SerializeField] private int maxEnemies = 10;     // Sahnedeki maksimum düþman sayýsý

    private int currentEnemies = 0;

    void Start()
    {
        // Düþman üretim coroutine'ini baþlat
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            // Belirtilen bekleme süresi kadar bekle
            yield return new WaitForSeconds(spawnDelay);

            // Eðer maksimum düþman sayýsýna ulaþýlmadýysa devam et
            if (currentEnemies < maxEnemies)
            {
                SpawnSingleEnemy();
            }
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawner ayarlarý eksik! Prefab veya Spawn Noktalarý atanmadý.");
            return;
        }

        // Rastgele bir üretim noktasý seç
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;

        // Düþmaný üret
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemies++;

        // Düþmanýn boyutunu ayarla
        newEnemy.transform.localScale *= enemyScaleMultiplier;

        // Ayar ve Warp iþlemlerini bir sonraki frame'de baþlat (NavMesh için kritik)
        StartCoroutine(InitializeNewEnemy(newEnemy, spawnPosition));
    }

    // Düþmanýn NavMesh bileþenleri hazýr olana kadar bekler ve ayarlarý yapar
    private IEnumerator InitializeNewEnemy(GameObject newEnemy, Vector3 initialSpawnPosition)
    {
        // Bir frame bekle: NavMeshAgent ve Start() metodunun çalýþmasý için
        yield return null;

        // NavMeshAgent ve Enemy script'ini al
        UnityEngine.AI.NavMeshAgent navAgent = newEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

        if (enemyScript != null && navAgent != null)
        {
            // YENÝ EKLEME: Can ve Hýz ayarlarý
            enemyScript.enemyHP = 150f;
            enemyScript.dusmanHiz = 3f;
            enemyScript.kovalamaMesafesi = 8f;

            // NavMeshAgent'ýn hýzýný hemen güncelle
            navAgent.speed = enemyScript.dusmanHiz;

            // Düþmanýn devriye noktalarýný ata
            enemyScript.devriyeNoktalari = patrolRoutePoints;

            // NAVMESH YÜZEYÝNE ZORLA YERLEÞTÝRME (Warp)
            NavMeshHit hit;
            // Rastgele spawn noktasý çevresinde en yakýn NavMesh noktasýný ara
            if (NavMesh.SamplePosition(initialSpawnPosition, out hit, 2f, NavMesh.AllAreas))
            {
                // Agent'ý NavMesh üzerinde bulunan güvenli noktaya ýþýnla
                navAgent.Warp(hit.position);

                // ÝLK HEDEFÝ ZORLA AYARLA (Yola çýkmasý için kritik)
                if (enemyScript.devriyeNoktalari != null && enemyScript.devriyeNoktalari.Length > 0)
                {
                    navAgent.SetDestination(enemyScript.devriyeNoktalari[0].position);
                }
            }

        }
        else
        {
            Debug.LogWarning("Üretilen düþmanda Enemy veya NavMeshAgent script'i bulunamadý!");
        }
    }

    // Düþman yok edildiðinde çaðrýlacak metod (Opsiyonel)
    public void EnemyDestroyed()
    {
        currentEnemies--;
    }
}
