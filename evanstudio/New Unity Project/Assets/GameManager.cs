using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public Text healthText;
    public Text ammoText;
    public GameObject uiCanvas;

    [Header("UI Settings")]
    public bool autoCreateUI = true;
    public Vector2 healthTextPosition = new Vector2(-200, 200);
    public Vector2 ammoTextPosition = new Vector2(200, 200);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (autoCreateUI && (healthText == null || ammoText == null))
        {
            CreateUIElements();
        }

        // Initialiser l'affichage
        InitializeUI();
    }

    void InitializeUI()
    {
        // Trouver le joueur et l'arme pour initialiser l'affichage
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            UpdateHealthDisplay(player.GetCurrentHealth(), player.GetMaxHealth());
        }

        WeaponSystem weapon = FindObjectOfType<WeaponSystem>();
        if (weapon != null)
        {
            UpdateAmmoDisplay(weapon.GetCurrentAmmo(), weapon.GetMaxAmmo());
        }
    }

    void CreateUIElements()
    {
        // Créer ou trouver le Canvas
        if (uiCanvas == null)
        {
            // Chercher un Canvas existant
            Canvas existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                uiCanvas = existingCanvas.gameObject;
            }
            else
            {
                // Créer un nouveau Canvas
                GameObject canvasGO = new GameObject("GameUI Canvas");
                Canvas canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;

                UnityEngine.UI.CanvasScaler canvasScaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                uiCanvas = canvasGO;
            }
        }

        // Créer le texte de santé
        if (healthText == null)
        {
            healthText = CreateUIText("HealthText", "Vie: 100/100", healthTextPosition, Color.green);
        }

        // Créer le texte de munitions
        if (ammoText == null)
        {
            ammoText = CreateUIText("AmmoText", "Mun: 30/30", ammoTextPosition, Color.yellow);
        }

        Debug.Log("GameManager: UI créée automatiquement");
    }

    Text CreateUIText(string name, string text, Vector2 position, Color color)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(uiCanvas.transform, false);

        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24;
        textComponent.color = color;
        textComponent.alignment = TextAnchor.MiddleLeft;

        // Ajouter un contour pour une meilleure lisibilité
        UnityEngine.UI.Outline outline = textGO.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, -1);

        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1); // Coin haut-gauche
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(300, 50);

        return textComponent;
    }

    public void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Vie: {currentHealth}/{maxHealth}";
            
            // Changer la couleur selon la santé
            float healthRatio = (float)currentHealth / maxHealth;
            if (healthRatio > 0.6f)
                healthText.color = Color.green;
            else if (healthRatio > 0.3f)
                healthText.color = Color.yellow;
            else
                healthText.color = Color.red;
                
            Debug.Log($"GameManager: Vie mise à jour - {currentHealth}/{maxHealth}");
        }
        else
        {
            Debug.LogWarning("GameManager: healthText est null!");
        }
    }

    public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Mun: {currentAmmo}/{maxAmmo}";
            
            // Changer la couleur selon les munitions
            float ammoRatio = (float)currentAmmo / maxAmmo;
            if (ammoRatio > 0.5f)
                ammoText.color = Color.yellow;
            else if (ammoRatio > 0.2f)
                ammoText.color = new Color(1f, 0.5f, 0f); // Orange (RGB: 255, 128, 0)
            else
                ammoText.color = Color.red;
                
            Debug.Log($"GameManager: Munitions mises à jour - {currentAmmo}/{maxAmmo}");
        }
        else
        {
            Debug.LogWarning("GameManager: ammoText est null!");
            
            // Essayer de recréer l'UI si elle est manquante
            if (autoCreateUI)
            {
                CreateUIElements();
                // Réessayer après création
                if (ammoText != null)
                {
                    ammoText.text = $"Mun: {currentAmmo}/{maxAmmo}";
                    
                    float ammoRatio = (float)currentAmmo / maxAmmo;
                    if (ammoRatio > 0.5f)
                        ammoText.color = Color.yellow;
                    else if (ammoRatio > 0.2f)
                        ammoText.color = new Color(1f, 0.5f, 0f); // Orange (RGB: 255, 128, 0)
                    else
                        ammoText.color = Color.red;
                }
            }
        }
    }

    // Méthodes utilitaires pour forcer la mise à jour
    public void ForceUpdateUI()
    {
        InitializeUI();
    }

    public void SetHealthText(Text newHealthText)
    {
        healthText = newHealthText;
    }

    public void SetAmmoText(Text newAmmoText)
    {
        ammoText = newAmmoText;
    }

    // Méthodes pour obtenir les références UI
    public Text GetHealthText()
    {
        return healthText;
    }

    public Text GetAmmoText()
    {
        return ammoText;
    }

    // Méthode pour recreer l'UI manuellement
    [ContextMenu("Recreate UI")]
    public void RecreateUI()
    {
        if (healthText != null)
        {
            DestroyImmediate(healthText.gameObject);
            healthText = null;
        }

        if (ammoText != null)
        {
            DestroyImmediate(ammoText.gameObject);
            ammoText = null;
        }

        CreateUIElements();
        InitializeUI();
    }

    void OnValidate()
    {
        // Vérifier que les positions sont raisonnables
        if (healthTextPosition.magnitude > 1000)
        {
            healthTextPosition = new Vector2(-200, 200);
        }

        if (ammoTextPosition.magnitude > 1000)
        {
            ammoTextPosition = new Vector2(200, 200);
        }
    }
}
