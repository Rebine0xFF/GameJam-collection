using UnityEngine;

public class aileron : MonoBehaviour
{
    public fusee fuseeee;
    [SerializeField] GameObject fusee;
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
        fusee.transform.localPosition = new Vector3(-16.85f, -2.35f, -1);
        fuseeee.aileron1 = true;

    }
}

