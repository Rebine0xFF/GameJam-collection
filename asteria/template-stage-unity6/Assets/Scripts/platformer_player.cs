using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformer_player : MonoBehaviour
{
    public bool lampe1;
    public bool lampe2;
    public bool nearlampe1;
    public bool nearlampe2;
    public GameObject Lampe1;
    public GameObject Lampe2;

    public Animator FilleAnimator;
    public SpriteRenderer Fille;
    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 motion = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position = transform.position + motion * Time.deltaTime;

        FilleAnimator.SetFloat("Speed", Mathf.Abs(motion.x));

        if (motion.x > 0.1)
        {

            Fille.flipX = false;
        }
        else if (motion.x < -0.1)
        {
            Fille.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (nearlampe1)
            {
                lampe1 = true;
                Destroy(Lampe1);
                Destroy(Lampe2);

            }
            else if (nearlampe2)
            {
                lampe2 = true;
                Destroy(Lampe2);
                Destroy(Lampe1);

            }
            
                
        }
    }
    public void L1(GameObject g, bool b) 
    {
        nearlampe1 = b;
        Lampe1= g;
    }
    public void L2(GameObject g, bool b)
    {
        nearlampe2 = b;
        Lampe2 = g;
    }
}
