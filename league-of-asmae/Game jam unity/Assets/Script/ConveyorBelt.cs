using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float conveyorSpeed = 2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("AAAAAAAA");
        Rigidbody2D rb = collision.rigidbody;
        if (rb != null)
        {
            Vector2 direction = new Vector2(1, 0);
            rb.velocity = new Vector2(conveyorSpeed, rb.velocity.y);
        }
    }
}
