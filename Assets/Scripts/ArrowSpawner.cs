using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private float arrowInterval, projectileSpeed, lifetime;

    private int offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("ShootArrow", 1f, arrowInterval);
        if (projectileSpeed < 0)
            offset = -1;
        else 
            offset = 1;
    }

    public void ShootArrow()
    {
        GameObject projectile = Instantiate(arrow, transform.position + new Vector3(offset, 0, 0), Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(projectileSpeed, 0);
        Destroy(projectile, lifetime);
    }
}
