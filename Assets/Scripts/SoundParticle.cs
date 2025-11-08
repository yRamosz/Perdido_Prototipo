using UnityEngine;

public class SoundParticle : MonoBehaviour
{
    public float speed = 3f;          // Velocidade da partícula
    public float lifetime = 2f;       // Tempo de vida
    public float fadeSpeed = 1.5f;    // Velocidade do fade-out

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color startColor;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
            rb.velocity = transform.up * speed;

        // Define a cor inicial totalmente opaca
        startColor = sr.color;
        startColor.a = 1f;  // Alpha = 1 = totalmente visível
        sr.color = startColor;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Faz o alpha diminuir com o tempo (fade-out)
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        sr.color = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            alpha // controla a transparência
        );

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
