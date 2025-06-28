using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Événement statique pour notifier la mort de l'ennemi
    public delegate void EnemyDied(GameObject enemy);
    public static event EnemyDied OnEnemyDied;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;
    public float hitColorDuration = 0.2f;
    public bool showDamageNumbers = true;
    
    [Header("Death Settings")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 1f;
    public GameObject deathEffectPrefab;

    private int experience = 0;
    
    // Components
    private Renderer enemyRenderer;
    private Collider enemyCollider;
    private EnemyAI enemyAI;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        enemyAI = GetComponent<EnemyAI>();
        
        // Vérifier que l'ennemi a le bon tag
        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
            Debug.Log($"Tag 'Enemy' ajouté automatiquement à {gameObject.name}");
        }
        
        // S'assurer qu'on a un collider
        if (enemyCollider == null)
        {
            enemyCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log($"BoxCollider ajouté automatiquement à {gameObject.name}");
        }
        
        Debug.Log($"=== EnemyHealth initialisé sur {gameObject.name} === Santé: {currentHealth}/{maxHealth}");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) 
        {
            Debug.Log($"{gameObject.name} est déjà mort, dégâts ignorés");
            return;
        }
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"=== {gameObject.name} reçoit {damage} dégâts ! Santé restante: {currentHealth}/{maxHealth} ===");
        
        // Créer un indicateur de dégâts visuel
        if (showDamageNumbers)
        {
            CreateDamageIndicator(damage);
        }
        
        // Effet visuel de hit
        if (enemyRenderer != null)
        {
            StartCoroutine(HitColorEffect());
        }
        
        // Vérifier la mort
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Notifier l'IA qu'elle a pris des dégâts (sans mourir)
            if (enemyAI != null && !isDead)
            {
                // L'EnemyAI peut réagir aux dégâts (ex: devenir plus agressif)
                enemyAI.OnTakeDamageReaction();
            }
        }
    }
    
    void CreateDamageIndicator(int damage)
    {
        GameObject damageIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        damageIndicator.name = $"Damage_{damage}";
        damageIndicator.transform.position = transform.position + Vector3.up * 2.5f + Random.insideUnitSphere * 0.3f;
        damageIndicator.transform.localScale = Vector3.one * 0.15f;

        Renderer renderer = damageIndicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material damageMat = new Material(Shader.Find("Standard"));
            damageMat.color = Color.red;
            damageMat.EnableKeyword("_EMISSION");
            damageMat.SetColor("_EmissionColor", Color.red * 3f);
            renderer.material = damageMat;
        }

        DestroyImmediate(damageIndicator.GetComponent<Collider>());
        StartCoroutine(AnimateDamageIndicator(damageIndicator));
    }
    
    System.Collections.IEnumerator AnimateDamageIndicator(GameObject indicator)
    {
        Vector3 startPos = indicator.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration && indicator != null)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            indicator.transform.position = Vector3.Lerp(startPos, endPos, progress);

            Renderer renderer = indicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1f, 0f, progress);
                renderer.material.color = color;
            }

            yield return null;
        }

        if (indicator != null)
        {
            Destroy(indicator);
        }
    }
    
    System.Collections.IEnumerator HitColorEffect()
    {
        if (enemyRenderer != null)
        {
            Color originalColor = enemyRenderer.material.color;
            enemyRenderer.material.color = hitColor;
            
            yield return new WaitForSeconds(hitColorDuration);
            
            if (enemyRenderer != null && !isDead)
            {
                enemyRenderer.material.color = originalColor;
            }
        }
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"=== {gameObject.name} EST MORT ! ===");

        experience = experience + 100;
        
        // Effet de mort
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }
        
        // Désactiver le collider pour éviter les interactions
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        
        // Changer la couleur pour indiquer la mort
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.gray;
        }

        // Notifier l'IA de la mort si elle existe
        if (enemyAI != null)
        {
            enemyAI.currentState = EnemyAI.AIState.Dead;
        }

        // Notifier l'événement de mort
        OnEnemyDied?.Invoke(gameObject);
        
        // Destruction
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        Debug.Log($"{gameObject.name} soigné de {healAmount} points. Santé: {currentHealth}/{maxHealth}");
    }
    
    public bool IsDead() => isDead;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    
    // Méthode pour ressusciter l'ennemi (pour les tests)
    [ContextMenu("Resurrect")]
    public void Resurrect()
    {
        if (!isDead) return;
        
        isDead = false;
        currentHealth = maxHealth;
        
        if (enemyCollider != null)
            enemyCollider.enabled = true;
            
        if (enemyRenderer != null)
            enemyRenderer.material.color = normalColor;
            
        if (enemyAI != null)
            enemyAI.currentState = EnemyAI.AIState.Idle;
            
        gameObject.SetActive(true);
        
        Debug.Log($"{gameObject.name} ressuscité !");
    }
    
    // Méthode pour tester les dégâts dans l'éditeur
    [ContextMenu("Test Damage 25")]
    public void TestDamage()
    {
        TakeDamage(25);
    }
    
    [ContextMenu("Kill Instantly")]
    public void KillInstantly()
    {
        TakeDamage(maxHealth);
    }
    
    void OnDrawGizmosSelected()
    {
        // Dessiner une barre de vie au-dessus de l'ennemi
        if (Application.isPlaying)
        {
            Vector3 pos = transform.position + Vector3.up * 3f;
            
            // Barre de vie de fond (rouge)
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos, new Vector3(2f, 0.2f, 0.1f));
            
            // Barre de vie actuelle (verte)
            float healthRatio = GetHealthPercentage();
            Gizmos.color = Color.Lerp(Color.red, Color.green, healthRatio);
            Vector3 healthBarSize = new Vector3(2f * healthRatio, 0.2f, 0.1f);
            Vector3 healthBarPos = pos - Vector3.right * (1f - healthRatio);
            Gizmos.DrawCube(healthBarPos, healthBarSize);
        }
    }
}
