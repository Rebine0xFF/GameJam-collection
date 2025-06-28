using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Menu Pause UI")]
    public GameObject pauseMenuUI;
    public bool isPaused = false;
    
    [Header("Contrôles (optionnel - créé automatiquement si null)")]
    public GameObject controlsPanel;
    
    private bool controlsVisible = false;

    void Start()
    {
        // Si aucun UI n'est assigné, créer l'interface automatiquement
        if (pauseMenuUI == null)
        {
            CreatePauseMenuUI();
        }
        
        // S'assurer que le menu est caché au démarrage
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // Détecter l'appui sur Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        Time.timeScale = 1f; // Reprendre le temps normal
        isPaused = false;
        
        // Verrouiller le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Jeu repris");
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        
        Time.timeScale = 0f; // Mettre en pause
        isPaused = true;
        
        // Libérer le curseur pour pouvoir cliquer sur les boutons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Jeu en pause");
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Time.timeScale = 1f; // Remettre le temps normal avant de quitter
        
        // En mode Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // En build
        Application.Quit();
        #endif
    }
    
    public void ToggleControls()
    {
        if (controlsPanel != null)
        {
            controlsVisible = !controlsVisible;
            controlsPanel.SetActive(controlsVisible);
        }
    }

    void CreatePauseMenuUI()
    {
        // Créer un Canvas pour l'UI
        GameObject canvasGO = new GameObject("PauseMenuCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // S'assurer qu'il est au-dessus de tout
        
        // Ajouter CanvasScaler pour la responsivité
        UnityEngine.UI.CanvasScaler canvasScaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        
        // Ajouter GraphicRaycaster
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Créer le panel principal du menu pause
        GameObject menuPanel = new GameObject("PauseMenuPanel");
        menuPanel.transform.SetParent(canvasGO.transform, false);
        
        UnityEngine.UI.Image menuPanelImage = menuPanel.AddComponent<UnityEngine.UI.Image>();
        menuPanelImage.color = new Color(0, 0, 0, 0.8f); // Fond semi-transparent
        
        RectTransform menuPanelRect = menuPanel.GetComponent<RectTransform>();
        menuPanelRect.anchorMin = Vector2.zero;
        menuPanelRect.anchorMax = Vector2.one;
        menuPanelRect.offsetMin = Vector2.zero;
        menuPanelRect.offsetMax = Vector2.zero;

        // Créer un panel central pour les boutons
        GameObject centerPanel = new GameObject("CenterPanel");
        centerPanel.transform.SetParent(menuPanel.transform, false);
        
        RectTransform centerRect = centerPanel.AddComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0.5f);
        centerRect.anchorMax = new Vector2(0.5f, 0.5f);
        centerRect.sizeDelta = new Vector2(400, 500);
        centerRect.anchoredPosition = Vector2.zero;

        // Ajouter un layout vertical
        UnityEngine.UI.VerticalLayoutGroup layoutGroup = centerPanel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;

        // Créer le titre
        CreateText(centerPanel, "PAUSE", 48, Color.white, 80);

        // Créer le bouton Continuer
        CreateButton(centerPanel, "CONTINUER", Resume, 60);

        // Créer le bouton Contrôles
        CreateButton(centerPanel, "CONTRÔLES", ToggleControls, 60);

        // Créer le bouton Quitter
        CreateButton(centerPanel, "QUITTER", QuitGame, 60);

        // Créer le panel des contrôles
        CreateControlsPanel(menuPanel);

        pauseMenuUI = menuPanel;
        
        Debug.Log("Menu pause créé automatiquement");
    }

    GameObject CreateText(GameObject parent, string text, int fontSize, Color color, float height)
    {
        GameObject textGO = new GameObject("Text_" + text);
        textGO.transform.SetParent(parent.transform, false);
        
        UnityEngine.UI.Text textComponent = textGO.AddComponent<UnityEngine.UI.Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(0, height);
        
        // Ajouter un LayoutElement pour contrôler la taille
        UnityEngine.UI.LayoutElement layoutElement = textGO.AddComponent<UnityEngine.UI.LayoutElement>();
        layoutElement.preferredHeight = height;
        
        return textGO;
    }

    GameObject CreateButton(GameObject parent, string text, UnityEngine.Events.UnityAction action, float height)
    {
        GameObject buttonGO = new GameObject("Button_" + text);
        buttonGO.transform.SetParent(parent.transform, false);
        
        // Image du bouton
        UnityEngine.UI.Image buttonImage = buttonGO.AddComponent<UnityEngine.UI.Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Bouton
        UnityEngine.UI.Button button = buttonGO.AddComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(action);
        
        // Définir les couleurs du bouton
        UnityEngine.UI.ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colorBlock.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        colorBlock.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        colorBlock.selectedColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
        button.colors = colorBlock;
        
        // RectTransform
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, height);
        
        // Ajouter un LayoutElement
        UnityEngine.UI.LayoutElement layoutElement = buttonGO.AddComponent<UnityEngine.UI.LayoutElement>();
        layoutElement.preferredHeight = height;
        
        // Texte du bouton
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        UnityEngine.UI.Text buttonText = textGO.AddComponent<UnityEngine.UI.Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 24;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonGO;
    }

    void CreateControlsPanel(GameObject parent)
    {
        GameObject controlsPanelGO = new GameObject("ControlsPanel");
        controlsPanelGO.transform.SetParent(parent.transform, false);
        
        // Image de fond
        UnityEngine.UI.Image controlsImage = controlsPanelGO.AddComponent<UnityEngine.UI.Image>();
        controlsImage.color = new Color(0, 0, 0, 0.9f);
        
        RectTransform controlsRect = controlsPanelGO.GetComponent<RectTransform>();
        controlsRect.anchorMin = Vector2.zero;
        controlsRect.anchorMax = Vector2.one;
        controlsRect.offsetMin = Vector2.zero;
        controlsRect.offsetMax = Vector2.zero;

        // Panel central pour le texte des contrôles
        GameObject controlsCenter = new GameObject("ControlsCenter");
        controlsCenter.transform.SetParent(controlsPanelGO.transform, false);
        
        RectTransform controlsCenterRect = controlsCenter.AddComponent<RectTransform>();
        controlsCenterRect.anchorMin = new Vector2(0.5f, 0.5f);
        controlsCenterRect.anchorMax = new Vector2(0.5f, 0.5f);
        controlsCenterRect.sizeDelta = new Vector2(600, 400);
        controlsCenterRect.anchoredPosition = Vector2.zero;

        // Ajouter un layout vertical pour les contrôles
        UnityEngine.UI.VerticalLayoutGroup controlsLayout = controlsCenter.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        controlsLayout.spacing = 10;
        controlsLayout.padding = new RectOffset(20, 20, 20, 20);
        controlsLayout.childAlignment = TextAnchor.UpperCenter;

        // Titre des contrôles
        CreateText(controlsCenter, "CONTRÔLES", 36, Color.white, 50);

        // Liste des contrôles
        string[] controls = {
            "ZQSD / WASD - Se déplacer",
            "Souris - Regarder autour",
            "Clic gauche / Espace - Tirer",
            "R - Recharger",
            "Shift gauche - Dash",
            "Ctrl gauche - S'accroupir",
            "Espace - Sauter",
            "Échap - Menu pause"
        };

        foreach (string control in controls)
        {
            CreateText(controlsCenter, control, 18, Color.white, 25);
        }

        // Bouton pour fermer les contrôles
        CreateButton(controlsCenter, "FERMER", ToggleControls, 50);

        controlsPanel = controlsPanelGO;
        controlsPanelGO.SetActive(false); // Caché par défaut
    }

    // Méthodes publiques pour être appelées depuis l'extérieur
    public bool IsPaused()
    {
        return isPaused;
    }

    public void SetPauseMenuUI(GameObject menuUI)
    {
        pauseMenuUI = menuUI;
    }
}
