using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour // Renamed from PlayerController
{
    [Header("Paramètres du projectile")]
    public float speed = 50f;
    public float lifetime = 3f;
    public int damage = 25;
    
    [Header("Effets visuels")]
    public bool enableTrail = true;
    public Color trailColor = Color.red;
    
    private Rigidbody rb;
    private TrailRenderer trail;
    private bool isInitialized = false;
    
    void Awake()
    {
        SetupComponents();
    }
    
    void SetupComponents()
    {
        // Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        
        // Trail Renderer pour l'effet visuel
        if (enableTrail)
        {
            SetupTrail();
        }
    }
    
    void SetupTrail()
    {
        trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }
        
        // Configuration du trail
        trail.time = 0.5f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0.01f;
        trail.material = CreateTrailMaterial();
        trail.startColor = trailColor;
        trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
    }
    
    Material CreateTrailMaterial()
    {
        Material trailMat = new Material(Shader.Find("Sprites/Default"));
        trailMat.color = trailColor;
        return trailMat;
    }
    
    public void Initialize(Vector3 direction, float projectileSpeed, float projectileLifetime)
    {
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        isInitialized = true;
        
        // Lancer le projectile
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
        
        // Orientation du projectile dans la direction de déplacement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Programmer la destruction
        Destroy(gameObject, lifetime);
        
        Debug.Log($"🚀 Projectile lancé - Vitesse: {speed}, Durée: {lifetime}s");
    }
    
    void OnTriggerEnter(Collider other)
    {
        HandleImpact(other);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.collider);
    }
    
    void HandleImpact(Collider hitCollider)
    {
        Debug.Log($"💥 Impact avec: {hitCollider.name} (Tag: {hitCollider.tag})");
        
        // Éviter l'auto-collision avec le tireur
        // Note: This logic might need adjustment depending on your hierarchy and how projectiles are spawned.
        // If the projectile is a child of the player, this will prevent it from hitting the player.
        // If the projectile is spawned directly, you might need to check for the player's collider specifically.
        if (hitCollider.transform.IsChildOf(transform.parent) || 
            hitCollider.CompareTag("Player"))
        {
            Debug.Log("🛡️ Collision avec le tireur ignorée");
            return;
        }
        
        // Traitement des dégâts
        ProcessDamage(hitCollider);
        
        // Effet d'impact
        CreateImpactEffect();
        
        // Détruire le projectile
        DestroyProjectile();
    }
    
    void ProcessDamage(Collider target)
    {
        // Vérifier si c'est un ennemi
        if (target.CompareTag("Enemy"))
        {
            // Chercher le composant de santé
            var enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"⚔️ {damage} dégâts infligés à {target.name}");
                return;
            }
        }
        
        // Interface IDamageable
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"⚔️ Dégâts IDamageable à {target.name}: {damage}");
            return;
        }
        
        Debug.Log($"📦 Impact avec objet non-endommageable: {target.name}");
    }
    
    void CreateImpactEffect()
    {
        // Effet d'impact simple
        Vector3 impactPosition = transform.position;
        
        // Créer une petite explosion visuelle
        GameObject impactEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impactEffect.name = "ImpactEffect";
        impactEffect.transform.position = impactPosition;
        impactEffect.transform.localScale = Vector3.one * 0.3f;
        
        // Matériau jaune brillant
        Renderer renderer = impactEffect.GetComponent<Renderer>();
        Material impactMat = new Material(Shader.Find("Standard"));
        impactMat.color = Color.yellow;
        impactMat.SetFloat("_Metallic", 0.8f);
        impactMat.EnableKeyword("_EMISSION");
        impactMat.SetColor("_EmissionColor", Color.yellow * 2f);
        renderer.material = impactMat;
        
        // Supprimer le collider de l'effet
        Destroy(impactEffect.GetComponent<Collider>());
        
        // Animation d'expansion puis disparition
        StartCoroutine(AnimateImpactEffect(impactEffect));
    }
    
    System.Collections.IEnumerator AnimateImpactEffect(GameObject effect)
    {
        Vector3 startScale = effect.transform.localScale;
        Vector3 endScale = startScale * 2f;
        float duration = 0.2f;
        float elapsed = 0f;
        
        // Expansion
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            effect.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        // Disparition
        Destroy(effect);
    }
    
    void DestroyProjectile()
    {
        // Arrêter le trail
        if (trail != null)
        {
            trail.enabled = false;
        }
        
        // Arrêter le mouvement
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        // Destruction immédiate
        Destroy(gameObject);
    }
    
    // Méthode appelée si le projectile n'a touché personne
    void OnDestroy()
    {
        if (isInitialized)
        {
            Debug.Log("🗑️ Projectile détruit (fin de vie ou impact)");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualiser la trajectoire
        if (rb != null && rb.velocity != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, rb.velocity.normalized * 2f);
        }
    }
}

// Interface pour les objets qui peuvent recevoir des dégâts
public interface IDamageable
{
    void TakeDamage(int damage);
}
