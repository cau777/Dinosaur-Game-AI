using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spotboo.Unity.Interfaces;
using TMPro;
using UnityEditor;
using UnityEngine;

//Create visual representation of the best dino's neural network
public class NeuralNetworkCreator : MonoBehaviour, IInitializable
{
    public GameObject neuronPrefab;
    public Vector2 space;
    public TextMeshProUGUI textInputs;
    public TextMeshProUGUI textOutputs;
    public int updatesPerSecond;

    [Header("Lines")] public GameObject linePrefab;
    public float lineThickness;

    private static readonly int[] Layers = {7, 6, 4, 3};
    private RectTransform _rectTransform;
    private List<Renderer> _neuronRenderers;
    private List<Renderer> _lineRenderers;
    private Vector2[][] _neuronPositions;
    private Transform _linePrefab;
    private static readonly int Bias = Shader.PropertyToID("_Bias");
    private static readonly int Weight = Shader.PropertyToID("_Weight");

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _neuronRenderers = new List<Renderer>();
        _lineRenderers = new List<Renderer>();
        _neuronPositions = new Vector2[Layers.Length][];

        Vector2 dimensions = _rectTransform.sizeDelta;

        for (int l = 0; l < Layers.Length; l++)
        {
            int neurons = Layers[l];
            _neuronPositions[l] = new Vector2[neurons];

            for (int n = 0; n < neurons; n++)
            {
                GameObject obj = Instantiate(neuronPrefab, transform, true);
                Vector2 offset = new Vector2(dimensions.x - space.x * (Layers.Length - 1),
                    dimensions.y - space.y * (neurons - 1)) / 2f;
                obj.transform.localPosition = (space * new Vector2(l, n) - dimensions / 2f) + offset;

                _neuronRenderers.Add(obj.GetComponent<Renderer>());
                _neuronPositions[l][n] = obj.transform.position;
            }
        }


        Transform found = gameObject.transform.Find("Lines");

        if (found == null)
        {
            _linePrefab = new GameObject("Lines").transform;
            _linePrefab.SetParent(transform);
        }
        else
            _linePrefab = found;

        for (int l = 0; l < Layers.Length - 1; l++)
        {
            for (int current = 0; current < Layers[l]; current++)
            {
                for (int next = 0; next < Layers[l + 1]; next++)
                {
                    Vector2 currentPos = _neuronPositions[l][current];
                    Vector2 nextPos = _neuronPositions[l + 1][next];

                    // Vector2 currentPos = _neuronPositions[0][0];
                    // Vector2 nextPos = _neuronPositions[1][0];

                    Vector2 center = Vector2.Lerp(currentPos, nextPos, 0.5f);
                    float length = Vector2.Distance(currentPos, nextPos);
                    float rot = Mathf.Acos(Vector2.Dot(Vector2.left, (nextPos - center).normalized)) * Mathf.Rad2Deg;
                    rot *= (center.y < nextPos.y) ? -1 : 1;

                    GameObject obj = Instantiate(linePrefab, _linePrefab, true);
                    obj.transform.localScale = new Vector3(length, lineThickness, 1);
                    obj.transform.position = center;
                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));

                    Renderer component = obj.GetComponent<Renderer>();
                    _lineRenderers.Add(component);
                }
            }
        }

        textInputs.lineSpacing = space.y * 2.6f;
    }

    public void PreInit() { }

    public void Init() { }

    public void PostInit()
    {
        StartCoroutine(UpdateNetwork());
    }

    //Runs less times so this function doesn't impact the framerate as much
    private IEnumerator UpdateNetwork()
    {
        while (true)
        {
            if (SimulationController.CurrentState != SimulationState.Stopped && SimulationController.AliveDinos.Count != 0)
            {
                Dino first = SimulationController.AliveDinos.First();
                textInputs.text = $"Speed : {SimulationController.GameSpeed:F3}\r\n" +
                                  $"Obstacle Distance: {ObstacleGenerator.Obstacles.First().ObstacleDistance:F3}\r\n" +
                                  $"Obstacle Height: {ObstacleGenerator.Obstacles.First().ObstacleHeight:F3}\r\n" +
                                  $"Obstacle Width: {ObstacleGenerator.Obstacles.First().ObstacleWidth:F3}\r\n" +
                                  $"Obstacle Y: {ObstacleGenerator.Obstacles.First().ObstacleY:F3}\r\n" +
                                  $"Obstacle Gap: {ObstacleGenerator.ObstacleGap:F3}\r\n" +
                                  $"Dino Y: {first.transform.position.y:F3}";

                textOutputs.text = first.Decision switch
                {
                    0u => "Walk",
                    1u => "Jump",
                    _ => "Duck"
                };

                for (int n = 0; n < _neuronRenderers.Count; n++)
                {
                    _neuronRenderers[n].material.SetFloat(Bias, first.Bias[n]);
                }

                for (int w = 0; w < _lineRenderers.Count; w++)
                {
                    _lineRenderers[w].material.SetFloat(Weight, first.Weights[w]);
                }

                yield return new WaitForSeconds(1f / updatesPerSecond);
            }
            else
            {
                yield return null;
            }
        }
    }
}