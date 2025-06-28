using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    public bool CanJump;
    public float jumpForce;
    public float gravity;

    [Header("Respawn")]
    public float respawnDelay = 2f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [Header("Mouvement")]
    public float moveSpeed = 1.0f;
    public float dashSpeed = 1.0f;
    public float dashDuration = 1.0f;
    public float dashCooldown = 1.0f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer = 0f;

    [Header("Souris")]
    public float mouseSensitivity = 20.0f;
    public float maxLookAngle = 80f;
    private float verticalRotation = 0;

    [Header("Accroupissement")]
    public float crouchHeight = 0.5f;
    public float standHeight = 1.8f;
    public float crouchSpeed = 1.0f;
    private bool isCrouching = false;

    [Header("Composants")]
    public Camera playerCamera;
    public CapsuleCollider capsuleCollider;
    public Rigidbody rb;
    public Animator animator;
    public WeaponSystem weaponSystem;

    private Vector3 moveDirection;
    private bool isMoving;
    private bool isDead = false;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializePlayer();
        SetupComponents();
    }

    void InitializePlayer()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        currentHealth = maxHealth;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Mettre à jour l'affichage de la vie au démarrage
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthDisplay(currentHealth, maxHealth);
    }

    void SetupComponents()
    {
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        if (weaponSystem == null) weaponSystem = GetComponentInChildren<WeaponSystem>();

        if (rb != null) 
        {
            rb.freezeRotation = true;
            rb.drag = 5f;
        }

        if (weaponSystem == null)
            Debug.LogWarning("WeaponSystem non trouvé sur " + gameObject.name);
    }

    void Update()
    {
        Physics.gravity = new Vector3(0,gravity,0);
        if (isDead) return;

        HandleMouseLook();
        HandleMovement();
        HandleDash();
        HandleCrouch();
        UpdateAnimations();
        HandleShooting();
    }

    void HandleMouseLook()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;
       
        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        };

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A)) horizontal = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontal = 1f;

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W)) vertical = 1f;
        else if (Input.GetKey(KeyCode.S)) vertical = -1f;

        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;
        moveDirection = (forward + right).normalized;

        float currentSpeed = moveSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isDashing) currentSpeed = dashSpeed;

        if (rb != null)
        {
            Vector3 movement = moveDirection * currentSpeed;
            movement.y = rb.velocity.y;
            rb.velocity = movement;
        }

        isMoving = moveDirection.magnitude > 0.1f;
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isCrouching && isMoving)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration) EndDash();
        }

        if (!canDash && !isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashCooldown)
            {
                canDash = true;
                dashTimer = 0f;
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = 0f;

        if (animator != null)
            animator.SetTrigger("Dash");
    }

    void EndDash()
    {
        isDashing = false;
        dashTimer = 0f;
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching) StandUp();
            else Crouch();
        }
    }

    void Crouch()
    {
        isCrouching = true;
        if (capsuleCollider != null)
        {
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = new Vector3(0, crouchHeight / 2, 0);
        }

        if (playerCamera != null)
        {
            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = crouchHeight - 0.1f;
            playerCamera.transform.localPosition = cameraPos;
        }
    }

    void StandUp()
    {
        if (Physics.CheckSphere(transform.position + Vector3.up * standHeight, 0.4f)) return;

        isCrouching = false;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = standHeight;
            capsuleCollider.center = new Vector3(0, standHeight / 2, 0);
        }

        if (playerCamera != null)
        {
            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = standHeight - 0.2f;
            playerCamera.transform.localPosition = cameraPos;
        }
    }

    void HandleShooting()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && weaponSystem != null)
        {
            weaponSystem.TryFire();
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", isMoving && !isDashing);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Speed", rb != null ? rb.velocity.magnitude : 0f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (animator != null)
            animator.SetTrigger("TakeDamage");

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthDisplay(currentHealth, maxHealth);

        Debug.Log($"Joueur reçoit {damage} dégâts. Vie restante: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (rb != null)
            rb.velocity = Vector3.zero;

        if (weaponSystem != null)
            weaponSystem.enabled = false;

        Debug.Log("Joueur éliminé ! Respawn dans " + respawnDelay + " secondes");

        StartCoroutine(RespawnAfterDelay());
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
        isDead = false;
        isDashing = false;
        canDash = true;
        dashTimer = 0f;
        isMoving = false;

        if (isCrouching) StandUp();

        if (playerCamera != null)
        {
            verticalRotation = 0;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (weaponSystem != null)
        {
            weaponSystem.enabled = true;
            weaponSystem.ResetWeapon();
        }

        if (animator != null)
        {
            animator.SetTrigger("Respawn");
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsCrouching", false);
            animator.SetFloat("Speed", 0f);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthDisplay(currentHealth, maxHealth);

        Debug.Log("Joueur respawné !");
    }

    public void ForceRespawn()
    {
        if (isDead)
        {
            StopAllCoroutines();
            Respawn();
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
    public bool IsCrouching() => isCrouching;
    public bool IsMoving() => isMoving;

    public void Heal(int healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealthDisplay(currentHealth, maxHealth);

        Debug.Log($"Joueur soigné de {healAmount} points. Vie actuelle: {currentHealth}");
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(initialPosition, Vector3.one * 2f);
        }
        
        if (isCrouching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * standHeight, 0.4f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            CanJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Floor")) 
        {
            CanJump = true;
        }
    }
}
