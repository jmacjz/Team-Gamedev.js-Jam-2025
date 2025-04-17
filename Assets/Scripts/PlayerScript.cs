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

    private bool inDoor; // boolean for when the player is touching the door/button that completes the level (just calling it door for now)


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
    MirroredPlayer mirrorScript; // script for mirrored player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        mirrorRb = mirroredPlayer.GetComponent<Rigidbody2D>();
        mirrorCollider = mirroredPlayer.GetComponent<BoxCollider2D>();
        mirrorScript = mirroredPlayer.GetComponent<MirroredPlayer>();
        mirroredPlayer.SetActive(false);
        spriteRend = GetComponent<SpriteRenderer>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        WallSlide();
        ProcessWallJump();

        if (dead == false)
            canMove = true;
        else
        {
            canMove = false;
            gameObject.SetActive(false);
        }

        if (!isWallJump)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

            Flip();
        }
        



        if (inDoor && mirrorScript.inDoor == true)
        {
            Debug.Log("You Beat The Level");
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (context.performed)
        {   
            mirrorScript.Jump();
            if (IsGrounded(boxCollider))
            {
                //hold to full jump height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            }
        }

        else if (context.canceled)
        {
            //light tap for short hop
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if (context.performed && wallJumpCount > 0f)
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
            mirrored = true;
            mirroredPlayer.SetActive(true);
            mirroredPlayer.transform.position = transform.position + new Vector3(5, 0, 0);
            
        }

        else if (context.performed && mirrored)
        {
            mirrored = false;
            mirroredPlayer.SetActive(false);
        }
    }

    public void FreezeMirror(InputAction.CallbackContext context)
    {
        if (context.performed && mirrorScript.canMove)
        {
            mirrorScript.canMove = false;
        }

        else if (context.performed && !mirrorScript.canMove)
        {
            mirrorScript.canMove = true;
        }

        
    }

    private void WallSlide()
    {
        if (IsWalled(boxCollider) && !IsGrounded(boxCollider) && horizontal != 0f)
        {
            print("Sliding");
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlidingSpeed));
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
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            inDoor = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            inDoor = false;
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
