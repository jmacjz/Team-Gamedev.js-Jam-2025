using Unity.VisualScripting;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerScript>().TakeDamage(damage);
            collision.GetComponent<MirroredPlayer>().TakeDamage(damage);
        }
        
    }
}
