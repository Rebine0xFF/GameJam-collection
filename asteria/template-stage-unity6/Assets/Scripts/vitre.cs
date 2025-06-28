using UnityEngine;

public class vitre : MonoBehaviour
{
    public fusee fusee;
    [SerializeField] GameObject fuse;
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
        fuse.transform.localPosition = new Vector3(-16, -1, -1);
        fusee.vitre = true;

    }

}

    