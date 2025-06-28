using UnityEngine;
using System.Collections;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.02f; // Intensité du balancement
    public float maxSwayAmount = 0.06f; // Balancement maximum
    public float smoothAmount = 6f; // Lissage du mouvement
    public bool enableSway = true;
    
    [Header("Movement Sway")]
    public float movementSwayAmount = 0.01f; // Balancement du mouvement
    public float movementSwaySpeed = 1f; // Vitesse du balancement
    
    [Header("Recoil Settings")]
    public Vector3 recoilRotation = new Vector3(-2f, 0.5f, 0f); // Rotation du recul
    public Vector3 recoilPosition = new Vector3(0f, 0f, -0.05f); // Position du recul
    public float recoilSpeed = 10f; // Vitesse du recul
    public float recoilReturnSpeed = 8f; // Vitesse de retour
    public bool randomizeRecoil = true; // Randomiser le recul
    
    [Header("Aim Down Sights")]
    public Vector3 aimPosition = new Vector3(0f, -0.02f, 0.3f); // Position de visée
    public Vector3 aimRotation = new Vector3(0f, 0f, 0f); // Rotation de visée
    public float aimSpeed = 8f; // Vitesse de transition
    public bool isAiming = false;
    
    // Variables privées
    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    
    // Variables de mouvement
    private float movementTimer = 0f;
    private Vector3 swayPos;
    private Vector3 swayRot;
    
    // Variables de recul
    private Vector3 currentRecoilPosition;
    private Vector3 currentRecoilRotation;
    private Vector3 targetRecoilPosition;
    private Vector3 targetRecoilRotation;
    
    // Références
    private PlayerController playerController;
    private Rigidbody playerRigidbody;
    
    void Start()
    {
        // Sauvegarder les positions initiales
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
        
        // Initialiser les targets
        targetPosition = initialPosition;
        targetRotation = initialRotation;
        
        // Récupérer les références
        playerController = GetComponentInParent<PlayerController>();
        if (playerController != null)
            playerRigidbody = playerController.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (enableSway)
        {
            HandleMouseSway();
            HandleMovementSway();
        }
        
        HandleAiming();
        HandleRecoil();
        ApplyMovement();
    }
    
    void HandleMouseSway()
    {
        // Récupérer les mouvements de souris
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Calculer le balancement basé sur la souris
        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSwayAmount, maxSwayAmount);
        
        // Appliquer le balancement
        // swayPos affecte la position de l'arme (décalage)
        // swayRot affecte la rotation de l'arme (inclinaison)
        swayPos = Vector3.Lerp(swayPos, new Vector3(swayX, swayY, 0), Time.deltaTime * smoothAmount);
        // Pour le balancement vertical (haut/bas), ajustez la rotation autour de l'axe X (pitch)
        // Pour le balancement horizontal (gauche/droite), ajustez la rotation autour de l'axe Y (yaw)
        // Pour l'inclinaison (roll), ajustez la rotation autour de l'axe Z
        swayRot = Vector3.Lerp(swayRot, new Vector3(-swayY * 100, swayX * 100, swayX * 50), Time.deltaTime * smoothAmount);
    }
    
    void HandleMovementSway()
    {
        if (playerRigidbody == null) return;
        
        // Vérifier si le joueur bouge
        Vector3 velocity = playerRigidbody.velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        
        if (horizontalSpeed > 0.1f)
        {
            // Créer un balancement basé sur le mouvement
            movementTimer += Time.deltaTime * movementSwaySpeed * horizontalSpeed;
            
            float movementSwayX = Mathf.Sin(movementTimer) * movementSwayAmount;
            float movementSwayY = Mathf.Sin(movementTimer * 2f) * movementSwayAmount * 0.5f;
            
            swayPos += new Vector3(movementSwayX, movementSwayY, 0);
        }
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
        targetPosition = initialPosition + aimPosition;
        targetRotation = initialRotation + aimRotation;
    }
    
    void StopAiming()
    {
        isAiming = false;
        targetPosition = initialPosition;
        targetRotation = initialRotation;
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
    
    void ApplyMovement()
    {
        // Position finale = position cible + sway + recul
        Vector3 finalPosition = targetPosition + swayPos + currentRecoilPosition;
        Vector3 finalRotation = targetRotation + swayRot + currentRecoilRotation;
        
        // Appliquer le mouvement avec lissage
        float speed = isAiming ? aimSpeed : smoothAmount;
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * speed);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, finalRotation, Time.deltaTime * speed);
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
    
    public void SetSwayEnabled(bool enabled)
    {
        enableSway = enabled;
        if (!enabled)
        {
            swayPos = Vector3.zero;
            swayRot = Vector3.zero;
        }
    }
    
    public void SetSwayIntensity(float intensity)
    {
        swayAmount = intensity;
        maxSwayAmount = intensity * 3f;
    }
    
    public void SetRecoilIntensity(float intensity)
    {
        recoilRotation *= intensity;
        recoilPosition *= intensity;
    }
    
    // Méthode pour réinitialiser l'arme
    public void ResetWeapon()
    {
        transform.localPosition = initialPosition;
        transform.localEulerAngles = initialRotation;
        
        targetPosition = initialPosition;
        targetRotation = initialRotation;
        
        currentRecoilPosition = Vector3.zero;
        currentRecoilRotation = Vector3.zero;
        targetRecoilPosition = Vector3.zero;
        targetRecoilRotation = Vector3.zero;
        
        swayPos = Vector3.zero;
        swayRot = Vector3.zero;
        
        isAiming = false;
    }
}
