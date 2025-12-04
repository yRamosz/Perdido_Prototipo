using UnityEngine;

public class EnemyHearingAI : MonoBehaviour
{
    [Header("Audição")]
    public float hearingRange = 10f;

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

    private Vector2 lastKnownPosition;
    private bool isAlert = false;

    private Vector2 patrolDestination;
    private float patrolTimer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolTimer = patrolChangeTime;
        SetNewPatrolDestination();
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

    void ListenForSounds()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, hearingRange);

        foreach (var col in found)
        {
            if (col.CompareTag("SoundParticle"))
            {
                SoundParticle p = col.GetComponent<SoundParticle>();
                if (p != null)
                {
                    lastKnownPosition = p.originPoint;
                    isAlert = true;
                }
            }
        }
    }

    void MoveAndRotate()
    {
        Vector2 target;
        float speed;

        if (isAlert)
        {
            // ALERTA → vai até a última posição do som
            target = lastKnownPosition;
            speed = moveSpeed;

            // Se chegou no ponto investigado, volta a patrulhar
            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.5f)
            {
                isAlert = false;
                SetNewPatrolDestination();
            }
        }
        else
        {
            // PATRULHA NORMAL
            target = patrolDestination;
            speed = patrolSpeed;

            patrolTimer -= Time.fixedDeltaTime;

            bool reached = Vector2.Distance(transform.position, patrolDestination) < 0.5f;
            bool timeout = patrolTimer <= 0f;

            if (reached || timeout)
            {
                SetNewPatrolDestination();
            }
        }

        // ROTACIONA SUAVEMENTE
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        // MOVIMENTO
        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );
        rb.MovePosition(newPos);
    }

    void HandleFootsteps()
    {
        bool moving = rb.velocity.magnitude > 0.1f;

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
            if (swe != null) swe.Emit();
        }
    }

    void SetNewPatrolDestination()
    {
        patrolTimer = patrolChangeTime;

        // Gera novo alvo ao redor da posição ATUAL (e não a inicial!)
        float angle = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        patrolDestination = (Vector2)transform.position + dir * Random.Range(1f, patrolRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        if (isAlert)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lastKnownPosition);
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        }
    }
}
