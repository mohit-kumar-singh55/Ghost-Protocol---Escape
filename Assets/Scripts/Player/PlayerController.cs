using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [Tooltip("Time to wait before next dash")]
    [SerializeField] private float _dashCooldownTime = 3f;

    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject _gun;
    #endregion

    #region Properties
    private Rigidbody2D _rb;
    private Animator _playerAnimator;
    private Animator _gunAnimator;
    private float _moveX;
    private float _dashTimer = 0;
    private bool _isShooting;

    // animator params
    private readonly int Speed = Animator.StringToHash("Speed");
    private readonly int IsShooting = Animator.StringToHash("IsShooting");
    #endregion

    void Awake()
    {
        // get references
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        _gunAnimator = _gun.TryGetComponent(out Animator anim) ? anim : null;
    }

    void Update()
    {
        // update dash timer
        if (_dashTimer > 0) _dashTimer -= Time.deltaTime;

        _playerAnimator.SetFloat(Speed, Mathf.Abs(_rb.linearVelocityX) / _speed);
    }

    void FixedUpdate()
    {
        float targetVelocity = _moveX * _speed;
        float velocityChange = targetVelocity - _rb.linearVelocityX;
        // _rb.linearVelocityX = move;
        _rb.AddForceX(velocityChange, ForceMode2D.Force);
    }

    // Input System Callbacks
    private void OnMove(InputValue val)
    {
        _moveX = val.Get<Vector2>().x;

        // flip sprite, only if the player is moving
        if (_moveX == 0) return;
        transform.rotation = _moveX < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void OnShoot(InputValue val)
    {
        _isShooting = val.isPressed;
        _playerAnimator.SetBool(IsShooting, _isShooting);
        _gun.SetActive(_isShooting);
    }

    private void OnSprint(InputValue val)
    {
        if (_dashTimer > 0) return;

        // start cooldown timer
        _dashTimer = _dashCooldownTime;

        // stop shooting
        _isShooting = false;
        _playerAnimator.SetBool(IsShooting, _isShooting);
        _gun.SetActive(_isShooting);

        // sudden force to dash
        _rb.AddForceX(_moveX * _speed, ForceMode2D.Impulse);
    }
}