using UnityEngine;

public class GameOverController : MonoBehaviour
{
    private Fader _fader;

    void Start()
    {
        _fader = FindAnyObjectByType<Fader>();

        if (!_fader)
        {
            Debug.LogError("GameOverController: Fader component is missing in the scene!");
            enabled = false;
            return;
        }

        // フェードインでゲームオーバー画面表示
        _fader.FadeInScreen();

        // カーソルを表示にする
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartLastLevel()
    {
        int lastPlayingLevel = PlayerPrefs.GetInt(PLAYER_PREFS.LAST_PLAYING_LEVEL, SCENES.LEVEL_1);
        _fader.FadeOutScreen(() => SceneLoader.LoadScene(lastPlayingLevel));
    }
}