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

    // Variáveis privadas para controle
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
            // Se a partícula alvo sumiu, mas ele estava alerta
            // Mantém a última posição conhecida para investigação
            targetSound = null;
        }
    }

    void Move()
    {
        if (isAlert)
        {
            // MODO ALERTA: Persegue a última posição do som
            if (targetSound != null)
            {
                lastKnownPosition = targetSound.position;
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                lastKnownPosition,
                moveSpeed * Time.deltaTime
            );

            // Se o inimigo chegou perto da última posição do som
            if (Vector2.Distance(transform.position, lastKnownPosition) < 0.1f)
            {
                isAlert = false;
                targetSound = null;
                SetNewPatrolDestination(); // Volta à patrulha
            }
        }
        else
        {
            // MODO PATRULHA: Move-se para um destino aleatório dentro de um raio
            transform.position = Vector2.MoveTowards(
                transform.position,
                patrolDestination,
                patrolSpeed * Time.deltaTime
            );

            // Se chegou ao destino de patrulha ou tempo esgotou, define um novo
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

    // Desenha um círculo de gizmo na cena (só visível no Editor)
    // para vermos o "raio de audição" e o raio de patrulha
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