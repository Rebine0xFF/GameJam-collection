using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceness : MonoBehaviour
{
    // Nom de la scène à charger
    public string paraexit;

    // Méthode à appeler lors du clic sur le bouton
    public void LoadSceness()
    {
        SceneManager.LoadScene(paraexit);


    }

}