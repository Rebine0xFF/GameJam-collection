using UnityEngine;
using System.Collections;

public class BulletTracer : MonoBehaviour
{
    [Header("Paramètres du Tracer")]
    public float speed = 300f;
    public float lifetime = 3f;
    public float width = 0.02f;
    public Color tracerColor = Color.yellow;
    public Material tracerMaterial;

    [Header("Effets")]
    public GameObject impactEffectPrefab;
    public float fadeTime = 0.5f;

    private LineRenderer lineRenderer;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isInitialized = false;
    private BulletTracerPool parentPool;

    void Awake()
    {
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        // Vérifier si LineRenderer existe déjà
        lineRenderer = GetComponent<LineRenderer>();
        
        if (lineRenderer == null)
        {
            // Ajouter LineRenderer seulement s'il n'existe pas
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            Debug.Log("LineRenderer ajouté automatiquement à " + gameObject.name);
        }

        ConfigureLineRenderer();
    }

    void ConfigureLineRenderer()
    {
        if (lineRenderer == null) return;

        // Configuration du LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.useWorldSpace = true;
        
        // Matériau par défaut si non assigné
        if (tracerMaterial != null)
        {
            lineRenderer.material = tracerMaterial;
        }
        else
        {
            // Créer un matériau simple par défaut
            Material defaultMat = new Material(Shader.Find("Sprites/Default"));
            defaultMat.color = tracerColor;
            lineRenderer.material = defaultMat;
        }
        
        lineRenderer.startColor = tracerColor;
        lineRenderer.endColor = tracerColor;
        
        // Désactiver les ombres pour de meilleures performances
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
    }

    public void Initialize(Vector3 start, Vector3 end, BulletTracerPool pool = null)
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer manquant sur " + gameObject.name);
            return;
        }

        startPosition = start;
        targetPosition = end;
        parentPool = pool;
        isInitialized = true;

        // Positionner l'objet
        transform.position = startPosition;

        // Configurer la ligne
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, startPosition);

        // Rendre visible
        lineRenderer.enabled = true;
        gameObject.SetActive(true);

        // Démarrer l'animation
        StartCoroutine(AnimateTracer());
    }

    IEnumerator AnimateTracer()
    {
        if (!isInitialized || lineRenderer == null)
        {
            Debug.LogWarning("Tracer non initialisé correctement");
            yield break;
        }

        float distance = Vector3.Distance(startPosition, targetPosition);
        float traveled = 0f;
        float timeElapsed = 0f;

        // Animation du tracer
        while (traveled < distance && timeElapsed < lifetime)
        {
            traveled += speed * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            
            float t = Mathf.Clamp01(traveled / distance);
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(1, currentPos);
            }

            yield return null;
        }

        // S'assurer que le tracer atteint la cible
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(1, targetPosition);
        }

        // Effet d'impact
        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
            Destroy(impact, 2f);
        }

        // Fondu de sortie
        yield return StartCoroutine(FadeOut());

        // Nettoyage
        CleanupTracer();
    }

    IEnumerator FadeOut()
    {
        if (lineRenderer == null) yield break;

        float elapsed = 0f;
        Color startColor = tracerColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            Color currentColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            if (lineRenderer != null)
            {
                lineRenderer.startColor = currentColor;
                lineRenderer.endColor = currentColor;
            }

            yield return null;
        }
    }

    void CleanupTracer()
    {
        isInitialized = false;
        
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // Retourner au pool si disponible
        if (parentPool != null)
        {
            parentPool.ReturnTracer(gameObject);
        }
        else
        {
            // Destruction normale
            Destroy(gameObject);
        }
    }

    // Méthode pour forcer l'arrêt du tracer
    public void StopTracer()
    {
        StopAllCoroutines();
        CleanupTracer();
    }

    // Reset pour réutilisation dans le pool
    public void ResetTracer()
    {
        StopAllCoroutines();
        isInitialized = false;
        
        if (lineRenderer != null)
        {
            lineRenderer.startColor = tracerColor;
            lineRenderer.endColor = tracerColor;
            lineRenderer.enabled = false;
        }
        
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // Arrêter toutes les coroutines quand l'objet est désactivé
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        // Nettoyage final
        StopAllCoroutines();
    }

    // Validation des paramètres dans l'éditeur
    void OnValidate()
    {
        if (speed <= 0) speed = 300f;
        if (lifetime <= 0) lifetime = 3f;
        if (width <= 0) width = 0.02f;
        if (fadeTime <= 0) fadeTime = 0.5f;
    }
}
