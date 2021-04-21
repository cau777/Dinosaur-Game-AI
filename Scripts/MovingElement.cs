using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Update the ground speed with the game speed
public class MovingElement : MonoBehaviour
{
    public float multi;

    private Renderer _renderer;
    private static readonly int Speed = Shader.PropertyToID("_Speed");

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        float speedValue = SimulationController.CurrentState == SimulationState.Running ? multi * SimulationController.GameSpeed : 0f;
        _renderer.material.SetFloat(Speed, speedValue);
    }
}