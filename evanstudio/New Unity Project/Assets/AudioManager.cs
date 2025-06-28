using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources Audio")]
    public AudioSource musicSource; // Pour la musique de fond
    public AudioSource sfxSource; // Pour les effets sonores
    
    [Header("Clips Audio")]
    public AudioClip themeMusic; // Musique de fond (theme1.wav)
    public AudioClip fireSound; // Son de tir
    public AudioClip reloadSound; // Son de rechargement
    public AudioClip enemyFireSound; // Son de tir ennemi
    public AudioClip hitSound; // Son d'impact
    public AudioClip footstepSound; // Son de pas
    
    [Header("Paramètres")]
    public float musicVolume = 0.7f;
    public float sfxVolume = 1.0f;
    
    public static AudioManager Instance;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persister entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Configurer les sources audio
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.loop = true;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.loop = false;
        }
        
        // Démarrer la musique de fond
        PlayBackgroundMusic();
    }
    
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && themeMusic != null)
        {
            musicSource.clip = themeMusic;
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PlayFireSound()
    {
        PlaySFX(fireSound);
    }
    
    public void PlayReloadSound()
    {
        PlaySFX(reloadSound);
    }
    
    public void PlayEnemyFireSound()
    {
        PlaySFX(enemyFireSound);
    }
    
    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }
    
    public void PlayFootstepSound()
    {
        PlaySFX(footstepSound);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}
