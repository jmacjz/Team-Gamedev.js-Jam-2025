using UnityEngine;

public class PortalScript : MonoBehaviour
{
    CapsuleCollider2D capsuleCollider;
    [SerializeField] Transform exitPortal;


    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            collision.gameObject.transform.position = exitPortal.position;
        }
    }
}
