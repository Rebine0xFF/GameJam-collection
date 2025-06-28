using UnityEngine;

public class modifcamera : MonoBehaviour
{
    public platformer_player vasi;
    public Camera camera;
    public float zoom1;
    public float zoom2;
    public float zoom3;
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
        if (vasi.lampe1)
        {
            camera.orthographicSize /= zoom1;
        }
        else if (vasi.lampe2)
        {
            camera.orthographicSize /= zoom2;
        }
        else 
        {
            camera.orthographicSize /= zoom3;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (vasi.lampe1)
        {
            camera.orthographicSize *= 2;
        }
        else if (vasi.lampe2)
        {
            camera.orthographicSize *= 3;
        }
        else
        {
            camera.orthographicSize *= 4;
        }
    }

}
