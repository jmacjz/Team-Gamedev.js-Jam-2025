using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public bool beatLevel;

    public int levelsBeaten = 1;

    private GameObject player;

    public static GameScript Instance;


    void Awake()
    {
        levelsBeaten = LoadLevelsBeaten();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if(scene.name.Contains("Level ") && beatLevel)
        {
          
            if (scene.name == "Level " + (levelsBeaten + 1))
            {
                levelsBeaten++;
                SaveLevelsBeaten(levelsBeaten);
            }
            
            beatLevel = false;
            StartCoroutine(SceneTransition());
            
        }


    }

    void SaveLevelsBeaten(int levelsBeaten)
    {
        PlayerPrefs.SetInt("LevelsBeaten", levelsBeaten);
        PlayerPrefs.Save();
    }

    int LoadLevelsBeaten()
    {
        return PlayerPrefs.GetInt("LevelsBeaten", 0);
    }


    private IEnumerator SceneTransition()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Levels");
    }

}
