using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();
    protected readonly Dictionary<string, int> level2Time = new Dictionary<string, int>
    {
        {"1-1", 900},
        {"1-2", 300},
    };


    private GameObject _player;
    private GameObject _scoreVisual;
    private GameObject _coinsVisual;
    private GameObject _worldVisual;
    private GameObject _timerVisual;

    private int _world = 0;
    private int _level = 8;
    private int _lives;
    private int _score;
    private int _coins;
    private int _timeRemaining;
    private bool _timeRunning;

    private IEnumerator _timerCo;

    void Start()
    {
        _player = GameObject.Find("Player");
        _scoreVisual = GameObject.Find("Top_HUD/Main/MARIO/score");
        _coinsVisual = GameObject.Find("Top_HUD/Main/COINS");
        _worldVisual = GameObject.Find("Top_HUD/Main/WORLD/world-level");
        _timerVisual = GameObject.Find("Top_HUD/Main/TIME/time");

        _timerCo = Timer();

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

        Physics2D.IgnoreLayerCollision(8, 9, true);

        LoadNextWorld();

    }

    public void SetupTimer()
    {
        if (!_timeRunning)
        {
            SetTimeRemaining(level2Time[GetVisualWorld(GetWorld(), GetLevel())]);

            _timerCo = Timer();
            StartCoroutine(_timerCo);

            _timeRunning = true;
        }
    }

    public void ManageHUD()
    {
        _timerVisual.GetComponent<Text>().text = GetVisualTimeRemaining(GetTimeRemaining());
        _worldVisual.GetComponent<Text>().text = GetVisualWorld(GetWorld(), GetLevel());
    }

    public void LoadNextWorld()
    {
        if (_timeRunning)
        {
            _timeRunning = false;
            StopCoroutine(_timerCo);
        }

        if (GetLevel() + 1 > 8)
        {
            SetWorld(GetWorld() + 1);
            SetLevel(0);
        }
        SetLevel(GetLevel() + 1);

        LoadScene(String.Format("Scenes/{0}/{1}", GetWorld(), GetLevel()));

        SetupTimer();
    }

    public string GetVisualWorld(int world, int level)
    {
        return world + "-" + level;
    }

    public void SetWorld(int world)
    {
        _world = world;
    }

    public int GetWorld() {
        return _world;
    }

    public void SetLevel(int level)
    {
        _level = level;
    }

    public int GetLevel()
    {
        return _level;
    }

    public string GetVisualTimeRemaining(int timeRemaining)
    {
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

    public void SetTimeRemaining(int timeRemaining)
    {
        _timeRemaining = timeRemaining;
    }

    public int GetTimeRemaining()
    {
        return _timeRemaining;
    }
    
    IEnumerator Timer()
    {

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

        ManageHUD();

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
