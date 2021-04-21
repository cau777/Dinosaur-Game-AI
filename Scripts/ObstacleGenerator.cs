using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spotboo.Unity.Interfaces;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour, IInitializable
{
    public static ObstacleGenerator OG { get; private set; }
    public static List<Obstacle> Obstacles { get; private set; }

    public static float ObstacleGap { get => Mathf.Abs(Obstacles[1].transform.position.x - Obstacles[0].transform.position.x); }

    public GameObject[] cactusPrefabs;
    public GameObject[] birdsPrefab;
    public Transform obstacleParent;
    public int minScoreToBirds;

    public void PreInit()
    {
        OG = this;
        Obstacles = new List<Obstacle>();
    }

    public void Init() { }

    public void PostInit() { }

    public void GenerateObstacle()
    {
        for (int index = 0; index < Obstacles.Count; index++)
        {
            Obstacle obstacle = Obstacles[index];
            if (obstacle.ObstacleDistance < 0)
            {
                Obstacles.RemoveAt(index);
            }
        }

        //Keep always 3 obstacles
        if (Obstacles.Count == 0)
        {
            GenerateObstacle(Random.Range(17.5f, 20f));
        }

        if (Obstacles.Count <= 1)
        {
            GenerateObstacle(Obstacles[0].ObstacleDistance + Random.Range(17.5f, 20f));
        }

        if (Obstacles.Count <= 2)
        {
            GenerateObstacle(Obstacles[1].ObstacleDistance + Random.Range(17.5f, 20f));
        }
    }

    private void GenerateObstacle(float distance)
    {
        List<GameObject> possibleObjects = new List<GameObject>();
        possibleObjects.AddRange(cactusPrefabs);

        if (SimulationController.Score >= minScoreToBirds)
        {
            possibleObjects.AddRange(birdsPrefab);
        }

        int randId = Random.Range(0, possibleObjects.Count);
        GameObject obj = Instantiate(possibleObjects[randId], obstacleParent, true);
        obj.transform.position += new Vector3(distance, 0);

        Obstacle obstacle = obj.GetComponent<Obstacle>();
        Obstacles.Add(obstacle);
    }

    public void ClearObstacles()
    {
        foreach (Obstacle obstacle in Obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        Obstacles.Clear();
    }
}