using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    float horizontal; 
    float vertical;
    public bool isFacingRight = true;
    Rigidbody2D rb;
    BoxCollider2D boxCollider; 


    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField] private float wallSlidingSpeed = 2f, wallJumpDir, wallJumpTime = 0.2f, wallJumpCount, wallJumpDur = 0.4f; public float jumpingPower = 15f;
    public bool isWallSliding, isWallJump;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    public LayerMask groundLayer, wallLayer;

    public GameObject mirroredPlayer;

    Rigidbody2D mirrorRb;

    public bool mirrored = false;
    public bool canMove;
    BoxCollider2D mirrorCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        mirrorRb = mirroredPlayer.GetComponent<Rigidbody2D>();
        mirrorCollider = mirroredPlayer.GetComponent<BoxCollider2D>();
        mirroredPlayer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (canMove)
        {
            mirrorRb.linearVelocity = new Vector2(horizontal * -speed, rb.linearVelocity.y);

            if (IsGrounded(boxCollider) && !IsGrounded(mirrorCollider))
            {
                mirrorRb.linearVelocity = new Vector2(horizontal * -speed, -8);
            }
        }
        WallSlide();

        Flip();

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded(boxCollider))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    public void Mirror(InputAction.CallbackContext context)
    {
        if (context.performed && !mirrored)
        {
            Debug.Log("mirror");
            mirrored = true;
            mirroredPlayer.SetActive(true);
            mirroredPlayer.transform.position = transform.position + new Vector3(5, 0, 0);
            
        }

        else if (context.performed && mirrored)
        {
            Debug.Log("not ");
            mirrored = false;
            mirroredPlayer.SetActive(false);
        }
    }

    public void FreezeMirror(InputAction.CallbackContext context)
    {
        if (context.performed && canMove)
        {
            canMove = false;
        }

        else if (context.performed && !canMove)
        {
            canMove = true;
        }

        
    }

    private void WallSlide()
    {
        if (IsWalled(boxCollider) && !IsGrounded(boxCollider) && horizontal != 0f)
        {
            print("Sliding");
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public bool IsGrounded(BoxCollider2D boxCollider)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        if (hit.collider != null)
        {
            print("Grounded");
            return true;
        }
        else
        {
            print("Not Grounded");
            return false;
        }
    }

    public bool IsWalled(BoxCollider2D boxCollider)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, transform.localScale, 0.05f, wallLayer);
        if (hit.collider != null)
        {
            print("Walled");
            return true;
        }
        else
        {
            return false;
        }
    }
}
