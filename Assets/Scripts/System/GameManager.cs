using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Fader _fader;

    void Start()
    {
        _fader = FindAnyObjectByType<Fader>();

        if (!_fader)
        {
            Debug.LogError("GameManager: Fader component is missing in the scene!");
            enabled = false;
            return;
        }

        // フェードインでタイトル画面表示
        _fader.FadeInScreen();

        // カーソルを非表示にする
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += OnGameOver;
        ExitDoor.OnPlayerWinLevel += GoNextLevel;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= OnGameOver;
        ExitDoor.OnPlayerWinLevel -= GoNextLevel;
    }

    private void OnGameOver()
    {
        // 今のレベルを再プレイする
        PlayerPrefs.SetInt(PLAYER_PREFS.LAST_PLAYING_LEVEL, SceneManager.GetActiveScene().buildIndex);
        _fader.FadeOutScreen(() => SceneManager.LoadScene(SCENES.GAME_OVER));
    }

    // will also be called on from timeline signal of entry and exit cutscenes
    public void GoNextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        // index = index == SCENES.GAME_OVER ? SCENES.GAME_CLEAR : index;
        _fader.FadeOutScreen(() => SceneManager.LoadScene(index));
    }
}