using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float stealthSpeed = 0.1f;

    private float currentSpeed;

    private PlayerSound sound;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isWalking;
    private bool isRunning;
    public bool isStealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sound = GetComponent<PlayerSound>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        // Shift ativa corrida
        bool shiftLeft = Input.GetKey(KeyCode.LeftShift);
        bool shiftRight = Input.GetKey(KeyCode.RightShift);

        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool ctrl = Input.GetKey(KeyCode.LeftControl);

        bool isMoving = moveInput.magnitude > 0.1f;
        // Estados
        isRunning = isMoving && shiftLeft || shiftRight;
        isWalking = isMoving && !shiftLeft || !shiftRight;

        isStealth = ctrl;
        isRunning = isMoving && shift && !ctrl;
        isWalking = isMoving && !shift && !ctrl;

        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool ctrl = Input.GetKey(KeyCode.LeftControl);

        bool isMoving = moveInput.magnitude > 0.1f;

        isStealth = ctrl;
        isRunning = isMoving && shift && !ctrl;
        isWalking = isMoving && !shift && !ctrl;

        if (isStealth)
            currentSpeed = stealthSpeed;
        else
            currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isMoving)
        {
            if (isStealth)
                sound.PlayStealth();
            else if (isWalking)
                sound.PlayWalk();
            else if (isRunning)
                sound.PlayRun();
        }

        if (Input.GetKeyDown(KeyCode.Space))
{
    sound.ForceEmit(isStealth); 
}
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Morreu!");
            
            FindObjectOfType<GameManager>().TriggerGameOver();
            
        }
    }

}
