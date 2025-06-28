using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartonScript : MonoBehaviour
{
    public bool Lifted;

    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Lifted)
        {
            transform.position = Player.transform.position;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("t5tttttt");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("vvvvvvvvvvvvvvvvvvv");
            if (Input.GetKeyDown(KeyCode.P))
            {
                Lifted = !Lifted;
                if (Lifted)
                {
                    Player = other.gameObject;
                    
                    
                    
                    
                }
            }
        }
    }
}
