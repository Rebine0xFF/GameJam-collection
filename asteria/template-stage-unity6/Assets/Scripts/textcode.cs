using UnityEngine;

public class textcode : MonoBehaviour
{
    public GameObject g;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        g.SetActive(true);

    }

    private void OnTriggerExit2D(Collider2D collision) { g.SetActive(false); }
}
