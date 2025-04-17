using UnityEngine;

public class MirroredPlayer : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    BoxCollider2D boxCollider;

    [SerializeField]
    private float jumpSpeed, wallSlidingSpeed;
    [SerializeField]
    private LayerMask groundLayer, wallLayer;
    [SerializeField]
    private bool isFacingRight, isWallSliding, isWallJump;

    [SerializeField] private float wallJumpDir, wallJumpTime = 0.2f, wallJumpCount, wallJumpDur = 0.4f; public float jumpingPower = 15f;
    
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    public bool canMove;

    public bool inDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");

        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(IsWalled(boxCollider));
        WallSlide();
        ProcessWallJump();

        if (!isWallJump && canMove)
        {
            if (canMove)
                rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, rb.linearVelocity.y);
            Flip();
        }
        
    }

    public void Jump()
    {
        if (IsGrounded(boxCollider) && canMove)
        {
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, jumpSpeed);
        }

        if (wallJumpCount > 0f)
        {
            isWallJump = true;
            rb.linearVelocity = new Vector2(wallJumpDir * wallJumpingPower.x, wallJumpingPower.y); //jump away from wall
            wallJumpCount = 0;

            // Force Flip
            if (transform.localScale.x != wallJumpDir)
            {
                isFacingRight = !isFacingRight;
                Vector2 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(CancelWallJump), wallJumpDur); // Wall jump + 0.5f -- Jump again = 0.6f
        }
    }

    

    private void WallSlide()
    {
        if (IsWalled(boxCollider) && !IsGrounded(boxCollider))
        {
            print("Mirror Sliding");
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJump = false;
            wallJumpDir = -transform.localScale.x;
            wallJumpCount = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpCount > 0f)
        {
            wallJumpCount -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJump = false;
    }




    private void Flip()
    {
        if (isFacingRight && rb.linearVelocity.x < 0 || !isFacingRight && rb.linearVelocity.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            inDoor = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            inDoor = false;
        }
    }

    public bool IsGrounded(BoxCollider2D boxCollider)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
            return false;

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
