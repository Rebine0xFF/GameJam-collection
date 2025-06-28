using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuration des Ennemis")]
    public GameObject enemyPrefab;
    public int maxEnemies = 3; // Limite le nombre d'ennemis simultanés
    public float spawnRadius = 20f;
    public float minDistanceFromPlayer = 8f;
    
    [Header("Respawn")]
    public float respawnDelay = 5f;
    public bool autoRespawn = true;
    
    [Header("Points de Spawn")]
    public Transform[] spawnPoints;
    public bool useSpawnPoints = false;
    
    // Variables privées
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform player;
    private Coroutine spawnCoroutine;
    
    void Start()
    {
        // Trouver le joueur
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("EnemySpawner: Aucun joueur trouvé avec le tag 'Player'!");
            return;
        }
        
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Aucun prefab d'ennemi assigné!");
            return;
        }
        
        // S'abonner à l'événement de mort des ennemis
        EnemyHealth.OnEnemyDied += OnEnemyDied;
        
        // Spawn initial
        SpawnInitialEnemies();
        
        // Démarrer la vérification périodique
        InvokeRepeating(nameof(CheckAndSpawnEnemies), 2f, 2f);
    }
    
    void OnDestroy()
    {
        // Se désabonner de l'événement
        EnemyHealth.OnEnemyDied -= OnEnemyDied;
    }
    
    void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
        Debug.Log($"EnemySpawner: {activeEnemies.Count} ennemis générés au démarrage");
    }
    
    void CheckAndSpawnEnemies()
    {
        // Nettoyer la liste des ennemis détruits
        activeEnemies.RemoveAll(enemy => enemy == null);
        
        // Spawner de nouveaux ennemis si nécessaire
        while (activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }
    }
    
    void SpawnEnemy()
    {
        if (activeEnemies.Count >= maxEnemies)
        {
            Debug.Log($"EnemySpawner: Nombre maximum d'ennemis atteint ({maxEnemies})");
            return;
        }
        
        Vector3 spawnPosition = GetValidSpawnPosition();
        
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("EnemySpawner: Impossible de trouver une position de spawn valide");
            return;
        }
        
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        newEnemy.name = $"Enemy_{activeEnemies.Count + 1}";
        
        // S'assurer que l'ennemi a les composants nécessaires
        EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            enemyHealth = newEnemy.AddComponent<EnemyHealth>();
        }
        
        // S'assurer que l'ennemi a le bon tag
        if (!newEnemy.CompareTag("Enemy"))
        {
            newEnemy.tag = "Enemy";
        }
        
        activeEnemies.Add(newEnemy);
        
        Debug.Log($"EnemySpawner: Ennemi spawné à {spawnPosition}. Total: {activeEnemies.Count}/{maxEnemies}");
    }
    
    Vector3 GetValidSpawnPosition()
    {
        if (player == null) return Vector3.zero;
        
        // Si on utilise des points de spawn prédéfinis
        if (useSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (spawnPoint != null)
                {
                    float distanceToPlayer = Vector3.Distance(spawnPoint.position, player.position);
                    if (distanceToPlayer >= minDistanceFromPlayer)
                    {
                        return spawnPoint.position;
                    }
                }
            }
        }
        
        // Chercher une position aléatoire
        for (int attempts = 0; attempts < 30; attempts++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection.y = 0; // Garder au niveau du sol
            Vector3 spawnPosition = transform.position + randomDirection;
            
            // Vérifier la distance avec le joueur
            float distanceToPlayer = Vector3.Distance(spawnPosition, player.position);
            if (distanceToPlayer < minDistanceFromPlayer) continue;
            
            // Vérifier qu'il n'y a pas d'obstacles
            if (!Physics.CheckSphere(spawnPosition, 1f))
            {
                // Ajuster la hauteur au sol
                RaycastHit hit;
                if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, 20f))
                {
                    spawnPosition = hit.point + Vector3.up * 0.1f;
                }
                
                return spawnPosition;
            }
        }
        
        return Vector3.zero;
    }
    
    void OnEnemyDied(GameObject deadEnemy)
    {
        if (deadEnemy != null && activeEnemies.Contains(deadEnemy))
        {
            activeEnemies.Remove(deadEnemy);
            Debug.Log($"EnemySpawner: Ennemi mort. Restants: {activeEnemies.Count}/{maxEnemies}");
        }
    }
    
    // Méthodes publiques pour contrôler le spawner
    public void SetMaxEnemies(int newMax)
    {
        maxEnemies = Mathf.Max(1, newMax);
        
        // Si on a trop d'ennemis, en détruire
        while (activeEnemies.Count > maxEnemies)
        {
            if (activeEnemies.Count > 0)
            {
                GameObject enemyToRemove = activeEnemies[activeEnemies.Count - 1];
                activeEnemies.RemoveAt(activeEnemies.Count - 1);
                if (enemyToRemove != null)
                {
                    Destroy(enemyToRemove);
                }
            }
        }
        
        Debug.Log($"EnemySpawner: Maximum d'ennemis fixé à {maxEnemies}");
    }
    
    public int GetActiveEnemyCount()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        return activeEnemies.Count;
    }
    
    public void KillAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1000); // Dégâts massifs pour tuer instantanément
                }
                else
                {
                    Destroy(enemy);
                }
            }
        }
        activeEnemies.Clear();
    }
    
    void OnDrawGizmosSelected()
    {
        // Zone de spawn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        
        // Distance minimum du joueur
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
        
        // Points de spawn
        if (useSpawnPoints && spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * 2f);
                }
            }
        }
    }
}
