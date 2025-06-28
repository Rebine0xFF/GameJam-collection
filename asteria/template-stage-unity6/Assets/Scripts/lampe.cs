using UnityEngine;

public class lampe : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<platformer_player>().L1(gameObject, true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        other.GetComponent<platformer_player>().L1(gameObject, false);
    }

}
