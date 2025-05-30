using UnityEngine;

public class ConveyorBeltScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;
    [SerializeField] MirroredPlayer mirrorScript;
    float moveSpeed = 20f;
    [SerializeField] bool facingRight;
    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.gameObject.GetComponent<Rigidbody2D>().linearVelocityY == 0)
        {
            if (facingRight)
                playerScript.rb.AddForce(new Vector2(moveSpeed * 10f, 0f));
            else
                playerScript.rb.AddForce(new Vector2(moveSpeed * -10f, 0f));
        }

        else if (collision.tag == "Mirrored Player" && collision.gameObject.GetComponent<Rigidbody2D>().linearVelocityY == 0)
        {
            if (facingRight)
                mirrorScript.rb.AddForce(new Vector2(moveSpeed * 10f, 0f));
            else
                mirrorScript.rb.AddForce(new Vector2(moveSpeed * -10f, 0f));
        }
    }
}
