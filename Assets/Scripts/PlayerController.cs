using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float stealthSpeed = 1.5f;

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

        // Tocar passos
        if (isMoving)
        {
            if (isStealth)
                sound.PlayStealth();
            else if (isWalking)
                sound.PlayWalk();
            else if (isRunning)
                sound.PlayRun();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * currentSpeed;
    }
}
