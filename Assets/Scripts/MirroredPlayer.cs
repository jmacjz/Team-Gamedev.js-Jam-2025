using System.Collections;
using UnityEngine;

public class MirroredPlayer : MonoBehaviour
{
    PlayerScript playerScript;
    GameObject player;
    Rigidbody2D rb;
    Rigidbody2D playerRb;
    BoxCollider2D boxCollider;

    [SerializeField]
    private float jumpSpeed, wallSlidingSpeed;
    [SerializeField]
    private LayerMask groundLayer, wallLayer, trapLayer;
    [SerializeField]
    private bool isFacingRight, isWallSliding, isWallJump;

    [SerializeField] private float wallJumpDir, wallJumpTime = 0.2f, wallJumpCount, wallJumpDur = 0.4f; public float jumpingPower = 15f;
    
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    public bool canMove, canJump;

    public bool inDoor;

    private bool onMovingPlatform;
    private GameObject movingPlatform;

    [SerializeField] int startingHealth;
    [SerializeField] private float currentHealth;
    private bool dead;
    [SerializeField] float invulnDuration;
    [SerializeField] int flashNumber;
    private SpriteRenderer spriteRend;
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private AudioClip jumpSound, deathSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        movingPlatform = GameObject.Find("Moving Platform");
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        canMove = true;
        canJump = true;
        playerScript = GetComponent<PlayerScript>();
        currentHealth = startingHealth;
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        WallSlide();
        ProcessWallJump();

        if (dead == true)
        {
            DespawnPlayer();
            StartCoroutine(RespawnPlayer());
        }

        if (!isWallJump && canMove)
        {

            if (!onMovingPlatform)
            {
                if(player.GetComponent<PlayerScript>().onMovingPlatform == true)
                    rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x + movingPlatform.GetComponent<MovingPlatform>().moveSpeed, rb.linearVelocity.y);               
                else
                    rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, rb.linearVelocity.y);
            }
                    
            else
            {
                rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x + movingPlatform.GetComponent<MovingPlatform>().moveSpeed, rb.linearVelocity.y);
            }
            
                
            Flip();
        }
        
    }

    public void Jump()
    {
        if (IsGrounded(boxCollider) && canMove)
        {
            //SoundManager.instance.PlaySound(jumpSound);
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

    public void DespawnPlayer()
    {
        if (dead == true)
        {
            boxCollider.enabled = false;
            canMove = false;
            canJump = false;
            spriteRend.enabled = false;
        }
        dead = false;
    }

    public IEnumerator RespawnPlayer()
    {
        if (dead == false)
        {
            yield return new WaitForSeconds(1);
            gameObject.transform.position = spawnPoint.position;
            boxCollider.enabled = true;
            spriteRend.enabled = true;
            canJump = true;
            canMove = true;
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("Moving Platform"))
        {
            movingPlatform = col.gameObject;
            onMovingPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("Moving Platform"))
        {
            onMovingPlatform = false;
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
            print("Mirror Walled");
            return true;
        }
        else
        {
            return false;
        }
    }

}
