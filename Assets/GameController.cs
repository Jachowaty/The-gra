using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{


    public GameObject player;
    public GameObject gameOverScreen;

    public Vector2 startPosition;

    public static event Action OnReset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerHealth.OnPlayerDeath += GameOverScreen;
        gameOverScreen.SetActive(false);
        startPosition = player.GetComponent<Rigidbody2D>().position;
    }

    public void resetGame()
    {
        gameOverScreen.SetActive(false);
        OnReset.Invoke();
        Time.timeScale = 1;
        player.GetComponent<Rigidbody2D>().position = startPosition;
    }
    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
