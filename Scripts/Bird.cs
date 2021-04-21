using UnityEngine;

public class Bird : Obstacle
{
    private void Awake()
    {
        float yPos = Random.Range(0.15f, 1.0f);
        transform.position += new Vector3(0, yPos);
        GetProperties();
    }
}