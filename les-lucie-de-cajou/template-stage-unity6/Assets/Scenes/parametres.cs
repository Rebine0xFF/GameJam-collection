using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Nom de la scène à charger
    public string parametres;

    // Méthode à appeler lors du clic sur le bouton
    public void LoadScene()
    {
        SceneManager.LoadScene(parametres);


    }

}