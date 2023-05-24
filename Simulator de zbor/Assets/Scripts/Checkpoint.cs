using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    Plane plane;
    [SerializeField]
    float maxHeight;

    RaycastHit hit;

    int dificultyOffset = 0;

    float score=0f;
    float oldTime;
    float height;
    public Vector3 Position
    {
        get
        {
            return this.transform.position;
        }
    }
    public void RandomizePosition()
    {
        Vector3 planePosition = plane.transform.position;

        float xOffset = Random.Range( 500,  1500);
        float zOffset = Random.Range( 500,  1500);

        float newX = (Random.Range(0, 2) * 2 - 1) * xOffset + planePosition.x;
        float newZ= (Random.Range(0, 2) * 2 - 1) * zOffset + planePosition.z;

        float terrainHeight=0;
        Ray ray = new Ray(new Vector3(newX, maxHeight, newZ), Vector3.down);
        if(Physics.Raycast(ray,out hit))
        {
            terrainHeight = maxHeight - hit.distance;
        }
        float newY;
        if (maxHeight - dificultyOffset-terrainHeight > 150) {
            newY = Random.Range(terrainHeight + 150, maxHeight - dificultyOffset);
        }
        else
        {
            newY = terrainHeight + 150;
        }

        height = newY - terrainHeight;

        this.transform.position = new Vector3(newX,
            newY, 
            newZ);
       
        dificultyOffset += 100;
        
    }

    public void CalculateScore()
    {
        float currentTime = Time.time;
        float timeBetewwnCheckpoints = currentTime - oldTime;
        if (timeBetewwnCheckpoints <= 0)
        {
            timeBetewwnCheckpoints = 0.01f;
        }
        score += 1000000 * (1 / timeBetewwnCheckpoints)*(1/height);
        oldTime = currentTime;
    }
    public int getScore()
    {
        return (int)score;
    }

    private void Start()
    {
        RandomizePosition();
        oldTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        CalculateScore();
        RandomizePosition();
    }
}
