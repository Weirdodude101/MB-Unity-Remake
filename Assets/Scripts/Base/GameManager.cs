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
        {"1-1", 400},
        {"1-2", 400},
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
    private bool _isPlaying;
    private bool _timeUp;
    private IEnumerator _timerCo;

    public enum PlayerStates {Small, Big, Fire}
    public PlayerStates playerState;

    void Start()
    {
        ResetGlobals();

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

        DontDestroyOnLoad(GameObject.Find("Coin"));
        GameObject.Find("Coin").GetComponent<SpriteRenderer>().enabled = false;

        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name.StartsWith("model_", StringComparison.Ordinal))
                DontDestroyOnLoad(obj);
        }

        Physics2D.IgnoreLayerCollision(8, 9, true);

        LoadScene("WorldInfo");

    }

    public void SetupTimer()
    {
        if (GetIsPlaying())
        {
            if (!_timeRunning)
            {
                SetTimeRemaining(level2Time[GetVisualWorld(GetWorld(), GetLevel())]);

                _timerCo = Timer();
                StartCoroutine(_timerCo);

                _timeRunning = true;
            }
        }
    }

    public void ManageHUD()
    {
        _timerVisual.GetComponent<Text>().text = GetVisualTimeRemaining(GetTimeRemaining());
        _worldVisual.GetComponent<Text>().text = GetVisualWorld(GetWorld(), GetLevel());
        _coinsVisual.GetComponent<Text>().text = GetVisualCoins(GetCoins());
        _scoreVisual.GetComponent<Text>().text = GetVisualScore(GetScore());
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void SetPlayerState(PlayerStates state)
    {
        playerState = state;
    }

    public PlayerStates GetPlayerState()
    {
        return playerState;
    }

    public void SetIsPlaying(bool isPlaying)
    {
        _isPlaying = isPlaying;
    }

    public bool GetIsPlaying()
    {
        return _isPlaying;
    }

    public string GetVisualZeros(int len, int data)
    {

        string result = "";

        string dataStr = data.ToString();

        int zeros = len - dataStr.Length;

        for (int i = 0; i < zeros; i++)
        {
            result += "0";
        }

        return result;
    }

    public void IncrementLives()
    {
        SetLives(GetLives() + 1);
    }

    public void DecrementLives()
    {
        SetLives(GetLives() - 1);

        if (_timeRunning)
        {
            _timeRunning = false;
            StopCoroutine(_timerCo);
        }
        LoadScene("WorldInfo");
    }

    public void SetLives(int lives)
    {
        _lives = lives;
    }

    public int GetLives()
    {
        return _lives;
    }

    public void IncrementScore(int amount)
    {
        SetScore(GetScore() + amount);
    }

    public string GetVisualScore(int score)
    {
        string zeros = GetVisualZeros(6, score);

        return zeros + score;
    }

    public void SetScore(int score)
    {
        _score = score;
    }

    public int GetScore()
    {
        return _score;
    }

    public void IncrementCoins()
    {
        if (GetCoins() >= 99)
        {
            SetCoins(0);
            IncrementLives();
        }
        else
        {

            SetCoins(GetCoins() + 1);
        }
    }

    public string GetVisualCoins(int coins)
    {
        string zeros = GetVisualZeros(2, coins);

        return "x" + zeros + coins;
    }

    public void SetCoins(int coins)
    {
        _coins = coins;
    }

    public int GetCoins()
    {
        return _coins;
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
        LoadScene("WorldInfo");
    }

    public void LoadWorld(int world, int level)
    {
        if (GetWorld() != world)
            SetWorld(world);

        if (GetLevel() != level)
             SetLevel(level);

        LoadScene(String.Format("Scenes/worlds/{0}-{1}", world, level));
        SetIsPlaying(true);
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
        string zeros = GetVisualZeros(3, timeRemaining);

        return zeros += timeRemaining;
    }

    public void SetTimeRemaining(int timeRemaining)
    {
        _timeRemaining = timeRemaining;
    }

    public int GetTimeRemaining()
    {
        return _timeRemaining;
    }

    public void SetTimeUp(bool timeUp)
    {
        _timeUp = timeUp;
    }

    public bool GetTimeUp()
    {
        return _timeUp;
    }
    
    IEnumerator Timer()
    {

        while (GetTimeRemaining() > 0)
        {
            yield return new WaitForSeconds(0.4f);
            SetTimeRemaining(GetTimeRemaining() - 1);
            if (GameObject.Find("Player").GetComponent<PlayerController>().IsDead())
            {
                StopCoroutine(_timerCo);
            }

        }

        if (GetTimeRemaining() <= 0)
        {
            SetTimeUp(true);
            GameObject.Find("Player").GetComponent<PlayerController>().HandlePlayerDeath();
        }

    }

    public void ResetGlobals(int world = 1, int level = 1)
    {
        SetCoins(0);
         
        SetWorld(world);
        SetLevel(level);
        SetLives(3);
        SetScore(0);

        SetIsPlaying(false);
        SetTimeRemaining(0);

    }
    
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!(GetLevel() + 1 > 2))
                LoadNextWorld();
        }

        ManageHUD();

    }

    public void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

}
