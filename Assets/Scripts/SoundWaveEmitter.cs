using UnityEngine;

public class SoundWaveEmitter : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject particlePrefab;         // prefab da partícula (SoundParticle)
    public int baseParticleCount = 80;        // padrão para onda normal
    public float baseSpreadAngle = 360f;
    public float baseParticleSpeed = 6f;
    public float baseParticleLifetime = 1.2f;

    [Header("Multipliers (ajustáveis)")]
    public float particleMultiplier = 1f;     // Use <1 para stealth, >1 para forte
    public float scaleMultiplier = 1f;        // escala das partículas
    public float angleJitter = 4f;           // jitter para não ficar rígido
    public int maxParticlesClamp = 500;      // evita criação excessiva

    // Emit() é público para ser chamado de fora (Player/Enemy)
    public void Emit()
    {
        if (particlePrefab == null)
        {
            Debug.LogWarning("[SoundWaveEmitter] particlePrefab não atribuído.");
            Destroy(gameObject);
            return;
        }

        int particleCount = Mathf.RoundToInt(baseParticleCount * particleMultiplier);
        particleCount = Mathf.Clamp(particleCount, 1, maxParticlesClamp);

        // calcula ângulo entre partículas (padrão)
        for (int i = 0; i < particleCount; i++)
        {
            float normalized = (float)i / particleCount;
            float angle = normalized * baseSpreadAngle;
            // adiciona jitter
            angle += Random.Range(-angleJitter, angleJitter);

            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            GameObject p = Instantiate(particlePrefab, transform.position, rot);

            // configura a partícula criada (se existir o script)
            SoundParticle sp = p.GetComponent<SoundParticle>();
            if (sp != null)
            {
                sp.originPoint = transform.position;
                sp.speed = baseParticleSpeed;
                sp.lifetime = baseParticleLifetime;
            }

            // aplica escala
            p.transform.localScale = p.transform.localScale * scaleMultiplier;
        }

        // destrói o emissor rapidamente (parte visual já foi criada)
        Destroy(gameObject, 0.1f);
    }
}
