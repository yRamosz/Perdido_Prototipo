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

    // Variáveis privadas 
    private Transform targetSound;
    private Vector2 lastKnownPosition;
    private bool isAlert = false;
    private Vector2 patrolDestination;
    private float patrolTimer;
    private Vector2 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
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
        Collider2D[] foundSounds = Physics2D.OverlapCircleAll(transform.position, hearingRange);
        Transform closestSound = null;
        float closestDist = Mathf.Infinity;

        foreach (var soundCollider in foundSounds)
        {
            if (soundCollider.CompareTag("SoundParticle"))
            {
                float dist = Vector2.Distance(transform.position, soundCollider.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestSound = soundCollider.transform;
                }
            }
        }

        if (closestSound != null)
        {
            targetSound = closestSound;
            lastKnownPosition = targetSound.position;
            isAlert = true;
        }
        else if (targetSound != null)
        {
            targetSound = null;
        }
    }

    void MoveAndRotate()
    {
        Vector2 currentTarget = startPosition;
        float currentSpeed = patrolSpeed;

        if (isAlert)
        {
            if (targetSound != null) lastKnownPosition = targetSound.position;
            currentTarget = lastKnownPosition;
            currentSpeed = moveSpeed;

            if (Vector2.Distance(rb.position, lastKnownPosition) < 0.5f)
            {
                isAlert = false;
                targetSound = null;
                SetNewPatrolDestination();
            }
        }
        else
        {
            // --- MODO PATRULHA ---
            currentTarget = patrolDestination;
            currentSpeed = patrolSpeed;

            patrolTimer -= Time.fixedDeltaTime;
            if (Vector2.Distance(rb.position, patrolDestination) < 0.5f || patrolTimer <= 0f)
            {
                SetNewPatrolDestination();
            }
        }

        // Rotação
        Vector2 direction = currentTarget - rb.position;
        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), rotationSpeed * Time.fixedDeltaTime);
        }

        // Movimento
        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    void HandleFootsteps()
    {
        // Verifica se o inimigo está se movendo (distância do destino > 0.5)
        bool isMoving = false;
        if (isAlert) 
            isMoving = Vector2.Distance(transform.position, lastKnownPosition) > 0.5f;
        else 
            isMoving = Vector2.Distance(transform.position, patrolDestination) > 0.5f;

        if (isMoving)
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
            waveTimer = 0f; // reseta o timer
        }
    }

    void CreateSoundWave()
    {
        if (soundWavePrefab != null)
        {
            GameObject emitter = Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
            
            SoundWaveEmitter emitterScript = emitter.GetComponent<SoundWaveEmitter>();
            if (emitterScript != null)
            {
                emitterScript.Emit();
            }
        }
    }

    void SetNewPatrolDestination()
    {

        float randomAngle = Random.Range(0f, 360f);
        Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        
        patrolDestination = startPosition + randomDirection * Random.Range(1f, patrolRadius);
        patrolTimer = patrolChangeTime; 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, patrolRadius);
        Gizmos.DrawLine(transform.position, patrolDestination);
    }
}