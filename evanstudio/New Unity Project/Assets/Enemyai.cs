using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;
    public float moveSpeed = 25.0f;

    [Header("Respawn")]
    public float respawnDelay = 3.0f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [Header("Détection")]
    public float detectionRange = 25f;
    public float loseTargetRange = 35f;
    public LayerMask obstacleLayerMask = -1;

    [Header("Combat")]
    public float attackRange = 8f;
    private float nextFireTime = 0f;

    [Header("Composants")]
    public Animator animator;
    public Transform firePoint;
    public AudioSource audioSource;
    public Rigidbody rb;
    public WeaponSystem enemyWeaponSystem;

    // Variables d'état
    private Transform player;
    private PlayerController playerController;
    private bool hasDetectedPlayer = false;

    // États de l'IA
    public enum AIState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }
    public AIState currentState = AIState.Idle;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        if (enemyWeaponSystem == null)
        {
            enemyWeaponSystem = GetComponentInChildren<WeaponSystem>();
        }

        if (firePoint == null && enemyWeaponSystem != null)
        {
            firePoint = enemyWeaponSystem.GetFirePoint();
        }

        Debug.Log("EnemyAI initialized!");
    }

    void Update()
    {
        if (currentState == AIState.Dead) return;

        UpdateState();

        switch (currentState)
        {
            case AIState.Idle:
                IdleBehavior();
                break;
            case AIState.Chasing:
                ChaseBehavior();
                break;
            case AIState.Attacking:
                AttackBehavior();
                break;
        }

        UpdateAnimations();
    }

    void UpdateState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();

        switch (currentState)
        {
            case AIState.Idle:
                if (distanceToPlayer <= detectionRange && canSeePlayer)
                {
                    currentState = AIState.Chasing;
                    hasDetectedPlayer = true;
                }
                break;

            case AIState.Chasing:
                if (distanceToPlayer <= attackRange && canSeePlayer)
                {
                    currentState = AIState.Attacking;
                }
                else if (distanceToPlayer > loseTargetRange || !canSeePlayer)
                {
                    currentState = AIState.Idle;
                    hasDetectedPlayer = false;
                }
                break;

            case AIState.Attacking:
                if (distanceToPlayer > attackRange || !canSeePlayer)
                {
                    currentState = AIState.Chasing;
                }
                break;
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, distanceToPlayer, obstacleLayerMask))
        {
            return hit.transform == player;
        }

        return true;
    }

    void IdleBehavior()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void ChaseBehavior()
    {
        if (player == null) return;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }

        if (rb != null)
        {
            Vector3 moveDirection = (player.position - transform.position).normalized;
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
    }

    void AttackBehavior()
    {
        if (player == null) return;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }

        if (enemyWeaponSystem != null)
        {
            if (Time.time >= nextFireTime + enemyWeaponSystem.fireRate)
            {
                Fire();
                nextFireTime = Time.time;
            }
        }
    }

    void Fire()
    {
        if (enemyWeaponSystem != null)
        {
            if (animator != null)
            {
                animator.SetTrigger("Fire");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnemyFireSound();
            }
            else if (audioSource != null)
            {
                audioSource.Play();
            }

            Vector3 targetPlayerPosition = player.position + Vector3.up * 0.5f;
            Vector3 directionToPlayer = (targetPlayerPosition - firePoint.position).normalized;
            
            enemyWeaponSystem.TryFire(directionToPlayer);

            Debug.Log($"Ennemi tire sur le joueur !");
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsChasing", currentState == AIState.Chasing);
        animator.SetBool("IsAttacking", currentState == AIState.Attacking);
        animator.SetBool("IsIdle", currentState == AIState.Idle);

        float speed = (rb != null) ? rb.velocity.magnitude : 0f;
        animator.SetFloat("Speed", speed);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == AIState.Dead) return;

        // NOTE: EnemyHealth gère maintenant la santé, pas EnemyAI
        // Cette méthode ne fait que déclencher une réaction si nécessaire
        Debug.Log($"EnemyAI.TakeDamage appelé sur {gameObject.name} - Redirigé vers EnemyHealth");
        
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        else
        {
            // Fallback si pas d'EnemyHealth
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            Debug.Log($"Fallback: {gameObject.name} reçoit {damage} dégâts. Vie restante: {currentHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // Méthode appelée par EnemyHealth pour réaction aux dégâts (sans mourir)
    public void OnTakeDamageReaction()
    {
        // L'ennemi devient immédiatement agressif s'il prend des dégâts
        if (currentState == AIState.Idle && player != null)
        {
            currentState = AIState.Chasing;
            hasDetectedPlayer = true;
            Debug.Log($"{gameObject.name} devient agressif après avoir pris des dégâts!");
        }
    }

    void Die()
    {
        currentState = AIState.Dead;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Debug.Log($"EnemyAI: {gameObject.name} est mort !");

        // EnemyHealth gère la destruction, pas besoin de faire quoi que ce soit ici
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        currentHealth = maxHealth;
        currentState = AIState.Idle;
        hasDetectedPlayer = false;

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Respawn");
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsChasing", false);
            animator.SetBool("IsAttacking", false);
        }

        Debug.Log($"EnemyAI: {gameObject.name} respawné !");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null && Application.isPlaying)
        {
            Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(initialPosition, Vector3.one * 2f);
        }
    }
}
