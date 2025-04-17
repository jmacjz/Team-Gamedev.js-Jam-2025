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
    private bool isFacingRight, isWallSliding;

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
        if (canMove)
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (IsGrounded(boxCollider) && canMove)
        {
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, jumpSpeed);
        }
    }

    

    private void WallSlide()
    {
        if (IsWalled(boxCollider) && !IsGrounded(boxCollider))
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
