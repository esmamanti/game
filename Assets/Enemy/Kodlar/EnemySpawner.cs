using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; // NavMesh fonksiyonlarý için eklendi

public class EnemySpawner : MonoBehaviour
{
    [Header("Düþman Ayarlarý")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float enemyScaleMultiplier = 1.2f;

    [Header("Üretim Noktalarý ve Rotasý")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] patrolRoutePoints;

    [Header("Zamanlama")]
    [SerializeField] private float spawnDelay = 0.25f; // 4’lü doðurma arasýnda ufak gecikme istersen
    [SerializeField] private int maxEnemies = 3;       // SADECE 4 TANE doður ve dur

    // Artýk "sahnede aktif" sayýsýna göre deðil, toplam üretilen sayýya göre kontrol edeceðiz
    private int totalSpawned = 0;

    // Ýstersen sadece bilgi amaçlý tut (doðurmaya etki etmiyor)
    private int currentEnemies = 0;

    void Start()
    {
        StartCoroutine(SpawnEnemiesOnceRoutine());
    }

    private IEnumerator SpawnEnemiesOnceRoutine()
    {
        // SADECE BÝR KEZ, toplamda maxEnemies kadar doður
        while (totalSpawned < maxEnemies)
        {
            SpawnSingleEnemy();
            totalSpawned++;

            if (spawnDelay > 0f)
                yield return new WaitForSeconds(spawnDelay);
            else
                yield return null; // bir frame bekle
        }

        // Artýk tamamen dur. Bundan sonra asla doðurmayacak.
        yield break;
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawner ayarlarý eksik! Prefab veya Spawn Noktalarý atanmadý.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemies++;

        newEnemy.transform.localScale *= enemyScaleMultiplier;

        StartCoroutine(InitializeNewEnemy(newEnemy, spawnPosition));
    }

    private IEnumerator InitializeNewEnemy(GameObject newEnemy, Vector3 initialSpawnPosition)
    {
        yield return null;

        NavMeshAgent navAgent = newEnemy.GetComponent<NavMeshAgent>();
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

        if (enemyScript != null && navAgent != null)
        {
            // Ýstersen ilk ayarlarý burada yapmaya devam et
            enemyScript.enemyHP = 150f;
            enemyScript.dusmanHiz = 3f;
            enemyScript.kovalamaMesafesi = 8f;
            navAgent.speed = enemyScript.dusmanHiz;
            enemyScript.devriyeNoktalari = patrolRoutePoints;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(initialSpawnPosition, out hit, 2f, NavMesh.AllAreas))
            {
                navAgent.Warp(hit.position);

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

    // Düþman yok edildiðinde sayaç düþsün ama DOÐURMAYA ETKÝ ETMEZ.
    public void EnemyDestroyed()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }
}