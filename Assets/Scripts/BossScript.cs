using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField] 
    private float interval = 3f;

    float time;

    [SerializeField]
    private GameObject spike;

    int xDirection = -1;

    public int health;

    private GameObject gameManager;

    bool completeLevel = false;

    void Start()
    {
        time = 0f;
        gameManager = GameObject.Find("GameManager");
    }

    void Update()
    {
        time += Time.deltaTime;
        while (time >= interval)
        {
            ThrowHazards();
            time -= interval;
        }

        if(health <= 0 && !completeLevel)
        {
            CompleteLevel();
        }
    }

    void ThrowHazards()
    {
        xDirection = xDirection * -1;        
        int randomY = Random.Range(200, 500);
        GameObject hazard = Instantiate(spike, transform.position, transform.rotation);
        hazard.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDirection * 500, randomY));
        Destroy(hazard, 3);
    }


    private void CompleteLevel()
    {
        completeLevel = true;
        if(gameManager != null)
        {
            gameManager.GetComponent<GameScript>().beatLevel = true;
        }
    }

}
