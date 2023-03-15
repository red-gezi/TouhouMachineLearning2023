using UnityEngine;

namespace TouhouMachineLearningSummary.Test
{
    public class ParticeTest : MonoBehaviour
    {
        ParticleSystem particleSystem;
        ParticleSystem.Particle[] particles;
        // Start is called before the first frame update
        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        }

        // Update is called once per frame
        void Update()
        {
            int activeNum = particleSystem.GetParticles(particles);
            for (int i = 0; i < activeNum; i++)
            {
                float v = Time.time;
                Vector3 target = new Vector3(Mathf.Cos(Time.time * 10) * (100 - v), v, Mathf.Sin(i * 10) * (100 - v));
                particles[i].position = Vector3.Lerp(particles[i].position, target, 0.01f);

                //particles[i].velocity -= vector*0.01f;
            }
            particleSystem.SetParticles(particles);
        }
    }
}