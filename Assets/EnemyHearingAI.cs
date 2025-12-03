using UnityEngine;

public class EnemyHearingAI : MonoBehaviour
{
    [Header("Audição")]
    public float hearingRange = 10f;
    public float directPlayerDetectionRange = 2.5f; // distância pra detectar o player pela onda

    [Header("Movimento da IA")]
    public float moveSpeed = 5f;
    public float patrolSpeed = 2f;
    public float patrolRadius = 5f;
    public float patrolChangeTime = 3f;
    public float rotationSpeed = 10f;

    [Header("Sistema de Passos")]
    public GameObject soundWavePrefab;
    public float waveSpawnInterval = 0.5f;
    private float waveTimer = 0f;

    // Internas
    private Vector2 lastKnownPosition;
    private bool isAlert = false;
    private bool chasingPlayer = false;
    private Vector2 patrolDestination;
    private float patrolTimer;
    private Vector2 startPosition;
    private Rigidbody2D rb;

    private Transform player; // referência ao player

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        patrolTimer = patrolChangeTime;
        SetNewPatrolDestination();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        ListenForSounds();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    // --------------------------------------------------------------------
    // SISTEMA DE AUDIÇÃO
    // --------------------------------------------------------------------
    void ListenForSounds()
    {
        Collider2D[] foundSounds = Physics2D.OverlapCircleAll(transform.position, hearingRange);

        foreach (var soundCollider in foundSounds)
        {
            if (soundCollider.CompareTag("SoundParticle"))
            {
                SoundParticle particle = soundCollider.GetComponent<SoundParticle>();

                if (particle != null)
                {
                    lastKnownPosition = particle.originPoint;
                    isAlert = true;

                    // 🔥 Novo: Se a PARTÍCULA estiver muito perto, assume que o player está próximo
                    float dist = Vector2.Distance(transform.position, soundCollider.transform.position);
                    if (dist <= directPlayerDetectionRange)
                    {
                        chasingPlayer = true;
                    }
                }
            }
        }
    }

    // --------------------------------------------------------------------
    // MOVIMENTO E ROTAÇÃO
    // --------------------------------------------------------------------
    void MoveAndRotate()
    {
        Vector2 target = patrolDestination;
        float speed = patrolSpeed;

        // PRIORIDADE 1 — Perseguindo diretamente o player
        if (chasingPlayer && player != null)
        {
            target = player.position;
            speed = moveSpeed;

            if (Vector2.Distance(transform.position, player.position) > hearingRange * 1.5f)
                chasingPlayer = false; // perde o player se ficar muito longe
        }
        // PRIORIDADE 2 — Investigando última posição da onda
        else if (isAlert)
        {
            target = lastKnownPosition;
            speed = moveSpeed;

            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.5f)
            {
                isAlert = false;
                SetNewPatrolDestination();
            }
        }
        // PRIORIDADE 3 — Patrulha normal
        else
        {
            patrolTimer -= Time.fixedDeltaTime;

            if (Vector2.Distance(transform.position, patrolDestination) < 0.5f || patrolTimer <= 0)
                SetNewPatrolDestination();
        }

        // Rotação
        Vector2 dir = target - rb.position;
        if (dir.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, targetAngle),
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        // Movimento
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    // --------------------------------------------------------------------
    // PEGADAS / EMISSÃO DE SOM DO INIMIGO
    // --------------------------------------------------------------------
    void HandleFootsteps()
    {
        bool moving =
            (isAlert && Vector2.Distance(transform.position, lastKnownPosition) > 0.5f)
            || (!isAlert && Vector2.Distance(transform.position, patrolDestination) > 0.5f)
            || chasingPlayer;

        if (moving)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                CreateSoundWave();
                waveTimer = waveSpawnInterval;
            }
        }
        else
        {
            waveTimer = 0f;
        }
    }

    void CreateSoundWave()
    {
        if (soundWavePrefab != null)
        {
            GameObject emitter = Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
            SoundWaveEmitter swe = emitter.GetComponent<SoundWaveEmitter>();
            if (swe != null)
                swe.Emit();
        }
    }

    // --------------------------------------------------------------------
    // PATRULHA
    // --------------------------------------------------------------------
    void SetNewPatrolDestination()
    {
        float a = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad));
        patrolDestination = startPosition + dir * Random.Range(1f, patrolRadius);
        patrolTimer = patrolChangeTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, directPlayerDetectionRange);
    }
}
