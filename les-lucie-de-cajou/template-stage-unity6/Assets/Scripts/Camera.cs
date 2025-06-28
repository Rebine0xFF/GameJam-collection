using UnityEngine;

public class CameraSmashBros : MonoBehaviour
{
    public Transform joueur1, joueur2;
    public float zoomMin = 3f, zoomMax = 10f;
    public float vitesseLissage = 10f;
    public Vector2 minLimite = new Vector2(-10f, -10f);
    public Vector2 maxLimite = new Vector2(10f, 10f);

    void LateUpdate()
    {

        if (joueur1 == null || joueur2 == null)
        {
            return;
        }


        Vector3 centre = (joueur1.position + joueur2.position) / 2;

        centre.x = Mathf.Clamp(centre.x, minLimite.x, maxLimite.x);
        centre.y = Mathf.Clamp(centre.y, minLimite.y, maxLimite.y);


        float distance = Vector3.Distance(joueur1.position, joueur2.position);
        float zoom = Mathf.Clamp(distance * 0.3f + 2f, zoomMin, zoomMax); 

        Vector3 positionCible = new Vector3(centre.x, centre.y, -zoom);
        
        Camera.main.orthographicSize = zoom;

        transform.position = Vector3.Lerp(transform.position, positionCible, Time.deltaTime * vitesseLissage);
    }
}