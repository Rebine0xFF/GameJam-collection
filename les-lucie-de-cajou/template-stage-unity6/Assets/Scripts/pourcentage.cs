using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text pourcent1;
    public Text pourcent2;

    public Damageable joueur1;
    public Damageable joueur2;

    void Update()
    {
        if (joueur1 != null)
            pourcent1.text = joueur1.pourcentage.ToString("0") + "%";

        if (joueur2 != null)
            pourcent2.text = joueur2.pourcentage.ToString("0") + "%";
    }
}
