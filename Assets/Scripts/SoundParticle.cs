using UnityEngine;

public class SoundParticle : MonoBehaviour
{
    [HideInInspector] public Vector2 originPoint;

    public float speed = 6f;
    public float lifetime = 1.2f;
    public bool stopOnCollision = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float timer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originPoint = transform.position;

        if (rb != null)
            rb.velocity = transform.up * speed;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Fading da partícula
        if (sr != null)
        {
            float a = Mathf.Clamp01(1f - (timer / lifetime));
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Não parar em player ou outras ondas
        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("SoundParticle"))
            return;

        // Evita parar no próprio inimigo (caso inimigo gere ondas também)
        if (other.CompareTag("Enemy") && gameObject.CompareTag("EnemySound"))
            return;

        // Parar na parede
        if (stopOnCollision && other.CompareTag("Wall"))
        {
            if (rb != null)
                rb.velocity = Vector2.zero;
        }
    }
}
