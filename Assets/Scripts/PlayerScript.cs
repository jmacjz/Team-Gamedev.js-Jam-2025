using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    float horizontal;
    float vertical;
    public bool isFacingRight = true;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;


    [SerializeField] int startingHealth;
    private float currentHealth;
    private bool dead;
    [SerializeField] float invulnDuration;
    [SerializeField] int flashNumber;
    private SpriteRenderer spriteRend;


    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField] private float wallSlidingSpeed = 2f, wallJumpDir, wallJumpTime = 0.2f, wallJumpCount, wallJumpDur = 0.4f; public float jumpingPower = 15f;
    public bool isWallSliding, isWallJump;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    public LayerMask groundLayer, wallLayer, trapLayer;

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
        spriteRend = GetComponent<SpriteRenderer>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        if (dead == false)
            canMove = true;
        else
        {
            canMove = false;
            gameObject.SetActive(false);
        }

        if (canMove)
        {
            mirrorRb.linearVelocity = new Vector2(horizontal * -speed, rb.linearVelocity.y);

            if (IsGrounded(boxCollider) && !IsGrounded(mirrorCollider))
            {
                mirrorRb.linearVelocity = new Vector2(horizontal * -speed, -8);
            }
            WallSlide();

            Flip();
        }
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

    public IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(0, 9, true);
        //So the player cannot be harmed by traps

        for (int i = 0; i < flashNumber; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(invulnDuration / (flashNumber * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(invulnDuration / (flashNumber * 2));
        }
        Physics2D.IgnoreLayerCollision(0, 9, false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
            StartCoroutine(Invulnerability());
        else
        {
            if (!dead)
                dead = true;
        }
            
    }

    public void Heal(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
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
