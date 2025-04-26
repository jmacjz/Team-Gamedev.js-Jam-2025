using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private int level;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Levels()
    {
        SceneManager.LoadScene("Levels");
    }

    public void Level()
    {
        SceneManager.LoadScene("Level " + level);
    }
   
    public void StartScene()
    {
        SceneManager.LoadScene("Start");
    }

    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }
}
