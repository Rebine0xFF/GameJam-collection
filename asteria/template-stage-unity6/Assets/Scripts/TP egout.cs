using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class TPegout : MonoBehaviour
{
    public Transform teleportDestination;
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
        collision.gameObject.transform.position = teleportDestination.position;
    }
}
