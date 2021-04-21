using System;
using System.Collections;
using System.Collections.Generic;
using Compute;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dino : MonoBehaviour
{
    public BoxCollider2D walkingCollider;
    public BoxCollider2D duckingCollider;

    public float[] Weights { get; set; }
    public float[] Bias { get; set; }
    public uint Decision { get; private set; }

    [HideInInspector] public float bestScore;

    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityForce;

    private float _force;
    private bool _jumping;
    private Renderer _renderer;
    private Animator _animator;
    private bool _ducking;
    private static readonly int Ducking = Animator.StringToHash("Ducking");

    public void InputDecision(uint decision)
    {
        Decision = decision;
        switch (decision)
        {
            case 0u:
                if (_ducking) Duck(false);
                break;
            case 1u:
                Jump();
                break;
            default:
                if (!_ducking) Duck(true);
                break;
        }
    }

    public void GenerateRandomAI()
    {
        Weights = new float[AIComputer.WeightsLength];
        Bias = new float[AIComputer.BiasLength];

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Random.Range(-2f, 2f);
        }

        for (int i = 0; i < Bias.Length; i++)
        {
            Bias[i] = Random.Range(-2f, 2f);
        }
    }

    private void Die()
    {
        bestScore = SimulationController.Score;
        gameObject.SetActive(false);
        SimulationController.AliveDinos.Remove(this);
    }

    private void Jump()
    {
        bool canJump = (transform.position.y <= 0.1f);

        if (canJump && _force < 0.1f)
        {
            _jumping = false;
        }

        if (canJump && !_jumping)
        {
            _force = jumpForce;
            _jumping = true;
        }
    }

    private void Duck(bool state)
    {
        duckingCollider.enabled = state;
        walkingCollider.enabled = !state;

        _animator.SetBool(Ducking, state);
        _ducking = state;
    }

    //Get components and sets a random color
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
    }

    private void Update()
    {
        float gravityMulti = _ducking ? 4 : 1;

        Vector3 pos = transform.position;
        pos.y = Mathf.Max(pos.y + _force * Time.deltaTime, 0);
        transform.position = pos;

        _force -= gravityForce * Time.deltaTime * gravityMulti;
        _force = Mathf.Max(_force, -gravityForce * gravityMulti);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Die();
    }
}