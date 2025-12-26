using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += OnGameOver;
        // GhostHealth.OnPlayerWin += GoNextLevel;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= OnGameOver;
        // GhostHealth.OnPlayerWin -= GoNextLevel;
    }

    private void OnGameOver()
    {
        // 今のレベルを再プレイする
        PlayerPrefs.SetInt(PLAYER_PREFS.LAST_PLAYING_LEVEL, SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SCENES.GAME_OVER);
    }

    private void GoNextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        // index = index == SCENES.GAME_OVER ? SCENES.GAME_CLEAR : index;
        SceneManager.LoadScene(index);
    }
}