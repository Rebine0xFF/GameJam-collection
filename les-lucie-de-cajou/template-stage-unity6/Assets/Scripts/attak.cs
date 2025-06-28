using UnityEngine;

public class Attack : MonoBehaviour
{
    public KeyCode attakKey = KeyCode.R;
    public Transform pointattak;
    public float rayonattak = 2f;

    public float degats = 5f;
    public float puissanceDuCoup = 15f;
    public Animator AttackAnimator;

    public AudioClip hitSound; 
    private AudioSource audioSource; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Touche + Animator + SFX
    void Update()
    {
        if (Input.GetKeyDown(attakKey))
        {
            AttackCheck(); 

            
            AttackAnimator.SetBool("IsAttacking2", true);
            AttackAnimator.SetBool("IsAttacking", true);

            
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
        else
        {
            AttackAnimator.SetBool("IsAttacking2", false);
            AttackAnimator.SetBool("IsAttacking", false);
        }
    }

    // Attaque
    void AttackCheck()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pointattak.position, rayonattak);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            if (hit.CompareTag("Player"))
            {
                Damageable d = hit.GetComponent<Damageable>();
                if (d != null)
                {
                    d.TakeHit(degats, puissanceDuCoup, transform);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pointattak == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointattak.position, rayonattak);
    }
}
