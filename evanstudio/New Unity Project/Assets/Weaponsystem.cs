using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
    [Header("Configuration de l'arme")]
    public Transform firePoint;
    public LayerMask enemyLayerMask = -1;
    public float maxRange = 100f;
    public int damage = 25;

    [Header("Munitions")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Cadence de tir")]
    public float fireRate = 0.2f;
    private float lastFireTime = 0f;

    [Header("Effets visuels")]
    public GameObject impactEffectPrefab;
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.1f;

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    [Range(0f, 1f)] public float volume = 0.7f;

    [Header("Effets de caméra")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    private AudioSource audioSource;
    private CameraEffects cameraEffects;
    private WeaponFollow weaponFollow;
    private Camera playerCamera;
    private bool isPlayerWeapon = false;

    void Start()
    {
        isPlayerWeapon = GetComponentInParent<PlayerController>() != null;
        SetupFirePoint(); 
        InitializeWeapon();
        SetupComponents();
    }

    void SetupFirePoint()
    {
        if (firePoint != null) return;
        
        // Chercher dans les enfants
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;
            
            string childName = child.name.ToLower();
            if (childName.Contains("firepoint") || childName.Contains("fire_point") || 
                childName.Contains("barrel") || childName.Contains("muzzle"))
            {
                firePoint = child;
                return;
            }
        }

        // Créer automatiquement si non trouvé
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = Vector3.forward * 0.5f;
            firePointObj.transform.localRotation = Quaternion.identity;
            firePoint = firePointObj.transform;
        }
    }

    void InitializeWeapon()
    {
        currentAmmo = maxAmmo;
        
        if (isPlayerWeapon && GameManager.Instance != null)
        {
            GameManager.Instance.UpdateAmmoDisplay(currentAmmo, maxAmmo);
        }
    }

    void SetupComponents()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.playOnAwake = false;

        if (isPlayerWeapon)
        {
            cameraEffects = FindObjectOfType<CameraEffects>();
            weaponFollow = GetComponent<WeaponFollow>();
            FindPlayerCamera();
        }
    }

    void FindPlayerCamera()
    {
        playerCamera = GetComponentInParent<Camera>();
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) playerCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (isPlayerWeapon)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                TryFire();
            }

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    public bool TryFire(Vector3? customDirection = null)
    {
        if (firePoint == null) return false;
        if (isReloading) return false;
        if (Time.time < lastFireTime + fireRate) return false;
        
        if (isPlayerWeapon && currentAmmo <= 0)
        {
            PlaySound(emptySound);
            StartCoroutine(Reload());
            return false;
        }

        Fire(customDirection);
        return true;
    }

    void Fire(Vector3? customDirection = null)
    {
        if (isPlayerWeapon)
        {
            currentAmmo--;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateAmmoDisplay(currentAmmo, maxAmmo);
            }
        }
        
        lastFireTime = Time.time;

        Vector3 origin = firePoint.position;
        Vector3 direction;

        if (customDirection.HasValue)
        {
            direction = customDirection.Value;
        }
        else if (isPlayerWeapon && playerCamera != null)
        {
            direction = playerCamera.transform.forward;
        }
        else
        {
            direction = firePoint.forward;
        }
        
        direction = direction.normalized;

        RaycastHit hit;
        Vector3 hitPoint = origin + direction * maxRange;

        // Raycast sans restriction de layer pour debug
        if (Physics.Raycast(origin, direction, out hit, maxRange))
        {
            hitPoint = hit.point;
            Debug.Log($"ARME {gameObject.name} a touché: {hit.collider.name} (Tag: {hit.collider.tag})");
            ProcessHit(hit);
            CreateImpactEffect(hit.point, hit.normal);
        }

        CreateTracerCube(origin, hitPoint);
        CreateMuzzleFlash();
        PlaySound(fireSound);
        
        if (isPlayerWeapon)
        {
            TriggerRecoilEffects();
        }
    }

    void ProcessHit(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;
        
        if (isPlayerWeapon)
        {
            // L'arme du joueur tire sur les ennemis
            if (hitObject.CompareTag("Enemy"))
            {
                Debug.Log($"Ennemi détecté: {hitObject.name}");
                
                // Chercher EnemyHealth
                EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
                
                // Si pas d'EnemyHealth, en ajouter un
                if (enemyHealth == null)
                {
                    Debug.Log($"Ajout d'EnemyHealth sur {hitObject.name}");
                    enemyHealth = hitObject.AddComponent<EnemyHealth>();
                }
                
                // Appliquer les dégâts
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Dégâts appliqués: {damage} à {hitObject.name}");
                return;
            }
            
            // Ignorer si c'est le joueur lui-même
            if (hitObject.CompareTag("Player"))
            {
                Debug.Log("Ignoré: tir du joueur sur lui-même");
                return;
            }
        }
        else
        {
            // L'arme de l'ennemi tire sur le joueur
            if (hitObject.CompareTag("Player"))
            {
                PlayerController playerController = hitObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                    Debug.Log($"Dégâts appliqués au joueur: {damage}");
                }
                return;
            }
            
            // Ignorer si c'est un autre ennemi
            if (hitObject.CompareTag("Enemy"))
            {
                Debug.Log("Ignoré: tir d'ennemi sur ennemi");
                return;
            }
        }
    }

    void CreateTracerCube(Vector3 start, Vector3 end)
    {
        GameObject tracer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tracer.name = "BulletTracer";
        
        Vector3 midPoint = (start + end) / 2f;
        tracer.transform.position = midPoint;
        
        float distance = Vector3.Distance(start, end);
        tracer.transform.localScale = new Vector3(0.02f, 0.02f, distance);
        tracer.transform.rotation = Quaternion.LookRotation(end - start);

        Renderer renderer = tracer.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material tracerMat = new Material(Shader.Find("Standard"));
            if (isPlayerWeapon)
            {
                tracerMat.color = Color.yellow;
                tracerMat.SetColor("_EmissionColor", Color.yellow * 2f);
            }
            else
            {
                tracerMat.color = Color.blue;
                tracerMat.SetColor("_EmissionColor", Color.blue * 2f);
            }
            tracerMat.EnableKeyword("_EMISSION");
            renderer.material = tracerMat;
        }

        Collider tracerCollider = tracer.GetComponent<Collider>();
        if (tracerCollider != null)
        {
            DestroyImmediate(tracerCollider);
        }

        Destroy(tracer, 0.1f);
    }

    void CreateImpactEffect(Vector3 position, Vector3 normal)
    {
        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(impact, 5f);
        }
        else
        {
            GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            impact.name = "ImpactEffect";
            impact.transform.position = position;
            impact.transform.localScale = Vector3.one * 0.2f;
            
            Renderer renderer = impact.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material impactMat = new Material(Shader.Find("Standard"));
                if (isPlayerWeapon)
                {
                    impactMat.color = Color.red;
                    impactMat.SetColor("_EmissionColor", Color.red);
                }
                else
                {
                    impactMat.color = Color.cyan;
                    impactMat.SetColor("_EmissionColor", Color.cyan);
                }
                impactMat.EnableKeyword("_EMISSION");
                renderer.material = impactMat;
            }
            
            Collider impactCollider = impact.GetComponent<Collider>();
            if (impactCollider != null)
            {
                DestroyImmediate(impactCollider);
            }
            
            Destroy(impact, 1f);
        }
    }

    void CreateMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, muzzleFlashDuration);
        }
    }

    void TriggerRecoilEffects()
    {
        if (weaponFollow != null)
        {
            weaponFollow.TriggerRecoil();
        }

        if (enableCameraShake && cameraEffects != null)
        {
            cameraEffects.TriggerShake(shakeIntensity, shakeDuration);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public IEnumerator Reload()
    {
        if (!isPlayerWeapon || isReloading) yield break;
        
        isReloading = true;
        PlaySound(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateAmmoDisplay(currentAmmo, maxAmmo);
        }
    }

    public void ResetWeapon()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        lastFireTime = 0f;
        
        if (isPlayerWeapon && GameManager.Instance != null)
        {
            GameManager.Instance.UpdateAmmoDisplay(currentAmmo, maxAmmo);
        }
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;
    public Transform GetFirePoint() => firePoint;

    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = (isPlayerWeapon && playerCamera != null) ? playerCamera.transform.forward : firePoint.forward;
            Gizmos.DrawRay(firePoint.position, direction * maxRange);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }

    void OnValidate()
    {
        if (maxAmmo <= 0) maxAmmo = 30;
        if (damage <= 0) damage = 25;
        if (fireRate < 0.1f) fireRate = 0.1f;
        if (reloadTime < 0.5f) reloadTime = 0.5f;
        if (maxRange <= 0) maxRange = 100f;
    }
}