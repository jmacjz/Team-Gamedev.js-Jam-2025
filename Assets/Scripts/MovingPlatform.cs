using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float moveTime; // move time is how long the platform moves in one direction

    public float moveSpeed;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SwitchDirection", 0f, moveTime);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(moveSpeed, 0);
    }

    private void SwitchDirection()
    {
        moveSpeed = moveSpeed * -1;
    }

 

}
