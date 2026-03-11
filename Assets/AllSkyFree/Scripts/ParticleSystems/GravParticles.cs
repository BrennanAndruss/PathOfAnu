using UnityEngine;

public class GravParticles : MonoBehaviour
{
    ParticleSystem particleSys;
    ParticleSystem.Particle[] particles;

    float[] masses;
    float gravityScalar = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masses = new float[1000]; 

        for (int i = 0; i < 1000; i++)
        {
            masses[i] = Random.Range(0.1f,1.0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (particleSys == null)
        {
            particleSys = GetComponent<ParticleSystem>();
        }
        if (particles == null)
        {
            particles = new ParticleSystem.Particle[particleSys.main.maxParticles];
        }

        int numP = particleSys.GetParticles(particles);
        
        for (int i = 0; i < numP; i++) //source
        {
            for (int u = 0; u < numP; u++) // all other particles
            {
                if (i != u)
                {
                Vector3 dir = particles[u].position - particles[i].position;
                float dist = Vector3.Magnitude(dir);
                float force = (masses[u] * masses[i] * gravityScalar) / (dist * dist);
                float acc = force / masses[i];

                Vector3 a_vec = Vector3.Normalize(dir) * acc;
                particles[i].velocity += a_vec;
                }

            }
         
        }

        particleSys.SetParticles(particles, numP);
    }
}
