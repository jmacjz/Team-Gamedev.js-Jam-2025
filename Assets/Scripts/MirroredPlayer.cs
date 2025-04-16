using UnityEngine;

public class MirroredPlayer : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    bool canJump;
    BoxCollider2D boxCollider;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private LayerMask groundLayer;

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
        rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (IsGrounded(boxCollider))
        {
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, jumpSpeed);
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

}
