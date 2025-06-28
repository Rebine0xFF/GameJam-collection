using UnityEngine;

public class fusee : MonoBehaviour
{
    public GameObject videocanvas;
    public bool vitre;
    public bool aileron1;
    public bool aileron2;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (vitre && aileron1 && aileron2) 
        {
            EndGame();
        }
    }
    public void EndGame()
    {
        videocanvas.SetActive(true);

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
