using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Nom de la sc�ne � charger
    public string parametres;

    // M�thode � appeler lors du clic sur le bouton
    public void LoadScene()
    {
        SceneManager.LoadScene(parametres);


    }

}