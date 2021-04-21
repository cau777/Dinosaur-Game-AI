using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    public float ObstacleHeight { get; private set; }
    public float ObstacleWidth { get; private set; }
    public float ObstacleY { get; private set; }
    public float ObstacleDistance { get => transform.position.x; }

    protected void GetProperties()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        Bounds bounds = collider2D.bounds;
        ObstacleHeight = bounds.size.y;
        ObstacleWidth = bounds.size.x;
        ObstacleY = transform.position.y;
    }

    private void Update()
    {
        //Update position and destroy the object if it's outside the screen
        Vector3 pos = transform.position;

        if (pos.x < -1)
        {
            Destroy(gameObject);
        }

        pos.x -= SimulationController.GameSpeed * Time.deltaTime * 8f;
        transform.position = pos;
    }
}