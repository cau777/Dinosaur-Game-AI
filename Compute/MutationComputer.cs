using UnityEngine;

namespace Compute
{
    public class MutationComputer : MonoBehaviour, IInitializable
    {
        public static MutationComputer Computer { get; private set; }

        public ComputeShader shader;

        public void ComputeMutation(ref float[] reference)
        {
            shader.SetFloat("Time", Time.time);
            shader.SetFloat("MutationChance", SimulationController.MutationChance / 100f);

            ComputeHelper helper = new ComputeHelper(shader, 0);
            helper.CreateReadWriteBuffer("Reference", reference.Length, sizeof(float), reference);

            helper.RunShader(reference.Length, 1, 1);
        }

        public void PreInit()
        {
            Computer = this;
        }

        public void Init() { }

        public void PostInit() { }
    }
}