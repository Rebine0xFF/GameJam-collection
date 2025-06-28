using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quitter le jeu !");
        Application.Quit();

#if UNITY_EDITOR
        // Pour l’éditeur Unity uniquement
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
