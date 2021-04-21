using Spotboo.Unity.Classes;
using Spotboo.Unity.Interfaces;
using UnityEngine;

namespace Compute
{
    public class AIComputer : MonoBehaviour, IInitializable
    {
        public static AIComputer AIC { get; private set; }
        public ComputeShader shader;
        public const int WeightsLength = 78;
        public const int BiasLength = 20;
        public const int InputsLength = 7;

        public uint[] ComputeAIs(float[] inputs, float[] weights, float[] bias, int total)
        {
            ComputeHelper helper = new ComputeHelper(shader, 0);
            uint[] results = new uint[total];

            shader.SetInt("BiasCount", BiasLength);
            shader.SetInt("WeightsCount", WeightsLength);

            helper.CreateWriteBuffer("Inputs", InputsLength * total, sizeof(float), inputs);
            helper.CreateWriteBuffer("Weights", WeightsLength * total, sizeof(float), weights);
            helper.CreateWriteBuffer("Bias", BiasLength * total, sizeof(float), bias);
            helper.CreateReadBuffer("Results", total, sizeof(uint), results);

            helper.RunShader(total, 1, 1);

            return results;
        }

        public void PreInit()
        {
            AIC = this;
        }

        public void Init() { }

        public void PostInit() { }
    }
}