using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] private Animator _dashKeyAnimator;

    private const string DASH_KEY_ANIMATOR_PARAM = "KeyZoom";
    private readonly int _dashKeyHash = Animator.StringToHash(DASH_KEY_ANIMATOR_PARAM);

    public void SetTimerText(float min, float sec) => timerText.text = string.Format("{0:00}:{1:00}", min, sec);

    public void PlayDashKeyAnimation(bool play = true)
    {
        if (!_dashKeyAnimator) return;

        if (play) _dashKeyAnimator.SetBool(_dashKeyHash, true);
        else _dashKeyAnimator.SetBool(_dashKeyHash, false);
    }

    public Image[] InitializeHealthUI(int totalHealth, GameObject healthUIParent, GameObject healthUIPrefab)
    {
        // 体力 UI を最大値分まで生成する
        Image[] healthUIs = new Image[totalHealth];
        for (int i = 0; i < totalHealth; i++) healthUIs[i] = Instantiate(healthUIPrefab, healthUIParent.transform).GetComponent<Image>();
        return healthUIs;
    }

    public void UpdateHealthUI(int currentHealth, Image[] healthUIs, Sprite EmptyhealthImg)
    {
        for (int i = 0; i < healthUIs.Length; i++)
        {
            if (i < currentHealth) continue;
            healthUIs[i].sprite = EmptyhealthImg;
        }
    }
}