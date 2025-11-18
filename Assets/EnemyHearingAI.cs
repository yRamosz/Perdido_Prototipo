using UnityEngine;

public class EnemyHearingAI : MonoBehaviour
{
    [Header("Audição")]
    public float hearingRange = 10f; // O "raio de audição" do inimigo

    [Header("Movimento")]
    public float moveSpeed = 2f;     // Velocidade normal de perseguição
    public float patrolSpeed = 1f;   // Velocidade de patrulha
    public float patrolRadius = 5f;  // Raio de patrulha
    public float patrolChangeTime = 3f; // Tempo para mudar o destino de patrulha

    private Transform targetSound;       // A partícula de som específica que ele está seguindo
    private Vector2 lastKnownPosition;   // A última posição onde ele "ouviu" algo
    private bool isAlert = false;        // Estado do inimigo
    private Vector2 patrolDestination;   // Destino atual da patrulha
    private float patrolTimer;           // Timer para mudar o destino de patrulha
    private Vector2 startPosition;       // Posição inicial para a patrulha

    void Start()
    {
        startPosition = transform.position;
        patrolTimer = patrolChangeTime; // Começa com um destino de patrulha
        SetNewPatrolDestination();
    }

    void Update()
    {
        ListenForSounds();
        Move();
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

    void Move()
    {
        if (isAlert)
        {
            if (targetSound != null)
            {
                lastKnownPosition = targetSound.position;
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                lastKnownPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.1f)
            {
                isAlert = false;
                targetSound = null;
                SetNewPatrolDestination(); // Volta à patrulha
            }
        }
        else
        {

            transform.position = Vector2.MoveTowards(
                transform.position,
                patrolDestination,
                patrolSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, patrolDestination) < 0.1f || patrolTimer <= 0f)
            {
                SetNewPatrolDestination();
            }

            patrolTimer -= Time.deltaTime;
        }
    }

    void SetNewPatrolDestination()
    {
        // Gera um ponto aleatório dentro do raio de patrulha, centrado na posição inicial do inimigo
        float randomAngle = Random.Range(0f, 360f);
        Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        patrolDestination = startPosition + randomDirection * Random.Range(0f, patrolRadius);
        patrolTimer = patrolChangeTime; // Reseta o timer
    }
    private void OnDrawGizmosSelected()
    {
        // Raio de Audição
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Raio de Patrulha
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, patrolRadius);

        // Destino de Patrulha
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(patrolDestination, 0.2f); // Pequena esfera para o destino
    }
}