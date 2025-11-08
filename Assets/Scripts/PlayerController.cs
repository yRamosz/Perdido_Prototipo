using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject soundWavePrefab; // arrasta o prefab aqui no Inspector
    public float waveSpawnInterval = 0.5f; // tempo entre ondas automáticas
    private float waveTimer = 0f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimento do player
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Se o player apertar espaço → cria uma onda
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateSoundWave();
        }

        // Se o player estiver se movendo → gera ondas periódicas
        if (moveInput.magnitude > 0)
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
            waveTimer = 0f; // reseta o timer quando parar
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    void CreateSoundWave()
{
    GameObject emitter = Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
    emitter.GetComponent<SoundWaveEmitter>().Emit();
}

}
