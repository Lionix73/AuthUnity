using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float maxJumpTime = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private bool isJumping = false;
    private float jumpTimeCounter;
    private bool isGrounded;
    private Rigidbody2D rb;

    private PlayerInput playerInput;
    private InputAction jumpAction;

    private GameManager gameManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];

        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnEnable()
    {
        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;
    }

    void OnDisable()
    {
        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;
    }

    void Update()
    {

        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
        if (isJumping && jumpTimeCounter < maxJumpTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpTimeCounter += Time.deltaTime;
        }
        else if (!isJumping){
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.GameOver();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
