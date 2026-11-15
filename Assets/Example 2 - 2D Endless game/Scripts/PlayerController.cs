using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundedRaycastStartPoint;

    [Header("Player Stats")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 9f;
    [SerializeField] float gravityScale = 1.3f;
    [SerializeField] float groundCheckDistance = 0.6f;
    [SerializeField] int maxWallJumps = 2;
    [Tooltip("Set it to ignore the player layer")]
    [SerializeField] LayerMask groundedCheckLayerMask;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip jumpSound;
    [Range(0f, 1f)] [SerializeField] float jumpVolume = 0.3f;

    bool canJump;
    bool wasTouchingLastFrame;
    int wallJumpsLeft;

    void Start()
    {
        rb.freezeRotation = true;
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        if (gameManager.IsGameOver || Time.timeScale == 0f) return;

        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        bool onGround = IsTouching(Vector2.down);
        bool onWall = IsTouching(Vector2.left) || IsTouching(Vector2.right);
        bool touching = (onGround || onWall) && rb.linearVelocity.y <= 0.1f;
        if (touching && !wasTouchingLastFrame)
        {
            if (onGround)
            {
                canJump = true;
                wallJumpsLeft = maxWallJumps;
            }
            else if (wallJumpsLeft > 0)
            {
                canJump = true;
                wallJumpsLeft--;
            }
        }
        wasTouchingLastFrame = touching;

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canJump = false;
            audioSource.PlayOneShot(jumpSound, jumpVolume);
        }
    }

    bool IsTouching(Vector2 direction)
    {
        Vector2 boxSize = new Vector2(0.7f, 0.7f);
        float distance = groundCheckDistance - boxSize.x / 2f;
        RaycastHit2D hit = Physics2D.BoxCast(groundedRaycastStartPoint.position, boxSize, 0f, direction, distance, groundedCheckLayerMask);
        return hit && hit.collider.GetComponent<Obstacle>() == null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Obstacle>() != null)
        {
            Die();
        }
    }

    // Called by KillZone (a trigger collider) when the player falls into it.
    public void Die()
    {
        // add a small rotation force to show loss of control
        rb.freezeRotation = false;
        rb.AddTorque(25f);
        gameManager.OnPlayerCollidedWithObstacle();
    }

    // Displays the grounded raycasts for debug purposes
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundedRaycastStartPoint.position, Vector2.left * groundCheckDistance);
        Gizmos.DrawRay(groundedRaycastStartPoint.position, Vector2.right * groundCheckDistance);
        Gizmos.DrawRay(groundedRaycastStartPoint.position, Vector2.down * groundCheckDistance);
    }
}
