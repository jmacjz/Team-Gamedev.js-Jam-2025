using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    float horizontal; 
    float vertical;
    Rigidbody2D rb;
    BoxCollider2D boxCollider; 


    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;

    public LayerMask groundLayer;

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

            if (isGrounded(boxCollider) && !isGrounded(mirrorCollider))
            {
                mirrorRb.linearVelocity = new Vector2(horizontal * -speed, -8);
            }
        }
            

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded(boxCollider))
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

    public bool isGrounded(BoxCollider2D boxCollider)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        return hit.collider != null;
    }
}
