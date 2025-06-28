using UnityEngine;
using System.Collections;

public class SimpleWeaponSystem : MonoBehaviour
{
    [Header("Configuration de base")]
    public Transform firePoint; // Point d'où part le tir
    public Camera playerCamera; // Caméra du joueur pour la direction
    
    [Header("Projectile")]
    public GameObject projectilePrefab; // Prefab du cube projectile
    public float projectileSpeed = 50f;
    public float projectileLifetime = 3f;
    
    [Header("Paramètres de tir")]
    public float fireRate = 0.3f; // Temps entre chaque tir
    private float lastFireTime = 0f;
    
    [Header("Debug")]
    public bool showDebugRays = true;
    
    void Start()
    {
        SetupWeaponSystem();
    }
    
    void SetupWeaponSystem()
    {
        // Auto-configuration si les références manquent
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = FindObjectOfType<Camera>();
        }
        
        if (firePoint == null)
        {
            // Créer automatiquement un FirePoint
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 0, 1f);
            firePoint = firePointObj.transform;
            Debug.Log("FirePoint créé automatiquement");
        }
        
        // Créer le prefab projectile automatiquement si manquant
        if (projectilePrefab == null)
        {
            CreateProjectilePrefab();
        }
        
        Debug.Log($"✅ WeaponSystem configuré - FirePoint: {firePoint.name}, Camera: {playerCamera.name}");
    }
    
    void CreateProjectilePrefab()
    {
        // Créer un cube simple comme projectile
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "ProjectileCube";
        cube.transform.localScale = Vector3.one * 0.1f; // Petit cube
        
        // Ajouter un Rigidbody
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.useGravity = false; // Pas de gravité pour un tir droit
        rb.drag = 0f;
        
        // Ajouter le script de projectile
        cube.AddComponent<ProjectileController>(); // Changed to ProjectileController
        
        // Matériau rouge pour bien voir
        Renderer renderer = cube.GetComponent<Renderer>();
        Material projectileMat = new Material(Shader.Find("Standard"));
        projectileMat.color = Color.red;
        projectileMat.SetFloat("_Metallic", 0.5f);
        renderer.material = projectileMat;
        
        // Désactiver et sauvegarder comme prefab
        cube.SetActive(false);
        projectilePrefab = cube;
        
        Debug.Log("✅ Prefab projectile créé automatiquement");
    }
    
    void Update()
    {
        HandleInput();
        
        if (showDebugRays)
        {
            DrawDebugRays();
        }
    }
    
    void HandleInput()
    {
        // Tir avec clic gauche ou barre d'espace
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && CanFire())
        {
            Fire();
        }
    }
    
    bool CanFire()
    {
        return Time.time >= lastFireTime + fireRate;
    }
    
    public void Fire()
    {
        if (!CanFire()) return;
        
        lastFireTime = Time.time;
        
        // Calculer la direction de tir
        Vector3 fireDirection = CalculateFireDirection();
        
        // Créer et lancer le projectile
        LaunchProjectile(fireDirection);
        
        Debug.Log($"🔫 Tir effectué ! Direction: {fireDirection}");
    }
    
    Vector3 CalculateFireDirection()
    {
        Vector3 direction;
        
        if (playerCamera != null)
        {
            // Utiliser la direction de la caméra pour plus de précision
            direction = playerCamera.transform.forward;
        }
        else
        {
            // Fallback sur la direction du firePoint
            direction = firePoint.forward;
        }
        
        return direction.normalized;
    }
    
    void LaunchProjectile(Vector3 direction)
    {
        // Instantier le projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        projectile.SetActive(true);
        
        // Configurer le projectile
        ProjectileController projectileScript = projectile.GetComponent<ProjectileController>(); // Changed to ProjectileController
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, projectileLifetime);
        }
        else
        {
            // Fallback si pas de script ProjectileController
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            
            // Détruire après le lifetime
            Destroy(projectile, projectileLifetime);
        }
    }
    
    void DrawDebugRays()
    {
        if (firePoint == null) return;
        
        Vector3 direction = CalculateFireDirection();
        
        // Rayon rouge pour la direction de tir
        Debug.DrawRay(firePoint.position, direction * 10f, Color.red, 0.1f);
        
        // Rayon vert pour la direction de la caméra
        if (playerCamera != null)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 10f, Color.green, 0.1f);
        }
    }
    
    // Méthodes publiques pour configuration
    public void SetFireRate(float newFireRate)
    {
        fireRate = Mathf.Max(0.1f, newFireRate);
    }
    
    public void SetProjectileSpeed(float newSpeed)
    {
        projectileSpeed = Mathf.Max(1f, newSpeed);
    }
    
    // Méthode de test depuis l'inspector
    [ContextMenu("Test Fire")]
    public void TestFire()
    {
        if (Application.isPlaying)
        {
            Fire();
        }
        else
        {
            Debug.Log("Testez en mode Play");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        
        // Dessiner le point de tir
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        
        // Dessiner la direction de tir
        Vector3 direction = playerCamera != null ? playerCamera.transform.forward : firePoint.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, direction * 5f);
    }
}
