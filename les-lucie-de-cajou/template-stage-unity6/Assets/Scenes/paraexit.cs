using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceness : MonoBehaviour
{
    // Nom de la sc�ne � charger
    public string paraexit;

    // M�thode � appeler lors du clic sur le bouton
    public void LoadSceness()
    {
        SceneManager.LoadScene(paraexit);


    }

}