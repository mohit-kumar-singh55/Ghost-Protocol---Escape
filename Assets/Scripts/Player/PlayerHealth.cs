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
    [Tooltip("Should be in order, Full Heart -> Empty Heart")]
    [SerializeField] protected Sprite[] _healthUIImages;
    #endregion

    #region Properties
    private UIManager _uiManager;
    private Image[] _healthUIs;       // image component of each health ui

    // to disable player controls on death
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

        // populate total health ui
        _healthUIs = _uiManager.InitializeHealthUI(_maxHealth, _healthUIParent, _healthUIPrefab);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // update health ui
        _uiManager.UpdateHealthUI(_currentHealth, _healthUIs, _healthUIImages[^1]);

        // ** lose condition **
        if (_currentHealth == 0) StartCoroutine(PlayerDeathHandler());
    }

    private IEnumerator PlayerDeathHandler()
    {
        // disable player controls
        _rb.simulated = false;
        _playerInput.enabled = false;
        _playerShoot.enabled = false;
        _spriteRenderer.enabled = false;
        _playerController.enabled = false;
        _playerController.transform.GetChild(0).gameObject.SetActive(false);    // disable sprite skin

        // play death effect
        _deathEffect.Play();
        float duration = _deathEffect.main.duration;
        CameraController.Instance.ShakeCamera(0.2f);

        // wait for effect to finish
        yield return new WaitForSeconds(duration);

        // invoke player death event
        OnPlayerDeath?.Invoke();
    }
}
