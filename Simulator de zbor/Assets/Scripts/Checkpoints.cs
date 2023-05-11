using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField]
    Plane plane;

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

        float xOffset = Random.Range( 1000,  3000);
        float yOffset = Random.Range( 1000,  3000);
        float zOffset = Random.Range( 1000,  3000);

        this.transform.position = new Vector3(
            (Random.Range(0, 2) * 2 - 1) * xOffset + planePosition.x,
            (Random.Range(0, 2) * 2 - 1) * yOffset + planePosition.y, 
            (Random.Range(0, 2) * 2 - 1) * zOffset + planePosition.z);
    }

    private void Start()
    {
        RandomizePosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        RandomizePosition();
    }
}
