using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerController), typeof(PlayerInput))]
[RequireComponent(typeof(ParticleSystem), typeof(SpriteRenderer), typeof(PlayerShoot))]
public class PlayerHealth : Health
{
    #region Serialized Fields
    [SerializeField] protected GameObject _healthUIParent;
    [SerializeField] protected GameObject _healthUIPrefab;
    [Tooltip("次の順番になるように入れる, Full Heart -> Empty Heart")]
    [SerializeField] protected Sprite[] _healthUIImages;
    #endregion

    #region Properties
    private UIManager _uiManager;
    private Image[] _healthUIs;       // 各体力 UI の Image コンポーネント

    // 死亡時にプレイヤー操作を無効化するため
    private Rigidbody2D _rb;
    private PlayerShoot _playerShoot;
    private PlayerInput _playerInput;
    private ParticleSystem _deathEffect;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;
    #endregion

    public static event Action OnPlayerDeath = delegate { };

    protected override void Start()
    {
        base.Start();

        // init
        _uiManager = UIManager.Instance;
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerShoot = GetComponent<PlayerShoot>();
        _deathEffect = GetComponent<ParticleSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController = GetComponent<PlayerController>();

        // 体力 UI を最大値分まで生成する
        _healthUIs = _uiManager.InitializeHealthUI(_maxHealth, _healthUIParent, _healthUIPrefab);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // 体力 UI を更新
        _uiManager.UpdateHealthUI(_currentHealth, _healthUIs, _healthUIImages[^1]);

        // ** lose condition **
        if (_currentHealth == 0) StartCoroutine(PlayerDeathHandler());
    }

    private IEnumerator PlayerDeathHandler()
    {
        // プレイヤー操作を無効化する
        _rb.simulated = false;
        _playerInput.enabled = false;
        _playerShoot.enabled = false;
        _spriteRenderer.enabled = false;
        _playerController.enabled = false;
        _playerController.transform.GetChild(0).gameObject.SetActive(false);    // disable sprite skin

        // 死亡エフェクトを再生
        _deathEffect.Play();
        float duration = _deathEffect.main.duration;
        CameraController.Instance.ShakeCamera(0.2f);

        // エフェクが終わるまで待機
        yield return new WaitForSeconds(duration);

        // 死亡処理
        OnPlayerDeath?.Invoke();
    }
}
