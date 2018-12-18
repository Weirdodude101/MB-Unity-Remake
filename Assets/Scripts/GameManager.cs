using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();



    private GameObject _player;
    private GameObject _scoreVisual;
    private GameObject _coinsVisual;
    private GameObject _worldVisual;
    private GameObject _timerVisual;

    private string _world;
    private int _lives;
    private int _score;
    private int _coins;
    private int _timeRemaining;
    private int _timeAvailable;
    private bool _timeRunning;


    void Start()
    {
        _player = GameObject.Find("Player");
        _scoreVisual = GameObject.Find("Top_HUD/Main/MARIO/score");
        _coinsVisual = GameObject.Find("Top_HUD/Main/COINS");
        _worldVisual = GameObject.Find("Top_HUD/Main/WORLD/world-level");
        _timerVisual = GameObject.Find("Top_HUD/Main/TIME/time");


        Setup();
    }


    public void Setup()
    {

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("Top_HUD"));
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name.StartsWith("model_", StringComparison.Ordinal))
                DontDestroyOnLoad(obj);
        }

        if (GetCurrentSceneName() == "Main")
            LoadScene("enemy_test");

        Physics2D.IgnoreLayerCollision(8, 9, true);

        if (!_timeRunning)
        {
            SetTimeRemaining(15);

            StartCoroutine(Timer());
            _timeRunning = true;
        }
    }

    public void SetTimeAvailable(int timeAvailable) {
        _timeAvailable = timeAvailable;
    }

    public int GetTimeAvailable() {
        return _timeAvailable;
    }

    public string GetVisualTimeRemaining(int timeRemaining) {
        string result = "";

        string remaining = timeRemaining.ToString();

        int zeros = 3 - remaining.Length;

        for (int i = 0; i < zeros; i++)
        {
            result += "0";
        }
        result += remaining;

        return result;
    }

    public void SetTimeRemaining(int timeRemaining) {
        _timeRemaining = timeRemaining;
    }

    public int GetTimeRemaining() {
        return _timeRemaining;
    }
    
    IEnumerator Timer() {
        while (GetTimeRemaining() > 0)
        {
            yield return new WaitForSeconds(0.4f);
            SetTimeRemaining(GetTimeRemaining() - 1);

        }

        if (GetTimeRemaining() <= 0)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().HandlePlayerDeath();
        }

    }
    
    void FixedUpdate()
    {
        _timerVisual.GetComponent<Text>().text = GetVisualTimeRemaining(GetTimeRemaining());

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
