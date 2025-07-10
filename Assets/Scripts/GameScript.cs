using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public bool beatLevel;

    public int levelsBeaten = 1;

    public int sceneCount;

    private GameObject player;

    public static GameScript Instance;

    [SerializeField]
    private int currentLevel = 1;

    private bool beatScene = false;

    void Awake()
    {
        currentLevel = 1;
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

        if (beatLevel && currentLevel == 3)
        {

            /*if (scene.name == "Level " + (levelsBeaten + 1))
            {
                levelsBeaten++;
                SaveLevelsBeaten(levelsBeaten);
            }
            
            
            */

            beatLevel = false;
            StartCoroutine(SceneTransition("Levels"));
        }

        else if(beatLevel && currentLevel <= 3)
        {
            if (beatScene == false)
            {
                currentLevel++;
                beatScene = true;
            }

            string sceneName = scene.name.Remove(scene.name.Length - 1, 1) + currentLevel;
            beatLevel = false;
            StartCoroutine(SceneTransition(sceneName));
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

    private IEnumerator SceneTransition(string scene)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        beatScene = false;
    }

}
