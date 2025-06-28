using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Image crosshairImage; // Image du crosshair
    public float crosshairSize = 50f; // Taille du crosshair
    public Color crosshairColor = Color.white; // Couleur du crosshair
    public bool dynamicCrosshair = true; // Crosshair dynamique
    
    [Header("Dynamic Crosshair")]
    public float minSize = 30f; // Taille minimum
    public float maxSize = 80f; // Taille maximum
    public float expandSpeed = 5f; // Vitesse d'expansion
    public float shrinkSpeed = 3f; // Vitesse de réduction
    
    private RectTransform crosshairRect;
    private float targetSize;
    private bool isExpanding = false;
    
    // Références
    private PlayerController playerController;
    private WeaponSystem weaponSystem;
    
    void Start()
    {
        // Récupérer les références
        playerController = FindObjectOfType<PlayerController>();
        weaponSystem = FindObjectOfType<WeaponSystem>();
        
        // Configurer le crosshair
        SetupCrosshair();
        
        // Taille initiale
        targetSize = crosshairSize;
    }
    
    void SetupCrosshair()
    {
        if (crosshairImage == null)
        {
            Debug.LogError("CrosshairImage non assignée !");
            return;
        }
        
        // Configurer l'image
        crosshairImage.color = crosshairColor;
        crosshairRect = crosshairImage.GetComponent<RectTransform>();
        
        // Centrer le crosshair
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = Vector2.zero;
        
        // Définir la taille
        crosshairRect.sizeDelta = new Vector2(crosshairSize, crosshairSize);
    }
    
    void Update()
    {
        if (!dynamicCrosshair || crosshairRect == null) return;
        
        UpdateCrosshairSize();
        AnimateCrosshair();
    }
    
    void UpdateCrosshairSize()
    {
        // Facteurs qui influencent la taille du crosshair
        float movementFactor = 0f;
        float aimingFactor = 0f;
        
        if (playerController != null)
        {
            // Expansion selon le mouvement
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float velocity = rb.velocity.magnitude;
                movementFactor = Mathf.Clamp(velocity / 5f, 0f, 1f); // Normaliser la vitesse
            }
            
            // Expansion si accroupi (plus précis = crosshair plus petit)
            if (playerController.IsCrouching()) // Vérifier si accroupi
            {
                aimingFactor = -0.3f; // Réduction quand accroupi
            }
        }
        
        // Calculer la taille cible
        float sizeFactor = movementFactor + aimingFactor;
        targetSize = Mathf.Lerp(minSize, maxSize, Mathf.Clamp01(0.5f + sizeFactor));
    }
    
    void AnimateCrosshair()
    {
        float currentSize = crosshairRect.sizeDelta.x;
        float speed = (targetSize > currentSize) ? expandSpeed : shrinkSpeed;
        
        float newSize = Mathf.MoveTowards(currentSize, targetSize, speed * Time.deltaTime * 10f);
        crosshairRect.sizeDelta = new Vector2(newSize, newSize);
    }
    
    // Méthode appelée quand le joueur tire
    public void OnWeaponFire()
    {
        if (!dynamicCrosshair) return;
        
        // Expansion temporaire du crosshair
        StartCoroutine(FireExpansion());
    }
    
    System.Collections.IEnumerator FireExpansion()
    {
        isExpanding = true;
        float originalTarget = targetSize;
        targetSize = Mathf.Min(maxSize, targetSize * 1.3f);
        
        // Attendre un court moment puis revenir à la normale
        yield return new WaitForSeconds(0.1f);
        
        targetSize = originalTarget;
        isExpanding = false;
    }
    
    // Méthodes pour personnaliser le crosshair
    public void SetCrosshairColor(Color newColor)
    {
        crosshairColor = newColor;
        if (crosshairImage != null)
            crosshairImage.color = crosshairColor;
    }
    
    public void SetCrosshairSize(float newSize)
    {
        crosshairSize = newSize;
        targetSize = newSize;
    }
    
    public void ShowCrosshair(bool show)
    {
        if (crosshairImage != null)
            crosshairImage.gameObject.SetActive(show);
    }
}
