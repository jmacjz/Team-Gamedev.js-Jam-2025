using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    float horizontal;
    public float vertical { get; private set; }
    public bool isFacingRight = true;
    public Rigidbody2D rb { get; private set; }
    BoxCollider2D boxCollider;
    [SerializeField] Transform spawnPoint;


    [SerializeField] int startingHealth;
    [SerializeField] private float currentHealth;
    private bool dead = false;
    [SerializeField] float invulnDuration;
    [SerializeField] int flashNumber;
    private SpriteRenderer spriteRend;

    private bool inDoor; // boolean for when the player is touching the door that completes the level 
    private bool beatLevel = false;
    public static bool paused;
    [SerializeField] private GameObject pauseMenuUI;


    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;
    public bool isClimbing;
    private float climbSpeed = 10f;

    public LayerMask groundLayer, trapLayer, ladderLayer;

    public GameObject mirroredPlayer;

    Rigidbody2D mirrorRb;

    public bool mirrored = false;
    public bool canMove, canJump;
    BoxCollider2D mirrorCollider;
    MirroredPlayer mirrorScript; // script for mirrored player

    public bool onMovingPlatform;
    private GameObject movingPlatform;

    [SerializeField]
    private float coyoteTime = 0.2f, coyoteTimeCount;

    [SerializeField]
    private float jumpBufferTime = 0.2f, jumpBufferCount;

    [SerializeField]
    private AudioClip jumpSound, deathSound;
    [SerializeField]
    private bool interacting, canRestart, canChangeLevel, onHubDoor;
    private GameObject hubDoor = null;

    [SerializeField]
    private GameObject gameManager;

    public bool canVaryJump;

    void Awake()
    {
        if (GameObject.Find("GameManager") == null)
        {
            gameManager = Instantiate(gameManager, transform.position, transform.rotation);
            gameManager.name = "GameManager";
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paused = false;
        pauseMenuUI.SetActive(false);
        canMove = true;
        canJump = true;

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        mirrorRb = mirroredPlayer.GetComponent<Rigidbody2D>();
        mirrorCollider = mirroredPlayer.GetComponent<BoxCollider2D>();
        mirrorScript = mirroredPlayer.GetComponent<MirroredPlayer>();
        spriteRend = GetComponent<SpriteRenderer>();
        currentHealth = startingHealth;

        
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Levels")
        {
            canRestart = false;
        }
        else
        {
            canRestart = true;
        }

        if (onHubDoor && hubDoor != null && hubDoor.GetComponent<HubDoor>().locked == false && interacting)
        {
            StartCoroutine(ChangeScene(hubDoor.GetComponent<HubDoor>().level));
        }
        
        if (dead == true)
        {
            DespawnPlayer();
            StartCoroutine(RespawnPlayer());
        }

        

        if (inDoor && mirrorScript.inDoor == true && !beatLevel)
        {
            if (GameObject.Find("GameManager") != null)
            {
                GameScript gameScript = GameObject.Find("GameManager").GetComponent<GameScript>();
                gameScript.beatLevel = true;
                beatLevel = true;
            }

        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            if (IsGrounded())
            {
                coyoteTimeCount = coyoteTime;
            }
            else
            {
                coyoteTimeCount -= Time.deltaTime;
            }



            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);


            Flip();
        }

        if (!canVaryJump && IsGrounded())
        {
            canVaryJump = true;
        }

        if (rb.linearVelocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(12, 16);
            if (rb.linearVelocity.y != 0)
                canJump = false;
        }
        if (rb.linearVelocity.y < 0)
        {
            Physics2D.IgnoreLayerCollision(12, 16, false);
            canJump = true;
        }

        RaycastHit2D ladderCheck = Physics2D.Raycast(transform.position, Vector2.up, 0.5f, ladderLayer);

        if (ladderCheck.collider != null && vertical != 0)
            isClimbing = true;
        else if (Input.GetKey(KeyCode.Space) || ladderCheck.collider == null || IsGrounded())
            isClimbing = false;

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, vertical * climbSpeed);
            rb.gravityScale = 0;
        }
        else
            rb.gravityScale = 3;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (paused == false && context.performed)
        {
            Time.timeScale = 0;
            paused = true;
            pauseMenuUI.SetActive(true);
        }

        else if (paused == true && context.performed)
        {
            Time.timeScale = 1;
            paused = false;
            pauseMenuUI.SetActive(false);
        }
    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;

    }

    public void Jump(InputAction.CallbackContext context)
    {

        // jump
        if (context.performed)
        {
            if(canJump)
                jumpBufferCount = jumpBufferTime;
            mirrorScript.Jump();
        }
        else
        {
            print("Jump Buffer Else");
            jumpBufferCount -= Time.deltaTime;
        }

        if (jumpBufferCount > 0f && coyoteTimeCount > 0f && canJump)
        {

            if(SoundManager.instance != null)
                SoundManager.instance.PlaySound(jumpSound);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            jumpBufferCount = 0f;
        }

            
        

        if (context.canceled)
        {
            // short hop mechanic
            mirrorScript.ReleaseJump();
            if (canVaryJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                coyoteTimeCount = 0f;
            }
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
            canMove = false;
            canJump = false;
            spriteRend.enabled = false;

            DeleteTraps("Arrow");
            DeleteTraps("ThrowableSpike");
        }
        dead = false;
    }

    public IEnumerator RespawnPlayer()
    {
        if (dead == false)
        {
            yield return new WaitForSeconds(1);
            gameObject.transform.position = spawnPoint.position;
            spriteRend.enabled = true;
            canJump = true;
            canMove = true;
        }
    }

    public void Reset(InputAction.CallbackContext context)
    {
        if (context.performed && canRestart)
        {
            StartCoroutine(MoveToSpawn(0));
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interacting = true;
        }

        else
        {
            interacting = false;
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

    private void DeleteTraps(string trapName)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Traps");

        foreach (GameObject trap in gameObjects)
        {
            if (trap.name.Contains(trapName))
            {
                Destroy(trap);
            }
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

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("Platform"))
        {
            if (vertical < 0)
            {
                StartCoroutine(DisablePlatformCollider(col.gameObject.GetComponent<BoxCollider2D>()));
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name.Contains("Moving Platform"))
        {
            onMovingPlatform = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            inDoor = true;
        }
        if (col.gameObject.layer == 13)
        {
            onHubDoor = true;
            hubDoor = col.gameObject;
        }
        if (col.gameObject.layer == 14)
        {
            if (mirrorScript.hitBoss == true)
            {
                Debug.Log("Hit boss");
            }

            col.gameObject.GetComponent<BossScript>().health--;
            StartCoroutine(MoveToSpawn(1));

        }

        if (col.gameObject.name.Contains("JumpPad"))
        {
            canVaryJump = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 10)
        {
            inDoor = false;
        }

        if (col.gameObject.layer == 13)
        {
            onHubDoor = false;
            hubDoor = null;
        }
        

    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator ChangeScene(int level)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Level " + level);
    }

    public IEnumerator MoveToSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = spawnPoint.position;
        mirroredPlayer.transform.position = mirroredPlayer.GetComponent<MirroredPlayer>().spawnPoint.position;
        DeleteTraps("ThrowableSpike");
    }

    public IEnumerator DisablePlatformCollider(BoxCollider2D platformCollider)
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        platformCollider.enabled = true;
    }

    
}
