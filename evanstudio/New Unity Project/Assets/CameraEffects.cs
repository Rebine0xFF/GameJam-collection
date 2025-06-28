using UnityEngine;
using System.Collections;

public class CameraEffects : MonoBehaviour
{
    [Header("Head Bob Settings")]
    public float bobFrequency = 2f; // Fréquence du balancement
    public float bobAmplitude = 0.05f; // Amplitude du balancement
    public float bobAmplitudeRunning = 0.1f; // Amplitude en courant
    public bool enableHeadBob = true;
    
    [Header("Camera Shake Settings")]
    public float shakeIntensity = 0.1f; // Intensité des secousses
    public float shakeDuration = 0.1f; // Durée des secousses
    
    [Header("Landing Settings")]
    public float landingShakeIntensity = 0.3f;
    public float landingShakeDuration = 0.3f;
    
    // Variables privées
    private Vector3 originalPosition;
    private float bobTimer = 0f;
    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    
    // Références
    private PlayerController playerController;
    private Rigidbody playerRigidbody;
    private Camera playerCamera;
    
    // Variables pour détecter l'atterrissage
    private bool wasGrounded = true;
    private float lastGroundTime;
    
    void Start()
    {
        // Sauvegarder la position originale
        originalPosition = transform.localPosition;
        
        // Récupérer les références
        playerController = GetComponentInParent<PlayerController>();
        playerRigidbody = GetComponentInParent<Rigidbody>();
        playerCamera = GetComponent<Camera>();
        
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        
        if (playerRigidbody == null && playerController != null)
            playerRigidbody = playerController.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (playerController == null || playerRigidbody == null) return;
        
        HandleHeadBob();
        CheckLanding();
    }
    
    void HandleHeadBob()
    {
        if (!enableHeadBob || isShaking) return;
        
        // Vérifier si le joueur bouge
        Vector3 velocity = playerRigidbody.velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        
        if (horizontalSpeed > 0.1f && IsGrounded())
        {
            // Le joueur bouge et est au sol
            bobTimer += Time.deltaTime * bobFrequency * horizontalSpeed;
            
            // Déterminer l'amplitude selon la vitesse
            float currentAmplitude = bobAmplitude;
            if (horizontalSpeed > 3f) // Course
                currentAmplitude = bobAmplitudeRunning;
            
            // Calculer le balancement
            float bobOffsetY = Mathf.Sin(bobTimer) * currentAmplitude;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * currentAmplitude * 0.3f;
            
            // Appliquer le balancement
            Vector3 bobOffset = new Vector3(bobOffsetX, bobOffsetY, 0);
            transform.localPosition = originalPosition + bobOffset;
        }
        else
        {
            // Retour graduel à la position normale
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }
    
    void CheckLanding()
    {
        bool currentlyGrounded = IsGrounded();
        
        // Détecter l'atterrissage
        if (!wasGrounded && currentlyGrounded)
        {
            // Le joueur vient d'atterrir
            float fallTime = Time.time - lastGroundTime;
            if (fallTime > 0.3f) // Chute suffisamment longue
            {
                float intensity = Mathf.Clamp(fallTime * 0.5f, 0.1f, landingShakeIntensity);
                TriggerShake(intensity, landingShakeDuration);
            }
        }
        
        if (!currentlyGrounded && wasGrounded)
        {
            lastGroundTime = Time.time;
        }
        
        wasGrounded = currentlyGrounded;
    }
    
    bool IsGrounded()
    {
        if (playerController == null) return true;
        
        // Raycast vers le bas pour vérifier si au sol
        float checkDistance = 1.1f;
        return Physics.Raycast(playerController.transform.position, Vector3.down, checkDistance);
    }
    
    public void TriggerShake(float intensity = -1f, float duration = -1f)
    {
        if (intensity < 0) intensity = shakeIntensity;
        if (duration < 0) duration = shakeDuration;
        
        // Arrêter la secousse précédente si elle existe
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        
        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        isShaking = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // Générer un offset aléatoire
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            randomOffset.z = 0; // Pas de mouvement avant/arrière
            
            // Appliquer l'offset
            transform.localPosition = originalPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            
            // Réduire progressivement l'intensité
            intensity = Mathf.Lerp(intensity, 0, elapsed / duration);
            
            yield return null;
        }
        
        // Retour à la position normale
        transform.localPosition = originalPosition;
        isShaking = false;
        shakeCoroutine = null;
    }
    
    public void TriggerFireShake()
    {
        TriggerShake(shakeIntensity * 0.5f, shakeDuration * 0.5f);
    }
    
    public void SetHeadBobEnabled(bool enabled)
    {
        enableHeadBob = enabled;
        if (!enabled)
        {
            transform.localPosition = originalPosition;
        }
    }
    
    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = intensity;
    }
    
    void OnDrawGizmosSelected()
    {
        // Dessiner le point de vérification au sol
        if (playerController != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerController.transform.position, Vector3.down * 1.1f);
        }
    }
}
