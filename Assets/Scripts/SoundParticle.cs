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
        if (rb != null)
            rb.velocity = transform.up * speed;
    }

    void Update()
    {
        timer += Time.deltaTime;

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
        if (stopOnCollision && other.CompareTag("Wall"))
        {
            if (rb != null)
                rb.velocity = Vector2.zero;
        }
    }
}
