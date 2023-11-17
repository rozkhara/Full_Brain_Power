using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    public CubeGenerator CubeGenerator;
    public CubeController CubeController;
    public PlaneController PlaneController;

    public bool isPaused { get; private set; }
    public int Difficulty { get; private set; } // 0 for Easy, 1 for Medium, 2 for Hard, -1 for every other scenes

    private void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        _instance = GetComponent<GameManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void GameOver()
    {
        PauseGame();
        Debug.Log("GameOver");
    }

    public void PauseGame()
    {
        Time.timeScale = 0; isPaused = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1; isPaused = false;
    }
    public void SetDifficulty(int _difficulty)
    {
        Difficulty = _difficulty;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
