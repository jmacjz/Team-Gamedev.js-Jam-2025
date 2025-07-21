using Unity.VisualScripting;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Destroy(gameObject);
        }
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerScript>().TakeDamage(damage);
            GameObject.Find("Mirrored Player").GetComponent<MirroredPlayer>().TakeDamage(damage);

        }

        else if (collision.tag == "Mirrored Player")
        {
            collision.GetComponent<MirroredPlayer>().TakeDamage(damage);
            GameObject.Find("Player").GetComponent<PlayerScript>().TakeDamage(damage);
        }

        

    }
}
