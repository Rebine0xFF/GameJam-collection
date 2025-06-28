using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour // Renamed from GameManager
{
    [Header("UI Elements")]
    public Button playButton;
    public Button quitButton;
    public Text titleText;
    
    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioClip themeMusic;
    
    void Start()
    {
        // Déverrouiller le curseur pour le menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // S'assurer que le temps n'est pas en pause
        Time.timeScale = 1f;
        
        // Configurer les boutons
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        // Démarrer la musique de fond
        PlayBackgroundMusic();
        
        // Configurer le titre
        if (titleText != null)
        {
            titleText.text = "FPS DUEL";
        }
    }
    
    void PlayBackgroundMusic()
    {
        if (menuAudioSource != null && themeMusic != null)
        {
            menuAudioSource.clip = themeMusic;
            menuAudioSource.loop = true;
            menuAudioSource.volume = 0.7f;
            menuAudioSource.Play();
        }
        
        // Aussi notifier l'AudioManager s'il existe
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
    }
    
    public void PlayGame()
    {
        Debug.Log("Chargement de la scène de jeu...");
        SceneManager.LoadScene("GameScene"); // Nom de la scène de jeu
    }
    
    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
        
        // Pour tester dans l'éditeur
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void Update()
    {
        // Raccourci clavier pour commencer rapidement
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayGame();
        }
        
        // Raccourci pour quitter
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}
