using UnityEngine;

public class platformer_movements : MonoBehaviour
{
    [Header("Mouvements")]
    public float moveSpeed = 5f;     // Vitesse de déplacement
    public float jumpForce = 10f;    // Force de saut

    [Header("Touches")]
    public KeyCode leftKey = KeyCode.LeftArrow;       // Touche pour aller à gauche
    public KeyCode rightKey = KeyCode.RightArrow;      // Touche pour aller à droite
    public KeyCode jumpKey = KeyCode.UpArrow;   // Touche pour sauter

    private Rigidbody2D rb;
    private bool isGrounded = false;
        
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveDirection = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = 1f;
        }

        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.collider.CompareTag("Ground"))
        isGrounded = true;
    }

    
}