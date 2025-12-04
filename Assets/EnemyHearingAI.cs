using UnityEngine;

public class EnemyHearingAI : MonoBehaviour
{
    [Header("Audição")]
    public float hearingRange = 7f;

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

    [Header("Player")]
    public Transform player;
    public float attackDistance = 0.4f;

    private bool isAlert = false;
    private Vector2 lastKnownPosition;
    private Vector2 patrolDestination;
    private float patrolTimer;
    private Rigidbody2D rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolTimer = patrolChangeTime;
        SetNewPatrolDestination();

        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        ListenForSounds();
        HandleFootsteps();
        CheckKillPlayer();
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    // Detecta partículas sonoras do player
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
                    isAlert = true;
                    lastKnownPosition = (Vector2)player.position; // Persegue a posição atual do player
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
            // ALERTA → persegue o player
            target = lastKnownPosition;
            speed = moveSpeed;

            // Se chegou próximo do player e não detectou novas ondas, volta a patrulhar
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                isAlert = false;
                SetNewPatrolDestination();
            }
        }
        else
        {
            // Patrulha normal
            target = patrolDestination;
            speed = patrolSpeed;

            patrolTimer -= Time.fixedDeltaTime;

            if (Vector2.Distance(transform.position, patrolDestination) < 0.5f || patrolTimer <= 0f)
                SetNewPatrolDestination();
        }

        // Rotação suave
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.fixedDeltaTime);
        }

        // Movimento
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    void HandleFootsteps()
{
    // Calcula se está se movendo com base na posição alvo
    Vector2 target = isAlert ? (Vector2)player.position : patrolDestination;
    Vector2 moveDelta = target - rb.position;
    bool moving = moveDelta.magnitude > 0.01f; // considera movimento mesmo sem velocity

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
        float angle = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        patrolDestination = (Vector2)transform.position + dir * Random.Range(1f, patrolRadius);
    }

    // Mata o player ao encostar
    void CheckKillPlayer()
    {
        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            if (gameManager != null)
                gameManager.TriggerGameOver();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
