using UnityEngine;
using System.Collections.Generic;

public class BulletTracerPool : MonoBehaviour
{
    public GameObject tracerPrefab;
    public int poolSize = 10;

    private Queue<GameObject> tracerPool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tracer = Instantiate(tracerPrefab);
            tracer.SetActive(false);
            tracerPool.Enqueue(tracer);
        }
    }

    public GameObject GetTracer()
    {
        if (tracerPool.Count > 0)
        {
            GameObject tracer = tracerPool.Dequeue();
            tracer.SetActive(true);
            return tracer;
        }
        else
        {
            // Pool vide, on peut instancier un nouveau tracer si besoin
            GameObject tracer = Instantiate(tracerPrefab);
            return tracer;
        }
    }

    public void ReturnTracer(GameObject tracer)
    {
        tracer.SetActive(false);
        tracerPool.Enqueue(tracer);
    }
}
