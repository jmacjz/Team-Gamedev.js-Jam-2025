using UnityEngine;
using System.Collections;

public class JumpPadScript : MonoBehaviour
{
    float bounce = 20f;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12 && collision.gameObject.GetComponent<Rigidbody2D>().linearVelocityY <= 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocityY = 0;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }

   
}
