using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanMoves : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    public Animator anim;
    

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(5.684651f, 5.684651f, 5.684651f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-5.684651f, 5.684651f, 5.684651f);

        // Jump
        if (Input.GetKey(KeyCode.Space) && body.velocity.y == 0)
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
        // Climb
        if (Input.GetKey(KeyCode.UpArrow))
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0.3f), ForceMode2D.Impulse);


        anim.SetFloat("XVelocity", Mathf.Abs(body.velocity.x));
        anim.SetFloat("YVelocity", Mathf.Abs(body.velocity.y));
    }

}
