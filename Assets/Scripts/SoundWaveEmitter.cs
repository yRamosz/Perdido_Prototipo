using UnityEngine;

public class SoundWaveEmitter : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject particlePrefab;
    public int baseParticleCount = 80;
    public float baseSpreadAngle = 360f;
    public float baseParticleSpeed = 6f;
    public float baseParticleLifetime = 1.2f;

    [Header("Multipliers")]
    public float particleMultiplier = 1f;
    public float scaleMultiplier = 1f;
    public float angleJitter = 4f;
    public int maxParticlesClamp = 500;

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

        for (int i = 0; i < particleCount; i++)
        {
            float normalized = (float)i / particleCount;
            float angle = normalized * baseSpreadAngle;
            angle += Random.Range(-angleJitter, angleJitter);

            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            GameObject p = Instantiate(particlePrefab, transform.position, rot);

            SoundParticle sp = p.GetComponent<SoundParticle>();
            if (sp != null)
            {
                sp.originPoint = transform.position;
                sp.speed = baseParticleSpeed;
                sp.lifetime = baseParticleLifetime;
            }

            p.transform.localScale *= scaleMultiplier;
        }

        Destroy(gameObject, 0.1f);
    }
}
