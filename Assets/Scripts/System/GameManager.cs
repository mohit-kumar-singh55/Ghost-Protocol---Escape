using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += OnGameOver;
        // GhostHealth.OnPlayerWin += GoNextLevel;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= OnGameOver;
        // GhostHealth.OnPlayerWin -= GoNextLevel;
    }

    void OnGameOver()
    {
        // 今のレベルを再プレイする
        PlayerPrefs.SetInt(PLAYER_PREFS.LAST_PLAYING_LEVEL, SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SCENES.GAME_OVER);
    }

    void GoNextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        index = index == SCENES.GAME_OVER ? SCENES.GAME_CLEAR : index;
        SceneManager.LoadScene(index);
    }
}