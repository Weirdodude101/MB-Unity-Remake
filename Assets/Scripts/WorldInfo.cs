using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldInfo : MonoBehaviour
{

    GameManager _gameManager;

    IEnumerator Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        GameObject worldNumber = GameObject.Find("Info HUD/Main/World");
        GameObject livesNumber = GameObject.Find("Info HUD/Main/Lives");
        GameObject mario = GameObject.Find("Mario");

        _gameManager.SetTimeRemaining(0);
        if (_gameManager.GetTimeUp())
        {
            GameObject timeUp = GameObject.Find("Info HUD/Main").transform.Find("Time Up").gameObject;
            Destroy(worldNumber);
            Destroy(livesNumber);
            Destroy(mario);
            timeUp.SetActive(true);
            _gameManager.SetTimeUp(false);

            yield return StartCoroutine("LoadWorldInfo");
        }
        /*else if (_gameManager.GetLives() <= 0)
        {
            GameObject gameOver = GameObject.Find("Info HUD/Main").transform.Find("Game Over").gameObject;
            Destroy(worldNumber);
            Destroy(livesNumber);
            Destroy(mario);
            gameOver.SetActive(true);

            yield return StartCoroutine("LoadTitleScreen");
        }*/
        else
        {
            worldNumber.GetComponent<Text>().text = "WORLD " + _gameManager.GetVisualWorld(_gameManager.GetWorld(), _gameManager.GetLevel());
            livesNumber.GetComponent<Text>().text = "x " + _gameManager.GetLives();

            yield return StartCoroutine("LoadWorldScene");
        }

    }

    private IEnumerator LoadWorldScene()
    {
        yield return new WaitForSeconds(3);
        _gameManager.LoadWorld(_gameManager.GetWorld(), _gameManager.GetLevel());
        _gameManager.SetIsPlaying(true);
    }

    // Loads the title screen.
    /*private IEnumerator LoadTitleScreen()
    {
        yield return new WaitForSeconds(7);

        // Reset the game manager's global game variables.
        _gameManager.ResetGlobals();

        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    } */

    // Loads the world info scene.
    private IEnumerator LoadWorldInfo()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("WorldInfo", LoadSceneMode.Single);

    }
}
