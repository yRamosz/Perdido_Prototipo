using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private float currentSpeed;

    public GameObject soundWavePrefab;

    private PlayerSound sound;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isWalking;
    private bool isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sound = GetComponent<PlayerSound>();
    }

    void Update()
    {
        // Entrada de movimento
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Shift ativa corrida
        bool shift = Input.GetKey(KeyCode.LeftShift);

        // Detecta se está movendo
        bool isMoving = moveInput.magnitude > 0.1f;

        // Estados
        isRunning = isMoving && shift;
        isWalking = isMoving && !shift;

        // Escolhe velocidade
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Criar onda manual (tecla espaço)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateSoundWave();
        }

        // Sons (agora SÓ os sons controlam as ondas!!)
        if (isMoving)
        {
            if (isWalking)
                sound.PlayWalk();

            if (isRunning)
                sound.PlayRun();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * currentSpeed;
    }

    public void CreateSoundWave()
    {
        GameObject emitter = Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
        emitter.GetComponent<SoundWaveEmitter>().Emit();
    }
}
