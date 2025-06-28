using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class aileron2 : MonoBehaviour
{
    public fusee fusee;
    [SerializeField] GameObject fuseee;
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
        fuseee.transform.localPosition = new Vector3(-15.1f, -2.29f, -1);
        fusee.aileron2 = true;
    }
}

