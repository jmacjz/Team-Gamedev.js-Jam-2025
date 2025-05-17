using UnityEngine;

public class HubDoor : MonoBehaviour
{
    public bool locked = true;
    public GameScript gameScript;
    public int level;
    public Color unlockedColor;
    public Color lockedColor;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameScript = GameObject.Find("GameManager").GetComponent<GameScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameScript.levelsBeaten + 1 >= level)
        {
            locked = false;
            spriteRenderer.color = unlockedColor;
        }
        else
        {
            locked = true;
            spriteRenderer.color = lockedColor;
        }

       
    }
}
