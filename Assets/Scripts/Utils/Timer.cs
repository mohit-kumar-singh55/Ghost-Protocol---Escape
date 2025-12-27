using System;
using UnityEngine;

/// <summary>
/// タイマーを管理するクラス
/// </summary>
public class Timer : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField] private float startTime = 90f;
    #endregion

    #region Properties
    private float currentTime;
    private bool isRunning = true;
    private UIManager uiManager;

    public float CurrentTime => currentTime;
    #endregion

    #region Events
    public static event Action OnTimerEnd = delegate { };
    #endregion

    void Start()
    {
        uiManager = UIManager.Instance;

        if (!uiManager)
        {
            Debug.LogError("UIManager not found in scene!");
            enabled = false;
            return;
        }

        currentTime = startTime;
    }

    // ** タイマーの更新 **
    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;

            // ゲームオーバー
            OnTimerEnd?.Invoke();
            return;
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // UI更新
        uiManager.SetTimerText(minutes, seconds);
    }
}