using System.Collections;
using UnityEngine;

public class MirroredPlayer : MonoBehaviour
{
    PlayerScript playerScript;
    GameObject player;
    public Rigidbody2D rb { get; private set; }
    Rigidbody2D playerRb;
    BoxCollider2D boxCollider;

    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private LayerMask groundLayer, wallLayer, trapLayer, ladderLayer;
    [SerializeField]
    private bool isFacingRight, isWallSliding, isWallJump, isClimbing;
    
    private float climbSpeed = 10f;

    public bool canMove, canJump, hitBoss;

    public bool inDoor;

    private bool onMovingPlatform;
    private GameObject movingPlatform;

    [SerializeField] int startingHealth;
    [SerializeField] private float currentHealth;
    private bool dead;
    [SerializeField] float invulnDuration;
    [SerializeField] int flashNumber;
    private SpriteRenderer spriteRend;
    
    public Transform spawnPoint;

    [SerializeField]
    private AudioClip jumpSound, deathSound;

    private bool canVaryJump;

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
        playerScript = player.GetComponent<PlayerScript>();
        currentHealth = startingHealth;
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead == true)
        {
            DespawnPlayer();
            StartCoroutine(RespawnPlayer());
        }

        
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(17, 16);
            if (rb.linearVelocity.y != 0)
                canJump = false;
        }
        if (rb.linearVelocity.y < 0)
        {
            Physics2D.IgnoreLayerCollision(17, 16, false);
            canJump = true;
        }

        if (canMove)
        {
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, rb.linearVelocity.y);

            Flip();
        }

        if (!canVaryJump && IsGrounded(boxCollider))
        {
            canVaryJump = true;
        }

        RaycastHit2D ladderCheck = Physics2D.Raycast(transform.position, Vector2.up, 0.5f, ladderLayer);

        if (ladderCheck.collider != null && playerScript.vertical != 0)
            isClimbing = true;
        else if (Input.GetKey(KeyCode.Space) || ladderCheck.collider == null || IsGrounded(boxCollider))
            isClimbing = false;

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, playerScript.vertical * climbSpeed);
            rb.gravityScale = 0;
        }
        else
            rb.gravityScale = 3;
    }

    public void Jump()
    {
        if (IsGrounded(boxCollider) && canMove && canJump)
        {
            if(SoundManager.instance != null)
                SoundManager.instance.PlaySound(jumpSound);
            rb.linearVelocity = new Vector2(-playerRb.linearVelocity.x, jumpSpeed);
        }
    }

    public void ReleaseJump()
    {
        if(!IsGrounded(boxCollider) && canVaryJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
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
        if (col.gameObject.layer == 14)
        {
            hitBoss = true;
        }

        if (col.gameObject.name.Contains("JumpPad"))
        {
            canVaryJump = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            inDoor = false;
        }
        if (col.gameObject.layer == 14)
        {
            hitBoss = false;
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

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("Platform"))
        {
            if (playerScript.vertical < 0)
            {
                StartCoroutine(DisablePlatformCollider(col.gameObject.GetComponent<BoxCollider2D>()));
            }
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

    public IEnumerator DisablePlatformCollider(BoxCollider2D platformCollider)
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        platformCollider.enabled = true;
    }

}
