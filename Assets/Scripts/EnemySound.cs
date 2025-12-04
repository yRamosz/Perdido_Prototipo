using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip roarClip;      // Som forte do monstro
    public AudioClip stepClip;      // Som de passos do monstro

    [Header("Wave Emitters")]
    public GameObject roarWavePrefab;   // Onda forte (ruído)
    public GameObject stepWavePrefab;   // Onda fraca (passos)

    [Header("Roar Settings")]
    public float roarInterval = 5f;   // Tempo entre um rugido e outro
    private float roarTimer;

    [Header("Step Settings")]
    public float stepInterval = 0.5f; // Ritmo dos passos
    private float stepTimer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        roarTimer = roarInterval;
        stepTimer = stepInterval;
    }

    void Update()
    {
        HandleRoarSounds();
        HandleStepSounds();
    }

    void HandleRoarSounds()
    {
        roarTimer -= Time.deltaTime;

        if (roarTimer <= 0f)
        {
            // toca rugido
            if (roarClip != null)
                source.PlayOneShot(roarClip, 1f);

            EmitWave(roarWavePrefab); // onda forte

            roarTimer = roarInterval;
        }
    }

    void HandleStepSounds()
    {
        bool isMoving = rb.velocity.magnitude > 0.1f;

        if (!isMoving) return;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            if (stepClip != null)
                source.PlayOneShot(stepClip, 0.6f);

            EmitWave(stepWavePrefab); // onda fraca

            stepTimer = stepInterval;
        }
    }

    void EmitWave(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject emitter = Instantiate(prefab, transform.position, Quaternion.identity);
        SoundWaveEmitter swe = emitter.GetComponent<SoundWaveEmitter>();
        if (swe != null)
            swe.Emit();
    }
}
