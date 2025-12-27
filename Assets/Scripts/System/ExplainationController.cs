using UnityEngine;

public class ExplainationController : MonoBehaviour
{
    private Fader _fader;

    void Start()
    {
        _fader = FindAnyObjectByType<Fader>();

        if (!_fader)
        {
            Debug.LogError("ExplainationController: Fader component is missing in the scene!");
            enabled = false;
            return;
        }

        // フェードインでタイトル画面表示
        _fader.FadeInScreen();

        // カーソルを表示にする
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToNextScene()
    {
        // フェードアウトして次のシーンへ移動
        _fader.FadeOutScreen(() => SceneLoader.LoadScene(SCENES.ENTRY_CUTSCENE));
    }
}