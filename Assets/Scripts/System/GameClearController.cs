using UnityEngine;

public class GameClearController : MonoBehaviour
{
    private Fader _fader;

    void Start()
    {
        _fader = FindAnyObjectByType<Fader>();

        if (!_fader)
        {
            Debug.LogError("GameClearController: Fader component is missing in the scene!");
            enabled = false;
            return;
        }

        // フェードインでゲームオーバー画面表示
        _fader.FadeInScreen();

        // カーソルを表示にする
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToTitle()
    {
        _fader.FadeOutScreen(() => SceneLoader.LoadScene(SCENES.TITLE));
    }
}