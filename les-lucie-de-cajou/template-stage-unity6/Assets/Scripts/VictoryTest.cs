using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{

    public void FinDuMatch(string nomDuPerdant)
    {

        if (nomDuPerdant == "player2")
        {
            Debug.Log("Joueur 2 gagne, changement de scène !");
            SceneManager.LoadScene("SceneJoueur2Win");
        }

        else if (nomDuPerdant == "player")
        {
            Debug.Log("Joueur 1 gagne, changement de scène !");
            SceneManager.LoadScene("SceneJoueur1Win");
        }
    }
}
