using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name.StartsWith("model_", StringComparison.Ordinal))
                DontDestroyOnLoad(obj);
        }

        if (GetCurrentSceneName() == "Main")
            LoadScene("enemy_test");
    }


    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadScene("itemblock_test");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadScene("enemy_test");
        }
    }

    void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

}
