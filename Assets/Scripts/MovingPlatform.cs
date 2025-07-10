using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public int startingPoint;
    public Transform[] points;

    private int i; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(gameObject.activeInHierarchy)
            col.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (gameObject.activeInHierarchy)
            col.transform.SetParent(null);
    }
 

}
