using UnityEngine;
using UnityEngine.UI; 

public class Damageable : MonoBehaviour
{
    public float pourcentage = 0f;
    public float masse = 1f;
    public int vies = 3;

    public Transform pointDeRespawn;
    public PercentageDisplay pourcentageUI;

    private Rigidbody2D rb;
    public Text texteVies;  

    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;

    public GameObject Heart4;
    public GameObject Heart5;
    public GameObject Heart6;

    
    public string nomJoueur = "player";
    public VictoryManager victoryManager; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pourcentage = 0f;

        if (pourcentageUI != null)
            pourcentageUI.UpdatePercentage(0);

        if (texteVies != null)
            texteVies.text = "Vies: " + vies; 
    }



    public void TakeHit(float degats, float puissanceDuCoup, Transform attaquant)
    {
        pourcentage += degats;

        if (pourcentageUI != null)
        {
            pourcentageUI.UpdatePercentage(Mathf.RoundToInt(pourcentage));
        }

        float knockbackMultiplier = Mathf.Lerp(1f, 4f, pourcentage / 200f);
        float knockbackForce = puissanceDuCoup * knockbackMultiplier / masse;

        Vector2 direction = (transform.position - attaquant.position).normalized;
        direction.y = Mathf.Clamp(direction.y, -0.3f, 0.3f);
        direction.x *= 1.5f;
        direction.Normalize();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

        platformer_movements movement = GetComponent<platformer_movements>();
        if (movement != null)
        {
            movement.Knockback(0.3f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mort"))
        {
            vies--; 

            if (texteVies != null)
                texteVies.text = "Vies: " + vies; 

            if (vies > 0)
            {
                Debug.Log($"{gameObject.name} a perdu une vie. Il en reste {vies}");
                Respawn();
            }
            else
            {
                Debug.Log($"{gameObject.name} est KO !");

               // Syst victoire
                if (victoryManager != null)
                {
                    victoryManager.FinDuMatch(nomJoueur); 
                }

               
                gameObject.SetActive(false);
            }

            // Syst des cœurs perdus 
            if (vies <= 2 && Heart1 != null)
            {
                Destroy(Heart1);
            }

            if (vies <= 1 && Heart2 != null)
            {
                Destroy(Heart2);
            }

            if (vies <= 0 && Heart3 != null)
            {
                Destroy(Heart3);
            }

            if (vies <= 2 && Heart4 != null)
            {
                Destroy(Heart4);
            }

            if (vies <= 1 && Heart5 != null)
            {
                Destroy(Heart5);
            }

            if (vies <= 0 && Heart6 != null)
            {
                Destroy(Heart6);
            }

        }
    }

    void Respawn()
    {
        pourcentage = 0f;

        if (pourcentageUI != null)
            pourcentageUI.UpdatePercentage(0);

        if (pointDeRespawn != null)
        {
            transform.position = pointDeRespawn.position;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
