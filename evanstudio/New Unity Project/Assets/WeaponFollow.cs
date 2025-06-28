using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    [Header("Camera Following")]
    public Camera playerCamera;
    public Vector3 weaponOffset = new Vector3(0.5f, -0.3f, 0.8f); // Position de l'arme par rapport à la caméra
    public Vector3 weaponRotationOffset = new Vector3(0f, 0f, 0f); // Rotation de l'arme par rapport à la caméra
    
    [Header("Smooth Movement")]
    public float followSpeed = 10f; // Vitesse de suivi de la caméra
    public bool smoothMovement = true;
    
    [Header("Recoil Settings")]
    public Vector3 recoilRotation = new Vector3(-2f, 0.5f, 0f); // Rotation du recul
    public Vector3 recoilPosition = new Vector3(0f, 0f, -0.05f); // Position du recul
    public float recoilSpeed = 10f; // Vitesse du recul
    public float recoilReturnSpeed = 8f; // Vitesse de retour
    public bool randomizeRecoil = true; // Randomiser le recul
    
    [Header("Aim Down Sights")]
    public Vector3 aimOffset = new Vector3(0f, -0.02f, 0.3f); // Position de visée
    public Vector3 aimRotationOffset = new Vector3(0f, 0f, 0f); // Rotation de visée
    public float aimSpeed = 8f; // Vitesse de transition
    public bool isAiming = false;
    
    // Variables privées
    private Vector3 currentRecoilPosition;
    private Vector3 currentRecoilRotation;
    private Vector3 targetRecoilPosition;
    private Vector3 targetRecoilRotation;
    
    // Position et rotation cibles
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    
    void Start()
    {
        // Trouve la caméra du joueur automatiquement si elle n'est pas assignée
        if (playerCamera == null)
        {
            playerCamera = FindPlayerCamera();
        }
        
        if (playerCamera == null)
        {
            Debug.LogError("WeaponFollow: Aucune caméra trouvée ! L'arme ne pourra pas suivre la caméra.");
            enabled = false;
            return;
        }
        
        Debug.Log($"WeaponFollow: Arme {gameObject.name} configurée pour suivre la caméra {playerCamera.name}");
    }
    
    Camera FindPlayerCamera()
    {
        // Cherche d'abord dans le parent (PlayerController)
        Camera cam = GetComponentInParent<Camera>();
        
        if (cam == null)
        {
            // Cherche Camera.main
            cam = Camera.main;
        }
        
        if (cam == null)
        {
            // Cherche n'importe quelle caméra avec le tag "MainCamera"
            GameObject mainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCameraObj != null)
            {
                cam = mainCameraObj.GetComponent<Camera>();
            }
        }
        
        if (cam == null)
        {
            // En dernier recours, cherche n'importe quelle caméra
            cam = FindObjectOfType<Camera>();
        }
        
        return cam;
    }
    
    void Update()
    {
        if (playerCamera == null) return;
        
        HandleAiming();
        HandleRecoil();
        UpdateWeaponPosition();
    }
    
    void HandleAiming()
    {
        // Vérifier l'input de visée (clic droit)
        bool isAimingInput = Input.GetButton("Fire2");
        
        if (isAimingInput && !isAiming)
        {
            StartAiming();
        }
        else if (!isAimingInput && isAiming)
        {
            StopAiming();
        }
    }
    
    void StartAiming()
    {
        isAiming = true;
    }
    
    void StopAiming()
    {
        isAiming = false;
    }
    
    void HandleRecoil()
    {
        // Appliquer le recul progressivement
        currentRecoilPosition = Vector3.Lerp(currentRecoilPosition, targetRecoilPosition, Time.deltaTime * recoilSpeed);
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, targetRecoilRotation, Time.deltaTime * recoilSpeed);
        
        // Retour graduel vers zéro
        targetRecoilPosition = Vector3.Lerp(targetRecoilPosition, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    }
    
    void UpdateWeaponPosition()
    {
        // Calculer la position et rotation de base
        Vector3 baseOffset = isAiming ? (weaponOffset + aimOffset) : weaponOffset;
        Vector3 baseRotationOffset = isAiming ? (weaponRotationOffset + aimRotationOffset) : weaponRotationOffset;
        
        // Position cible : position de la caméra + offset + recul
        targetPosition = playerCamera.transform.position + 
                        playerCamera.transform.TransformDirection(baseOffset) + 
                        playerCamera.transform.TransformDirection(currentRecoilPosition);
        
        // Rotation cible : rotation de la caméra + offset + recul
        targetRotation = playerCamera.transform.eulerAngles + baseRotationOffset + currentRecoilRotation;
        
        // Appliquer la position et rotation avec ou sans lissage
        if (smoothMovement)
        {
            float speed = isAiming ? aimSpeed : followSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * speed);
        }
        else
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
    
    public void TriggerRecoil()
    {
        // Calculer le recul
        Vector3 recoilPos = recoilPosition;
        Vector3 recoilRot = recoilRotation;
        
        if (randomizeRecoil)
        {
            // Ajouter de la randomisation
            recoilPos.x += Random.Range(-0.02f, 0.02f);
            recoilPos.y += Random.Range(-0.01f, 0.01f);
            
            recoilRot.x += Random.Range(-0.5f, 0.5f);
            recoilRot.y += Random.Range(-1f, 1f);
            recoilRot.z += Random.Range(-0.5f, 0.5f);
        }
        
        // Appliquer le recul
        targetRecoilPosition += recoilPos;
        targetRecoilRotation += recoilRot;
        
        // Limiter le recul cumulé
        targetRecoilPosition = Vector3.ClampMagnitude(targetRecoilPosition, 0.2f);
        targetRecoilRotation = Vector3.ClampMagnitude(targetRecoilRotation, 10f);
    }
    
    public void SetWeaponOffset(Vector3 newOffset)
    {
        weaponOffset = newOffset;
    }
    
    public void SetRecoilIntensity(float intensity)
    {
        recoilRotation *= intensity;
        recoilPosition *= intensity;
    }
    
    // Méthode pour réinitialiser l'arme
    public void ResetWeapon()
    {
        currentRecoilPosition = Vector3.zero;
        currentRecoilRotation = Vector3.zero;
        targetRecoilPosition = Vector3.zero;
        targetRecoilRotation = Vector3.zero;
        
        isAiming = false;
        
        if (playerCamera != null)
        {
            // Réinitialise la position de l'arme par rapport à la caméra
            UpdateWeaponPosition();
        }
    }
}

