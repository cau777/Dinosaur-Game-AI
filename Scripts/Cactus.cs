using UnityEngine;

public class Cactus : Obstacle
{
    private void Awake()
    {
        transform.position += new Vector3(0, Random.Range(0.0f, 0.3f));
        GetProperties();
    }
}