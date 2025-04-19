using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private float arrowInterval, projectileSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("ShootArrow", 0f, arrowInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootArrow()
    {
        GameObject projectile = Instantiate(arrow, transform.position + new Vector3(1, 0, 0), transform.rotation);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(projectileSpeed, 0);
        Destroy(projectile, 3);
    }
}
