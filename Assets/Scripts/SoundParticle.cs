using UnityEngine;

public class SoundParticle : MonoBehaviour
{
    public float speed = 3f;
    public float lifetime = 2f;
    public float fadeSpeed = 1.5f;
    [HideInInspector] 
    public Vector2 originPoint;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color startColor;
    private float timer = 0.1f;

    private bool hitWall = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        originPoint = transform.position;

        rb.velocity = transform.up * speed;

        startColor = sr.color;
        startColor.a = 1f;
        sr.color = startColor;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // fade-out
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // destruir após o fade
        if (timer >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("SoundWave"))
            return;
            
        if (gameObject.CompareTag("EnemySound") && other.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Wall"))
        {
            hitWall = true;

            // Para totalmente
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;   // impede futuras movimentações
            rb.simulated = false;    // congela física

            // Gruda na parede mantendo posição atual
            transform.position = transform.position;
        }
    }
}
