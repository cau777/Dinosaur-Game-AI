using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compute;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationController : MonoBehaviour, IInitializable
{
    public static SimulationController SM { get; private set; }
    public static int Generation { get; private set; }
    public static float Score { get; private set; }
    public static SimulationState CurrentState { get; set; }
    public static List<Dino> AliveDinos { get; private set; }
    public static float GameSpeed { get; private set; }
    public static int PopulationSize { get; set; }
    public static float MaxSpeed { get; set; }
    public static float TimeToMaxSpeed { get; set; }
    public static int AIInterval { get; set; }
    public static float MutationChance { get; set; }
    public static int Seed { get; set; }

    public GameObject dinoPrefab;
    public Transform dinoParent;

    private List<Dino> _dinos = new List<Dino>();

    public void PreInit()
    {
        SM = this;
        GameSpeed = 1f;
        AliveDinos = new List<Dino>();
        CurrentState = SimulationState.Stopped;
    }

    public void Init() { }

    public void PostInit()
    {
        Random.InitState(Seed);
        StartCoroutine(CountingScore());
    }

    public void StartSimulation()
    {
        CurrentState = SimulationState.Running;
        //Create Dinos
        for (int i = 0; i < PopulationSize; i++)
        {
            GameObject obj = Instantiate(dinoPrefab, dinoParent, true);
            obj.name = "Dino " + i;
            Dino d = obj.GetComponent<Dino>();
            d.GenerateRandomAI();
            _dinos.Add(d);
        }

        StartCoroutine(Simulation());
    }

    private IEnumerator Simulation()
    {
        while (CurrentState == SimulationState.Running)
        {
            Generation++;
            int nextThink = 0;
            float simulationTime = 0f;
            Score = 0;
            AliveDinos.Clear();
            AliveDinos.AddRange(_dinos);

            foreach (Dino dino in _dinos)
            {
                dino.gameObject.SetActive(true);
            }

            print("Starting Sim");
            while (AliveDinos.Count > 0)
            {
                //Prepare obstacles and variables
                int totalAlive = AliveDinos.Count;
                simulationTime += Time.deltaTime;
                ObstacleGenerator.OG.GenerateObstacle();

                if (nextThink >= AIInterval)
                {
                    //AI
                    nextThink = 0;

                    float[] weights = new float[AIComputer.WeightsLength * totalAlive];
                    float[] bias = new float[AIComputer.BiasLength * totalAlive];
                    float[] inputs = new float[AIComputer.InputsLength * totalAlive];

                    float obstacleDistance = ObstacleGenerator.Obstacles.First().ObstacleDistance;
                    float obstacleHeight = ObstacleGenerator.Obstacles.First().ObstacleHeight;
                    float obstacleWidth = ObstacleGenerator.Obstacles.First().ObstacleWidth;
                    float obstacleY = ObstacleGenerator.Obstacles.First().ObstacleY;
                    float obstacleGap = ObstacleGenerator.ObstacleGap;


                    //Prepare Arrays
                    for (int i = 0; i < totalAlive; i++)
                    {
                        Dino d = AliveDinos[i];
                        d.Weights.CopyTo(weights, i * AIComputer.WeightsLength);
                        d.Bias.CopyTo(bias, i * AIComputer.BiasLength);

                        int start = i * AIComputer.InputsLength;
                        inputs[start + 0] = GameSpeed;
                        inputs[start + 1] = obstacleDistance;
                        inputs[start + 2] = obstacleHeight;
                        inputs[start + 3] = obstacleWidth;
                        inputs[start + 4] = obstacleY;
                        inputs[start + 5] = obstacleGap;
                        inputs[start + 6] = d.transform.position.y;
                    }

                    //Send Inputs to compute Shader
                    uint[] decisions = AIComputer.AIC.ComputeAIs(inputs, weights, bias, totalAlive);

                    for (int i = 0; i < totalAlive; i++)
                    {
                        //Send Result to Dinos
                        AliveDinos[i].InputDecision(decisions[i]);
                    }
                }

                //Update Game Speed
                GameSpeed = Mathf.Lerp(1f, MaxSpeed, Mathf.Clamp01(simulationTime / TimeToMaxSpeed));

                nextThink++;
                yield return null;
            }

            //Find Dino with best score
            Dino bestDino = _dinos[0];
            float bestScore = _dinos[0].bestScore;

            foreach (Dino dino in _dinos)
            {
                if (dino.bestScore > bestScore)
                {
                    bestDino = dino;
                    bestScore = dino.bestScore;
                }
            }

            //Create list with all dinos but the best
            List<Dino> losers = new List<Dino>();
            losers.AddRange(_dinos);
            losers.Remove(bestDino);

            //Arrays with copies of the best genes
            float[] losersWeights = new float[losers.Count * AIComputer.WeightsLength];
            float[] losersBias = new float[losers.Count * AIComputer.BiasLength];

            for (int i = 0; i < losers.Count; i++)
            {
                bestDino.Weights.CopyTo(losersWeights, i * AIComputer.WeightsLength);
                bestDino.Bias.CopyTo(losersBias, i * AIComputer.BiasLength);
            }

            //Apply mutations to weights and bias
            MutationComputer.Computer.ComputeMutation(ref losersWeights);
            MutationComputer.Computer.ComputeMutation(ref losersBias);

            //Apply mutations to losers dinos
            for (int i = 0; i < losers.Count; i++)
            {
                Dino d = losers[i];
                d.Weights = losersWeights.Skip(i * AIComputer.WeightsLength).Take(AIComputer.WeightsLength).ToArray();
                d.Bias = losersBias.Skip(i * AIComputer.BiasLength).Take(AIComputer.BiasLength).ToArray();
            }

            ObstacleGenerator.OG.ClearObstacles();
        }

        CurrentState = SimulationState.Stopped;
        yield return null;
    }

    //Score updating is done 200 times per second
    private IEnumerator CountingScore()
    {
        const float waitTime = 1f / 200f;
        while (true)
        {
            if (CurrentState != SimulationState.Stopped)
            {
                Score += 50 * waitTime * GameSpeed;
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }
}